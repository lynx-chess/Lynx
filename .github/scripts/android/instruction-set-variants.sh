#!/usr/bin/env bash

# Prototype: the Android ARM64 variants (same ILC ARM64 code generation as linux-bionic-arm64) and the JIT on this runner's CPU.
# Each one must give the reference bench, unless the CPU lacks its instruction sets (NativeAOT then refuses to start). NPS is informative.
# v8.9 needs FEAT_CSSC ('cssc' in /proc/cpuinfo Features): without it, it's skipped. The JIT uses CSSC by itself where the CPU has it.
# Usage, from the repository root on an arm64 Linux machine, after publishing the NativeAOT default build to artifacts/linux-arm64-aot:
#   instruction-set-variants.sh <expected bench node count>

set -e   # Same as GitHub's default `run` shell (bash -e)

expected=${1:?Usage: instruction-set-variants.sh <expected bench node count>}

grep -m1 '^Features' /proc/cpuinfo || true
lscpu | grep -E 'Vendor ID|Model name' || true

dotnet publish src/Lynx.Cli/Lynx.Cli.csproj --runtime linux-arm64 --self-contained /p:Optimized=true -o variants/jit
# <name>:<IlcInstructionSet>. %2C: MSBuild splits /p: values on commas
for v in v8.2-dotprod:armv8.2-a%2Cdotprod v8.9:armv8.9-a; do
  dotnet publish src/Lynx.Cli/Lynx.Cli.csproj --runtime linux-arm64 --self-contained /p:Optimized=true /p:PublishAot=true "/p:IlcInstructionSet=${v#*:}" -o "variants/${v%%:*}"
done
cp -r artifacts/linux-arm64-aot variants/v8.0

runnable=""
for n in jit v8.0 v8.2-dotprod v8.9; do
  out=$(printf 'runtimeconfig\nquit\n' | timeout 30 "variants/$n/Lynx.Cli" 2>&1) || true
  if echo "$out" | grep -q 'missing one or more'; then
    echo "::warning::$n doesn't run on this CPU: $(echo "$out" | grep -m1 'missing')"
    continue
  fi
  echo "$n: $(echo "$out" | grep -E -- '- (JIT|AdvSimd|Crc32|Rdm|Dp|Sve) =' | tr -d '\t' | sed 's/^- //' | tr '\n' ' ')"
  runnable="$runnable $n"
done

# Interleaved rounds, so that runner noise affects all variants alike
: > nps.txt
for round in 1 2 3 4 5; do
  for n in $runnable; do
    line=$(timeout 300 "variants/$n/Lynx.Cli" bench | grep -E '^[0-9]+ nodes')
    if [ "$(echo "$line" | cut -d' ' -f1)" != "$expected" ]; then
      echo "::error::$n bench: $line, x64 JIT: $expected nodes"
      exit 1
    fi
    echo "$n $(echo "$line" | cut -d' ' -f3)" >> nps.txt
  done
done

echo "Median NPS of 5 rounds, and relative to v8.0 (NativeAOT default):"
for n in $runnable; do echo "$n $(grep "^$n " nps.txt | cut -d' ' -f2 | sort -n | sed -n 3p)"; done > medians.txt
base=$(grep '^v8.0 ' medians.txt | cut -d' ' -f2)
awk -v base="$base" '{ printf "  %-12s %9d nps  %+6.1f%%\n", $1, $2, base ? 100 * ($2 / base - 1) : 0 }' medians.txt
