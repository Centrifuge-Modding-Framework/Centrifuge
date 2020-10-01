#!/bin/bash

art="./__BUILD/artifact"
shared="./Centrifuge.Shared/Binaries"
net35_bin="./__BUILD/Release/net35"
net35_art="$art/net35"

std20_bin="./__BUILD/Release/netstandard20"
std20_art="$art/netstandard20"

installer_bin="./__BUILD/installer/Release/net461"

api_ver=$(sed -n 's/<Version>\(.*\)<\/Version>/\1/p' Reactor.API/Reactor.API.csproj | awk '{$1=$1};1')

platform="$(uname -s)"
case "${platform}" in
    Linux*)     machine="Linux";;
    Darwin*)    machine="Mac";;
    CYGWIN*)    machine="Cygwin";;
    MINGW*)     machine="MinGw";;
    *)          machine="Unsupported"
esac

rm -rf $art 2> /dev/null

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
cp $shared/System.Runtime.Serialization.dll $net35_art/Managed/

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

cd $net35_art
echo "Packaging .NET 3.5 build..."
zip -r9 ../Centrifuge.$api_ver.net35.zip Centrifuge Managed
cd ../../../
echo

cd $std20_art
echo "Packaging .NET Standard 2.0 build..."
zip -r9 ../Centrifuge.$api_ver.netstandard.zip Centrifuge Managed
cd ../../../
echo

if [ "$platform" == "Linux" ] || [ "$platform" == "Mac" ] || [ "$platform" == "Unsupported" ]; then
    echo "Skipping installer packaging as we're not running on Windows..."
else
    echo "Packaging installer..."

    cd $installer_bin
    zip -rj9 ../../../../$art/Centrifuge.Installer.zip .
    cd ../../../../
    echo
fi

stat -c "[%s bytes] %n" $art/Centrifuge.$api_ver.net35.zip
stat -c "[%s bytes] %n" $art/Centrifuge.$api_ver.netstandard.zip
stat -c "[%s bytes] %n" $art/Centrifuge.Installer.zip
