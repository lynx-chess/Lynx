#!/usr/bin/env bash

# Informative: the smallest thread stack that a deep search needs. Lynx searches up to 128 plies deep, and Android's default thread stack is ~1 MB.
# ulimit -s sets glibc's default thread stack size, so this only works on glibc Linux builds that don't set their own stack size.
# Usage: stack-needed.sh <path to Lynx.Cli> [FEN]
#
# Default position (also uci-session.sh's deep recursion test): locked pawns, so only the kings can move and neither can cross. Single-threaded, its depth 128 search
# reaches seldepth 99 in ~5 M nodes (deterministic). That's as deep as lines with only king moves get, since the 50-move rule ends them at ply 100.
# Out of ~2,000 positions tried (every placement of the kings, with and without spare pawns behind the barrier), Ka1/ka8 reached 76.
# Spare pawns let a few lines go past 100 (105 at most), but only in the last iterations, so a small search change can lose them.

set -e   # Same as GitHub's default `run` shell (bash -e)

engine=${1:?Usage: stack-needed.sh <path to Lynx.Cli> [FEN]}
fen=${2:-8/8/6k1/p1p1p1p1/P1P1P1P1/8/2K5/8 w - - 0 1}

deep() { # <stack KB>: prints the deepest seldepth of a depth 128 search, fails if the engine crashes
  ( ulimit -c 0; ulimit -s "$1"
    d=$(mktemp -d); mkfifo "$d/in"; : > "$d/out"
    "$engine" < "$d/in" >> "$d/out" 2>&1 &
    pid=$!
    exec 3> "$d/in"
    echo "ucinewgame" >&3; echo "position fen $fen" >&3; echo "go depth 128" >&3
    for i in $(seq 600); do grep -q '^bestmove' "$d/out" && break; kill -0 $pid 2>/dev/null || exit 1; sleep 0.1; done
    echo quit >&3; wait $pid || exit 1
    grep -oE 'seldepth [0-9]+' "$d/out" | cut -d' ' -f2 | sort -n | tail -1 )
}

# Binary search of the smallest stack that still reaches the seldepth of an 8 MB stack
reference=$(deep 8192)
lo=64; hi=2048
while [ $((hi - lo)) -gt 8 ]; do
  mid=$(( (lo + hi) / 2 ))
  if [ "$(deep $mid)" = "$reference" ]; then hi=$mid; else lo=$mid; fi
done
echo "seldepth $reference needs ~$hi KB of thread stack, ~$(( hi * 1024 / reference )) bytes per ply"
