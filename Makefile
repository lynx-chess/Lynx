.DEFAULT_GOAL := publish

OUTPUT_DIR:=artifacts/Lynx/

ifeq ($(OS),Windows_NT)
    ifeq ($(PROCESSOR_ARCHITEW6432),AMD64)
        RUNTIME=win-x64
    else
        ifeq ($(PROCESSOR_ARCHITECTURE),AMD64)
            RUNTIME=win-x64
        else ifeq ($(PROCESSOR_ARCHITECTURE),x86)
            RUNTIME=win-x86
        endif
    endif
else
    UNAME_S := $(shell uname -s)
	UNAME_P := $(shell uname -p)
    ifeq ($(UNAME_S),Linux)
		ifeq ($(UNAME_P),x86_64)
			RUNTIME=linux-x64
		else ifneq ($(filter aarch64%,$(UNAME_P)),)
			RUNTIME=linux-arm64
		else ifneq ($(filter armv8%,$(UNAME_P)),)
			RUNTIME=linux-arm64
		else ifneq ($(filter arm%,$(UNAME_P)),)
			RUNTIME=linux-arm
		endif
	else ifeq ($(UNAME_S),Darwin)
		ifeq ($(UNAME_P),x86_64)
			RUNTIME=osx-x64
		else ifneq ($(filter arm%,$(UNAME_P)),)
			RUNTIME=osx.11.0-arm64
		endif
    endif
endif

build:
	dotnet build -c Release

test:
	dotnet test -c Release & dotnet test -c Release --filter "TestCategory=LongRunning"

publish:
	dotnet publish src/Lynx.Cli/Lynx.Cli.csproj -c Release --runtime ${RUNTIME} --self-contained /p:Optimized=true -o ${OUTPUT_DIR}

run:
	dotnet run --project src/Lynx.Cli/Lynx.Cli.csproj -c Release --runtime ${RUNTIME}