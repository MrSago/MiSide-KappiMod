@echo off
setlocal EnableDelayedExpansion

dotnet build -c Release -p:Loader=BIE -p:Platform=x64
if errorlevel 1 goto :error

set "SOURCE=.\bin\Release\BIE"
set "DEST=C:\Program Files (x86)\Steam\steamapps\common\MiSide\BepInEx\plugins\KappiMod"
set "FILE=KappiMod.BepInEx.dll"

set "USERLIBS_SRC=.\Assemblies\UserLibs"
set "USERLIBS_DEST=C:\Program Files (x86)\Steam\steamapps\common\MiSide\BepInEx\plugins\KappiMod"
set "UNIVERSELIB=UniverseLib.BIE.IL2CPP.Interop.dll"

robocopy "%SOURCE%" "%DEST%" "%FILE%" /NFL /NDL /NJH /NJS
if errorlevel 8 goto :error

robocopy "%USERLIBS_SRC%" "%USERLIBS_DEST%" "%UNIVERSELIB%" /NFL /NDL /NJH /NJS /XO
if errorlevel 8 goto :error

echo Build and copy completed successfully
exit /b 0

:error
echo Failed with error #%errorlevel%.
exit /b %errorlevel%
