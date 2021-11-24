.DEFAULT_GOAL := publish

RUNTIME=
OUTPUT_DIR:=artifacts/Lynx/

ifeq ($(OS),Windows_NT)
    ifeq ($(PROCESSOR_ARCHITEW6432),AMD64)
        RUNTIME=win-x64
	else
		RUNTIME=win-x86
	endif
else
	UNAME_S := $(shell uname -s)
	UNAME_P := $(shell uname -p)
	ifeq ($(UNAME_S),Linux)
	    RUNTIME=linux-x64
		ifneq ($(filter aarch64%,$(UNAME_P)),)
			RUNTIME=linux-arm64
		else ifneq ($(filter armv8%,$(UNAME_P)),)
			RUNTIME=linux-arm64
		else ifneq ($(filter arm%,$(UNAME_P)),)
			RUNTIME=linux-arm
		endif
	else ifneq ($(filter arm%,$(UNAME_P)),)
		RUNTIME=osx.11.0-arm64
	else
		RUNTIME=osx-x64
	endif
endif

ifndef RUNTIME
$(error RUNTIME is not set for $(OS) $(UNAME_S) $(UNAME_P), please fill an issue in https://github.com/lynx-chess/Lynx/issues/new/choose)
endif

build:
	dotnet build -c Release

test:
	dotnet test -c Release & dotnet test -c Release --filter "TestCategory=LongRunning" & dotnet test -c Release --filter "TestCategory=Perft"

publish:
	dotnet publish src/Lynx.Cli/Lynx.Cli.csproj -c Release --runtime ${RUNTIME} --self-contained /p:Optimized=true -o ${OUTPUT_DIR}

run:
	dotnet run --project src/Lynx.Cli/Lynx.Cli.csproj -c Release --runtime ${RUNTIME}