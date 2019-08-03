@echo off
mkdir _build32 & pushd _build32
cmake -G "Visual Studio 15 2017" -DBITNESS="x86" ..\
popd
mkdir _build64 & pushd _build64
cmake -G "Visual Studio 15 2017 Win64" -DBITNESS="x64" ..\
popd

cmake --build _build32 --config Release
copy _build32\Release\gumbo.dll Gumbo.Net\x86

cmake --build _build64 --config Release
copy _build64\Release\gumbo.dll Gumbo.Net\x64