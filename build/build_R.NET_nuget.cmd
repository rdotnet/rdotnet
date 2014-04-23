f:

set MSB=%windir%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe
if not exist %MSB% goto MSBuild_not_found

:: ======= NuGet settings
:: Get the nuget tools from nuget.org. There is also one coming with the NuGet plug-on from Visual Studio.
set nuget_exe=f:\bin\NuGet.exe
if not exist %nuget_exe% goto Nuget_not_found

:: Section on NuGet.config for nuget update (NOT YET USED - CAME ACCROSS ISSUE WITH NUGET 2.8)
:: To limit machine specific issues, we will require an explicit nuget config file.
:: Usually you will have a config file %AppData%\NuGet\NuGet.config.
:: A gotcha is that even if you have configured your package feed from visual studio, you may need to also add a key to the activePackageSource
::  <activePackageSource>
::    <add key="per202 nuget tests" value="\\path\to\work\per202\nuget" />
::    <add key="nuget.org" value="https://www.nuget.org/api/v2/" />
::  </activePackageSource>
set nuget_conf_file=%AppData%\NuGet\NuGet.config
:: You can also adapt from the sample NuGet.config.sample in the same directory as this file
:: set nuget_conf_file=%~d0%~p0\NuGet.config
:: if not exist %nuget_exe% goto Nuget_config_not_found

:: ======= 
:: The command to use to delete whole directories, so that we force the update of packages over the build process
:: the DOS rmdir does not work on wildcard. At least, have not found a way to make it work. 
:: set rm_cmd=rmdir /S /Q
:: Instead, using for the time being the 'rm' from MinGW that comes with the RTools 
:: toolchain, used to build R and R packages on Windows. Any MinGW setup should to.
set rm_cmd=rm -rf

:: The target where we will put the resulting nuget packages.
set repo_dir=%~d0%~p0
if not exist %repo_dir% mkdir %repo_dir%


:: ================== Location of the source code ========================

:: https://hg.codeplex.com/rdotnet branch jperraud
set rdotnet_dir=%~d0%~p0..\
:: The xcopy options for the nuget packages (and some other build outputs)
set COPYOPTIONS=/Y /R /D

:: ================== code compilation settings
set BuildConfiguration=Release
set BuildConfiguration=Debug

:: Setting the variable named 'Platform' seems to interfere with the nuget pack command, so 
:: we deliberately set a variable BuildPlatform for use with MSBuild.exe
set BuildPlatform="Any CPU"
set Mode=Rebuild
:: set Mode=Build

:: ================== Start build process ========================
:: EVERYTHING else below this line should use paths relative to the lines above, or environment variables

set build_options=/t:%Mode% /p:Configuration=%BuildConfiguration% /p:Platform=%BuildPlatform%
set common_ng_pack_options=-Verbosity normal -Properties Configuration=%BuildConfiguration%

:: ================== R.NET ========================
:r_dot_net
:: package R.NET
:: TODO - could not get the F# build script to work.
:: @if "%VS120COMNTOOLS%"=="" echo "WARNING - env var VS120COMNTOOLS not found - fsharp interactive may not be found!"
:: if not "%VS120COMNTOOLS%" == "" (
:: call "%VS120COMNTOOLS%VsDevCmd.bat"
:: )
:: fsi.exe %rdotnet_dir%tools\build.fsx --debug
:: error FS0193: internal error: Value cannot be null.
:: Parameter name: con

set SLN=%rdotnet_dir%RDotNet.Release.sln
%MSB% %SLN% %build_options%

set pack_options=-OutputDirectory %rdotnet_dir%build %common_ng_pack_options%
if exist %repo_dir%R.NET.1.5.*.nupkg del %repo_dir%R.NET.1.5.*.nupkg  
%nuget_exe% pack %rdotnet_dir%RDotNet.nuspec %pack_options%
REM %nuget_exe% pack %rdotnet_dir%RDotNet.FSharp.nuspec %pack_options%
:: %repo_dir%RDotNet.FSharp.0.*.nupkg
xcopy %rdotnet_dir%build\*.nupkg %repo_dir% %COPYOPTIONS%
goto completed

:MSBuild_not_found
echo "ERROR: MSBuild.exe not found at the location given"
exit 1

:Nuget_not_found
echo "ERROR: NuGet.exe not found at the location given"
exit 1

:Nuget_config_not_found
echo "ERROR: NuGet.config not found at the location given"
exit 1

:completed
echo "INFO: Batch build completed with no known error"
