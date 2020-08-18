#!/bin/bash
net35_bin="./__BUILD/Release/net35"
net35_art="./__BUILD/artifact/net35"

std20_bin="./__BUILD/Release/netstandard20"
std20_art="./__BUILD/artifact/netstandard20"

rm -rf $net35_art 2> /dev/null
rm -rf $std20_art 2> /dev/null

mkdir -p $net35_art/Centrifuge
mkdir -p $net35_art/Managed
mkdir -p $std20_art/Centrifuge
mkdir -p $std20_art/Managed

cp $net35_bin/Reactor.dll $net35_art/Centrifuge/
cp $net35_bin/Reactor.API.dll $net35_art/Centrifuge/
cp $net35_bin/Centrifuge.dll $net35_art/Managed/
cp $net35_bin/Centrifuge.UnityInterop.dll $net35_art/Managed/
cp $net35_bin/0Harmony.dll $net35_art/Managed/
cp $net35_bin/Newtonsoft.Json.dll $net35_art/Managed/
cp $net35_bin/Mono.Cecil.dll $net35_art/Managed/
cp $net35_bin/MonoMod.RuntimeDetour.dll $net35_art/Managed/
cp $net35_bin/MonoMod.Utils.dll $net35_art/Managed/
cp $net35_bin/Spindle.exe $net35_art/Managed/
cp $net35_bin/install_windows.bat $net35_art/Managed/
cp $net35_bin/install_linux.sh $net35_art/Managed/

cp $std20_bin/Reactor.dll $std20_art/Centrifuge/
cp $std20_bin/Reactor.API.dll $std20_art/Centrifuge/
cp $std20_bin/Centrifuge.dll $std20_art/Managed/
cp $std20_bin/Centrifuge.UnityInterop.dll $std20_art/Managed/
cp $std20_bin/0Harmony.dll $std20_art/Managed/
cp $std20_bin/Newtonsoft.Json.dll $std20_art/Managed/
cp $std20_bin/Mono.Cecil.dll $std20_art/Managed/
cp $std20_bin/MonoMod.RuntimeDetour.dll $std20_art/Managed/
cp $std20_bin/MonoMod.Utils.dll $std20_art/Managed/
cp $std20_bin/Spindle.dll $std20_art/Managed/Spindle.exe
cp $std20_bin/install_windows.bat $std20_art/Managed/
cp $std20_bin/install_linux.sh $std20_art/Managed/
cp $std20_bin/System.* $std20_art/Managed/