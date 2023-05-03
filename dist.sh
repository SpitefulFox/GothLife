#!/bin/bash
set -euo pipefail
IFS=$'\n\t'

cd "$(dirname "$0")"

MOD_NAME="GothLife"
DOTNET_VERSION="7.0"

if grep -qE "(Microsoft|WSL)" /proc/version &> /dev/null ; then
    LOCALAPPDATA="$(wslpath "$(cmd.exe /C "echo %LOCALAPPDATA%")" | tr -d '\r')"
fi

if [ $# -gt 0 ] && [ "$1" == "-s" ]; then
    SKIP_BUILD=1
fi

if [ -z ${SKIP_BUILD+x} ]; then
    dotnet build
fi

rm -fv dist/*.zip
mkdir -p dist
pushd "bin/Debug/net$DOTNET_VERSION/"
rm -fv "$MOD_NAME.pdb"
zip -r "../../../dist/$MOD_NAME.zip" .
popd

cp -v dist/*.zip "$LOCALAPPDATA/Tiny Life/Mods"
