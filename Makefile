.DEFAULT_GOAL := publish
.PHONY: build test test-default test-configuration test-longrunning test-perft publish run

EXE =

PUBLISH_DIR = artifacts/Lynx/
OUTPUT_DIR =

ifdef EXE
OUTPUT_DIR = ../../
endif

RUNTIME = $(shell dotnet build src/Lynx.Cli/Lynx.Cli.csproj -getProperty:NETCoreSdkPortableRuntimeIdentifier)

checked_runtime = $(if $(and $(filter 1,$(words $1)),$(findstring -,$1)),$1,$(error Could not get the runtime identifier from the .NET SDK (got '$1'). You can try pass it explicitly, i.e. 'make RUNTIME=linux-x64'.))

build:
	dotnet build -c Release

publish:
	dotnet publish src/Lynx.Cli/Lynx.Cli.csproj --runtime $(call checked_runtime,$(RUNTIME)) --self-contained /p:Optimized=true /p:ExecutableName=$(EXE) -o $(PUBLISH_DIR) /p:FinalDestination=$(OUTPUT_DIR) --tl:off

run:
	dotnet run --project src/Lynx.Cli/Lynx.Cli.csproj -c Release

# 'make -j4 test' runs the four suites in parallel, after a single build
test: test-default test-configuration test-longrunning test-perft

test-default test-configuration test-longrunning test-perft: build

test-default:
	dotnet test -c Release --no-build

test-configuration:
	dotnet test -c Release --no-build --filter "TestCategory=Configuration"

test-longrunning:
	dotnet test -c Release --no-build --filter "TestCategory=LongRunning"

test-perft:
	dotnet test -c Release --no-build --filter "TestCategory=Perft"
