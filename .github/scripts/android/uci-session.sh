#!/usr/bin/env bash

# A UCI session that waits for each bestmove before sending the next command (Lynx doesn't exit on stdin EOF).
# Usage: uci-session.sh <engine command...>, e.g. uci-session.sh adb shell 'cd /data/local/tmp/lynx && ./Lynx.Cli', or uci-session.sh ./Lynx.Cli
# PROBE (optional environment variable): a command run while the engine has 8 search threads (see stack-probe.sh).
# It runs on the host, and adb forwards the engine's stdin, since a FIFO can't be created on the device:
# Android 7 (API 24) has no mkfifo, and Android 16 (API 36) doesn't let adb's shell create FIFOs in /data/local/tmp (mkfifo: Permission denied)

set -u

dir=$(mktemp -d)
mkfifo "$dir/in"
: > "$dir/out"
"$@" < "$dir/in" >> "$dir/out" 2>&1 &
pid=$!
exec 3> "$dir/in"
gos=0   # go commands sent: each one must get exactly one bestmove

send() { echo "$1" >&3; }
fail() { echo "::error::$1"; kill $pid 2>/dev/null; cat "$dir/out"; exit 1; }
bestmoves() { grep -c '^bestmove' "$dir/out"; }
lines() { wc -l < "$dir/out"; }
wait_for() { # <pattern> <count> <timeout in seconds>
  local i=0
  while [ "$(grep -c "$1" "$dir/out")" -lt "$2" ]; do
    kill -0 $pid 2>/dev/null || fail "Engine exited while waiting for '$1' x$2"
    i=$((i + 1)); [ $i -le $(( $3 * 10 )) ] || fail "Timeout waiting for '$1' x$2"
    sleep 0.1
  done
}
go() { send "$1"; gos=$((gos + 1)); wait_for '^bestmove' $gos $2; }   # <go command> <timeout in seconds>
go_async() { send "$1"; gos=$((gos + 1)); }                          # go infinite/ponder: bestmove after stop/ponderhit
max_seldepth_since() { tail -n +$(( $1 + 1 )) "$dir/out" | grep -oE 'seldepth [0-9]+' | cut -d' ' -f2 | sort -n | tail -1; }

send uci; wait_for uciok 1 10
send isready; wait_for readyok 1 10
send runtimeconfig; send isready; wait_for readyok 2 10
send "position startpos"; go "go depth 12" 60

# Deep recursion: in a locked position (only the kings can move), iterative deepening gets to MaxDepth (128) in seconds
# and single-threaded search lines go 99 plies deep: ~770 KB of stack (~7.7 KB per ply), Android's default thread stack being ~1 MB.
locked='8/8/6k1/p1p1p1p1/P1P1P1P1/8/2K5/8 w - - 0 1'
send ucinewgame; start=$(lines); send "position fen $locked"; go "go depth 128" 120
seldepth=$(max_seldepth_since $start); echo "Locked position, 1 thread: seldepth $seldepth"
[ "${seldepth:-0}" -ge 90 ] || fail "Locked position search only reached seldepth ${seldepth:-0} (99 expected)"
send "position fen 8/k7/3p4/p2P1p2/P2P1P2/8/8/K7 w - - 0 1"; go "go movetime 5000" 30   # Fine #70, a deep endgame search with a plan

# Multithreading: helper threads recursing deep too, then 8 threads (twice the emulator's cores) in a complex middlegame (Kiwipete)
send "setoption name Threads value 4"; send ucinewgame
start=$(lines); send "position fen $locked"; go "go depth 128" 120
echo "Locked position, 4 threads: seldepth $(max_seldepth_since $start)"
kiwipete='r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq - 0 1'
send "setoption name Threads value 8"; send "setoption name Hash value 64"; send ucinewgame
send "position fen $kiwipete"; go "go movetime 3000" 30
[ -z "${PROBE:-}" ] || eval "$PROBE" || fail "Stack check failed"

# stop after 2 s of go infinite, and stop right after go, which can be processed before the search starts
send "position fen $kiwipete moves e2a6"; go_async "go infinite"; sleep 2; send stop; wait_for '^bestmove' $gos 10
go_async "go infinite"; send stop; wait_for '^bestmove' $gos 10

# Pondering: no bestmove until ponderhit or stop
send "setoption name Ponder value true"
send "position startpos moves e2e4 e7e5 g1f3"; go_async "go ponder wtime 10000 btime 10000 winc 100 binc 100"; sleep 1
[ "$(bestmoves)" -eq $((gos - 1)) ] || fail "bestmove sent before ponderhit"
send ponderhit; wait_for '^bestmove' $gos 30
send "position startpos moves e2e4 e7e5 g1f3 b8c6"; go_async "go ponder wtime 10000 btime 10000 winc 100 binc 100"; sleep 1
[ "$(bestmoves)" -eq $((gos - 1)) ] || fail "bestmove sent before stop"
send stop; wait_for '^bestmove' $gos 10
send "setoption name Ponder value false"

# Self-play game with a clock, changing the number of threads every 10 plies, as a GUI would
send ucinewgame; moves=""
for ply in $(seq 0 29); do
  case $ply in 0) send "setoption name Threads value 2";; 10) send "setoption name Threads value 4";; 20) send "setoption name Threads value 1";; esac
  send "position startpos${moves:+ moves$moves}"; go "go wtime 4000 btime 4000 winc 50 binc 50" 30
  move=$(grep '^bestmove' "$dir/out" | tail -1 | cut -d' ' -f2)
  case "$move" in [a-h][1-8][a-h][1-8]*) moves="$moves $move";; *) echo "Game over: bestmove $move"; break;; esac
done
echo "Self-play:$moves"

send quit
exec 3>&-
wait $pid
rc=$?
cat "$dir/out"
echo "Exit code: $rc, go commands: $gos, bestmoves: $(bestmoves)"
[ $rc -eq 0 ] || { echo "::error::Exit code $rc"; exit 1; }
[ "$(bestmoves)" -eq $gos ] || { echo "::error::Expected exactly one bestmove per go command"; exit 1; }
! grep -q -i -E 'exception|unhandled' "$dir/out" || { echo "::error::Exception in the engine output"; exit 1; }
