#!/usr/bin/env bash

# Stack sizes of an engine's threads on an Android device (through adb): /proc/<pid>/maps stack mappings, named after the thread id
# ([stack:<tid>] or [anon:stack_and_tls:<tid>]), joined with the thread names in /proc/<pid>/task/<tid>/comm.
# Usage: stack-probe.sh <process name> [minimum KB]
# With a minimum, it fails unless every thread that can search has at least that much stack:
# SearchWorker ('Lynx search worker #N', cut to 15 chars), .NET TP Worker (main search) and .NET Long Task (UCI listener, runs bench)

process=$1; min_kb=${2:-}
pid=$(adb shell pidof "$process" | tr -d '\r' | grep -oE '[0-9]+' | head -1)
[ -n "$pid" ] || pid=$(adb shell pgrep -x "$process" | tr -d '\r' | grep -oE '[0-9]+' | head -1)   # pidof doesn't find translated processes
if [ -z "$pid" ]; then
  echo "Stack probe: no $process process found"
  [ -z "$min_kb" ]; exit
fi

threads=$(adb shell "cd /proc/$pid/task && for t in *; do echo \$t \$(cat \$t/comm); done" | tr -d '\r')
# Only anonymous [...stack...] mappings: file mappings such as /system/lib64/libunwindstack.so aren't stacks
stacks=$(adb shell "cat /proc/$pid/maps" | tr -d '\r' | grep -E '\[[^]]*stack[^]]*\]$' | while read -r range perms offset device inode mapping; do
  tid=$(echo "$mapping" | grep -oE '[0-9]+\]$' | tr -d ']')
  thread=$([ -z "$tid" ] || echo "$threads" | awk -v tid="$tid" '$1 == tid { $1 = ""; print substr($0, 2) }')
  echo "$(( (0x${range#*-} - 0x${range%-*}) / 1024 )) KB | ${thread:--} | $mapping"
done)
echo "Stack mappings of $process (pid $pid): count, size | thread | mapping"
echo "$stacks" | sed -E 's/[0-9]+\]$/<tid>]/' | sort -n | uniq -c

[ -n "$min_kb" ] || exit 0
search='^(Lynx search|[.]NET TP Worker|[.]NET Long Task)'
count=$(echo "$stacks" | awk -F' [|] ' -v re="$search" '$2 ~ re' | wc -l)
small=$(echo "$stacks" | awk -F' [|] ' -v re="$search" -v min="$min_kb" '$2 ~ re && $1 + 0 < min')
[ "$count" -gt 0 ] || { echo "::error::No search thread stack mappings found"; exit 1; }
[ -z "$small" ] || { echo "::error::Search thread stacks smaller than $min_kb KB:"; echo "$small"; exit 1; }
echo "All $count search thread stacks have at least $min_kb KB"
