#!/usr/bin/env bash

# Tests the Android builds on a running emulator or device, through adb: bench node count, UCI session and thread stack sizes.
# Usage, from the repository root, with the build job's artifacts downloaded to artifacts/Lynx-linux-bionic-*:
#   emulator-tests.sh <expected bench node count>

set -euxo pipefail

expected=$1
here=$(dirname "${BASH_SOURCE[0]}")

adb shell getprop ro.build.version.release
abis=$(adb shell getprop ro.product.cpu.abilist | tr -d '\r')
adb shell getconf PAGESIZE || true
adb shell mkdir -p /data/local/tmp/lynx
adb push artifacts/Lynx-linux-bionic-x64/Lynx.Cli /data/local/tmp/lynx/Lynx.Cli
adb push artifacts/Lynx-linux-bionic-arm64-v8.0/Lynx.Cli /data/local/tmp/lynx/Lynx.Cli-arm64
adb push artifacts/Lynx-linux-bionic-arm64-v8.2-dotprod/Lynx.Cli /data/local/tmp/lynx/Lynx.Cli-arm64-v8.2-dotprod
adb push artifacts/Lynx-linux-bionic-arm64-v8.9/Lynx.Cli /data/local/tmp/lynx/Lynx.Cli-arm64-v8.9
adb shell chmod 755 /data/local/tmp/lynx/Lynx.Cli /data/local/tmp/lynx/Lynx.Cli-arm64 /data/local/tmp/lynx/Lynx.Cli-arm64-v8.2-dotprod /data/local/tmp/lynx/Lynx.Cli-arm64-v8.9

bench() { # <executable>
  output=$(timeout 1800 adb shell "cd /data/local/tmp/lynx && ./$1 bench" | tr -d '\r')
  echo "$output"
  nodes=$(echo "$output" | grep -oE '^[0-9]+ nodes' | cut -d' ' -f1)
  if [ "$nodes" != "$expected" ]; then
    echo "::error::Android emulator bench ($1): $nodes nodes, x64 JIT: $expected nodes"
    exit 1
  fi
}
session() { # <executable> [minimum stack KB of search threads]
  PROBE="bash $here/stack-probe.sh $1 ${2:-}" timeout 1200 bash "$here/uci-session.sh" adb shell "cd /data/local/tmp/lynx && ./$1"
}

bench Lynx.Cli
# Search threads must have the 8 MB stacks set in Lynx.Cli.csproj (Android's ~1 MB default is too tight for 128 plies)
session Lynx.Cli 4096

# The arm64 build, the one for phones, only runs in images with ARM translation (API 36's lists arm64-v8a, API 24's doesn't)
if [[ ",$abis," == *",arm64-v8a,"* ]]; then
  bench Lynx.Cli-arm64
  session Lynx.Cli-arm64   # Informative stack probe only: under translation, the translator allocates the guest stacks
  # Informative: whether the translator supports the v8.2-dotprod and v8.9 variants. If it doesn't, NativeAOT refuses to start
  # ('The current CPU is missing one or more of the required instruction sets'), as it would on a phone with an older CPU
  for v in v8.2-dotprod v8.9; do
    out=$(printf 'runtimeconfig\nquit\n' | timeout 120 adb shell "cd /data/local/tmp/lynx && ./Lynx.Cli-arm64-$v" 2>&1 | tr -d '\r') || true
    echo "arm64-$v: $(echo "$out" | grep -E 'missing|- (RID|AdvSimd|Crc32|Rdm|Dp|Sve) =' | tr -d '\t' | tr '\n' ' ')"
  done
else
  echo "::warning::No ARM translation in this image ($abis): arm64 build not tested"
fi
