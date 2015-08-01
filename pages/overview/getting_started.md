---
title: Getting started
permalink: /getting_started/
tags: getting-started
audience: developer
keywords: overview
last_updated: 
summary: Getting started with R.NET
---

{% include linkrefs.html %} 

##  Getting set up

There is a page gathering Software Prerequisites listing the platforms on which R.NET is known to run.

As of version 1.5.10 or later, R.NET binaries are platform independent. You might need to set up a small add-on workaround on some Linux distributions (CentOS a known one), but otherwise you can just move and use the same R.NET binaries across platforms.

There is a page gathering  <a href="Software Prerequisites"></a> listing the platforms on which R.NET is known to run.

As of July 2015, NuGet is the strongly recommended way to manage dependencies on R.NET in its binary distribution form. You can find more general information about NuGet at the <a href="http://docs.nuget.org/">NuGet documentation page</a>


### Visual Studio

If you are using the NuGet packages:

You first have to install, if you have not already, the NuGet package manager via Tools - Extension and Updates:

<img src="{{ "/images/0001.png" | prepend: site.baseurl }}">

You can add the R.NET package as a dependency to one or more projects in your solution. For one project:

<img src="{{ "/images/001.png" | prepend: site.baseurl }}">

Note that you must uninstall packages dependencies or R.NET 1.5.5 or earlier, if pre-existing. R.NET 1.5.13 uses a different package identifier: *R.NET.Community*. Be sure to use the most recent entry when searching for R.NET on Nuget:

<img src="{{ "/images/005.png" | prepend: site.baseurl }}">

The NuGet system then adds a couple of references.

<img src="{{ "/images/003.png" | prepend: site.baseurl }}">

You can manage several projects in one go at the solution level:

<img src="{{ "/images/004.png" | prepend: site.baseurl }}">

You can find more general information about NuGet at NuGet documentation

### Xamarin Studio and MonoDevelop

If on MacOS, you may also want to look at the page <a href="http://rawgit.com/evelinag/Projects/master/RDotNetOnMac/output/RDotNetOnMac.html">Setting up R.NET on Mac</a>

This section's content is done with MonoDevelop version 5.9.4, on a Debian box 64 bits. As of that version, NuGet package management is readily available from MonoDevelop. Just right click on the package section and select the option "Add Packages...".

<img src="{{ "/images/nuget_monodevelop_001.png" | prepend: site.baseurl }}">

Search for the ID "R.NET.Community"; Do note that if you use "R.NET" you will also get an outdated NuGet feed, so I recomment you use the full ID "R.NET.Community".

<img src="{{ "/images/nuget_monodevelop_002.png" | prepend: site.baseurl }}">

That's it, click "Add Packages" and you shoul have the latest R.NET library.

<img src="{{ "/images/nuget_monodevelop_003.png" | prepend: site.baseurl }}">

### Updating environment variables on Linux and MacOS 

The path to libR.so (for Linux) *must* be in the environment variable {"LD_LIBRARY_PATH"} *before* the process start, otherwise the R.NET engine will not properly initialize. If this is not set up, R.NET will throw an exception with a detailed message.

For setting up on MacOS, you should read Evelyna Gabasova's <a href="Setting up R.NET on Mac|http://rawgit.com/evelinag/Projects/master/RDotNetOnMac/output/RDotNetOnMac.html"></a>

What you will need to do there depends on the Linux machine you are using. If R comes already preinstalled, and such that it uses a shared library libR.so, R.NET can use it as is.

Otherwise, you need to compile your own R from source, to get a shared R library:

```sh
LOCAL_DIR=/home/username/local
JAVAHOME=/apps/java/jdk1.7.0_25
cd ~src
cd R/
tar zxpvf R-3.0.2.tar.gz
cd R-3.0.2
./configure --prefix=$LOCAL_DIR --enable-R-shlib  CFLAGS="-g"
make
make install
```

Then prior to running a project with R.NET, you may need to update your {"LD_LIBRARY_PATH"}, and quite possibly PATH (though the latter can be done at runtime too).

```sh
LOCAL_DIR=/home/username/local
if [ "${LD_LIBRARY_PATH}" != "" ]
then
    export LD_LIBRARY_PATH=$LOCAL_DIR/lib:$LOCAL_DIR/lib64/R/lib:/usr/local/lib64:${LD_LIBRARY_PATH}
else
    export LD_LIBRARY_PATH=$LOCAL_DIR/lib:$LOCAL_DIR/lib64/R/lib:/usr/local/lib64
fi
# You may as well update the PATH environment variable, though R.NET does update it if need be.
export PATH=$LOCAL_DIR/bin:$LOCAL_DIR/lib64/R/lib:${PATH}
```

### Workaround for dlerror: 'invalid caller' issue on some Linux boxes. 

On at least one instance of one Linux flavour (CentOS), R.NET fails and 'dlerror' returns the message 'invalid caller'.
Dowload and follow the instructions in the zip file "libdlwrap.zip" included in the <a href="release:this download page|121090"></a>. If you use the source code, it is located under RDotNet.NativeLibrary/libdlwrap/

See <a href="https://rdotnet.codeplex.com/workitem/73">this issue</a> for detailed information about the issue.

## Advanced options for the R engine initialization

This is a placeholder section.

* custom CharacterConsole
* Multiple app domains
* A nuget documentation page to <a href="set up a local feed|http://docs.nuget.org/docs/creating-packages/hosting-your-own-nuget-feeds"></a>.


