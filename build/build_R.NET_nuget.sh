rdotnet_dir=~/src/github_jm/rdotnet/
MSB=xbuild

# ======= NuGet settings
# Get the nuget tools from nuget.org. There is also one coming with the NuGet plug-on from Visual Studio.
# Section on NuGet.config for nuget update (NOT YET USED - CAME ACCROSS ISSUE WITH NUGET 2.8)
# To limit machine specific issues, we will require an explicit nuget config file.
# Usually you will have a config file %AppData%\NuGet\NuGet.config.
# A gotcha is that even if you have configured your package feed from visual studio, you may need to also add a key to the activePackageSource
#  <activePackageSource>
#    <add key="per202 nuget tests" value="\\path\to\work\per202\nuget" />
#    <add key="nuget.org" value="https://www.nuget.org/api/v2/" />
#  </activePackageSource>
# nuget_conf_file=%AppData%\NuGet\NuGet.config
# You can also adapt from the sample NuGet.config.sample in the same directory as this file
# nuget_conf_file=%~d0%~p0\NuGet.config
# if not exist nuget goto Nuget_config_not_found

# The target where we will put the resulting nuget packages.
repo_dir=/home/per202/nuget/

# The xcopy options for the nuget packages (and some other build outputs)
# COPYOPTIONS=/Y /R /D

# ================== code compilation settings
# if not "$BuildConfiguration"=="Release" if not "$BuildConfiguration"=="Debug" BuildConfiguration=Release
BuildConfiguration=Release

# Setting the variable named 'Platform' seems to interfere with the nuget pack command, so 
# we deliberately a variable BuildPlatform for use with MSBuild.exe
BuildPlatform="Any CPU"
# Mode=Rebuild
Mode=Build

# ================== Start build process ========================
# EVERYTHING else below this line should use paths relative to the lines above, or environment variables

# build_options="/t:$Mode /p:Configuration=$BuildConfiguration /p:Platform=\"$BuildPlatform\""
build_options="/t:$Mode /p:Configuration=$BuildConfiguration"
common_ng_pack_options="-Verbosity normal -Properties Configuration=$BuildConfiguration"

# ================== R.NET ========================
# package R.NET
# TODO - could not get the F# build script to work.
# @if "%VS120COMNTOOLS%"=="" echo "WARNING - env var VS120COMNTOOLS not found - fsharp interactive may not be found!"
# if not "%VS120COMNTOOLS%" == "" (
# call "%VS120COMNTOOLS%VsDevCmd.bat"
# )
# fsi.exe $rdotnet_dirtools\build.fsx --debug
# error FS0193: internal error: Value cannot be null.
# Parameter name: con

SLN=$rdotnet_dir/RDotNet.Release.sln
$MSB $SLN $build_options

pack_options="-OutputDirectory $rdotnet_dir$build $common_ng_pack_options"
#if exist $repo_dir/R.NET.1.5.*.nupkg del $repo_dir/R.NET.1.5.*.nupkg  
#if "$BuildConfiguration"=="Release" nuspec_file=RDotNet.nuspec
#if "$BuildConfiguration"=="Debug" nuspec_file=RDotNet_debug.nuspec
nuspec_file=RDotNet.nuspec
nuget pack $rdotnet_dir$nuspec_file $pack_options
nuget pack $rdotnet_dir/RDotNet.FSharp.nuspec $pack_options
nuget pack $rdotnet_dir/RDotNet.Graphics.nuspec $pack_options

cp $rdotnet_dir$build/*.nupkg $repo_dir

