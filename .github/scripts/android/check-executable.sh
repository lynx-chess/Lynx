#!/usr/bin/env bash

# Checks that a linux-bionic NativeAOT build is an executable that Android can run.
# Usage: check-executable.sh <path to Lynx.Cli> <ELF machine, as readelf names it: AArch64 or X86-64>

set -e   # Same as GitHub's default `run` shell (bash -e)

exe=${1:?Usage: check-executable.sh <path to Lynx.Cli> <AArch64|X86-64>}
machine=${2:?Usage: check-executable.sh <path to Lynx.Cli> <AArch64|X86-64>}

ls -la "$(dirname "$exe")"
readelf -hlW "$exe" | grep -E 'Machine:|interpreter|LOAD'
readelf -dW "$exe" | grep NEEDED

readelf -h "$exe" | grep -q "Machine: .*$machine" || { echo "::error::ELF machine isn't $machine"; exit 1; }
readelf -lW "$exe" | grep -q 'Requesting program interpreter: /system/bin/linker64' || { echo "::error::Program interpreter isn't /system/bin/linker64"; exit 1; }

# 16 KB page size devices (Android 15+) need every LOAD segment aligned to 16 KB
if readelf -lW "$exe" | awk '$1 == "LOAD" && $NF != "0x4000"' | grep -q .; then
  echo "::error::LOAD segments aren't 16 KB aligned"
  exit 1
fi

# Only Android system libraries
if readelf -dW "$exe" | grep NEEDED | grep -v -E '\[lib(c|m|dl|log|z)\.so\]'; then
  echo "::error::Unexpected shared library dependency"
  exit 1
fi
