# Dev notes

## Debugging on .NET Core with VScode

On Linux

OmniSharp : needed to open only the rdotnet folder (workspace with multiple folders seems to fail to work). Need to ctrl-shift-P omnisharp - open project/solution to get it to sort of have intellisense working. 

.NET test explorer extension: trial and error needed to use "**/*Tests.csproj" for "Test Project Path". Only after that did it detect unit tests. Launching the test debug mode was a bit fiddly and hit and miss. 

## Trying on .NET core

Was testing .NET5 so have on my linux box: `dotnet --version` returning 5.0.100-rc.1.20452.10

`dotnet --list-sdks` `dotnet --list-runtimes` only return 5.0 related versions.

I also need to install 3.1. Following [these install instructions for dotnet on Debian](https://docs.microsoft.com/en-us/dotnet/core/install/linux-debian)

I had a "testing" section in /etc/apt/sources.list.d/microsoft-prod.list, so keeping it even if the instructions install "buster" only.

```text
deb [arch=amd64] https://packages.microsoft.com/debian/10/prod buster main
deb [arch=amd64] https://packages.microsoft.com/debian/10/prod testing main
```

`sudo apt-get install -y dotnet-sdk-3.1` but:
```
The following packages have unmet dependencies:
 dotnet-runtime-deps-3.1 : Depends: libicu but it is not installable or
                                    libicu66 but it is not installable or
                                    libicu65 but it is not installable or
```

I have the 'testing' version of Debian, so probably the cause.
  </ItemGroup>
`sudo snap install dotnet-runtime-31 --classic` is OK, but `dotnet --list-runtimes` does not list anything new in the 3.1 runtimes. 

What happens with `sudo snap alias dotnet-runtime-31.dotnet dotnet31`? `dotnet31 --list-runtimes` then seems to work as expected. Pity cannot have a central entry pooint.

[My rant on snap and dotnet-sdk](https://github.com/dotnet/runtime/issues/3793)

Too hard, too tired, don't care.

tried to install via APT but it complained about not having a libicu package dependency available to install.

```
sudo snap remove hello-world 
sudo snap aliases
sudo snap unalias dotnet31
sudo snap unalias dotnet
snap list
sudo snap remove dotnet-runtime-31
sudo snap remove dotnet-sdk
snap list
sudo snap install dotnet-sdk --classic --channel=3.1
sudo snap alias dotnet-sdk.dotnet dotnet
sudo snap install dotnet-runtime-31 --classic
```

R.NET Unit tests fail to run, still. Logged [this issue](https://github.com/rdotnet/rdotnet/issues/128). 

This [Couldn't find a valid ICU package installed on the system](https://github.com/dotnet/core/issues/5019) issue was quite a rabit hole. 

You need to set [DOTNET_ROOT](https://github.com/dotnet/sdk/issues/12359#issuecomment-543289509) if dotnet is not installed in its default location, otherwise binaries wont start with complaints about libhostfxr. The whole issue thread is quite a read...

Still stuck with my ICU package despite libraries being installed by snap AND my debian having the latest ICU version. 

if I add to a csproj:

```xml
  <PropertyGroup>
    <InvariantGlobalization>true</InvariantGlobalization>
  </PropertyGroup>
```

then `~/src/github_jm/rdotnet/TestApps/StressTest$ dotnet run` runs. But I do not like the idea of changing and not knowing the impact on things like date and time, given the local - specific beaviors of R and .NET by default (Urgh.)

```
Process terminated. Couldn't find a valid ICU package installed on the system. Set the configuration flag System.Globalization.Invariant to true if you want to run with no globalization support.
```

Solved with manual install of dotnet; [issues/128](https://github.com/rdotnet/rdotnet/issues/128)

## Debugging libR.so from VSCode

[This issue](https://github.com/rdotnet/rdotnet/issues/129)

I tried to build/install from source while having R installed from the debian repo. This may have caused libR.so to not depend on libRblas.so alongside but  the distro one. GDB was insisting on loading the repo libR.so so had to unintall the distro version, but then ldd fails to get the libRblas dependency. Uninstall `r-cran` before building a custom one. troublesome but may be required. 

`aptitude search r-cran | grep "^i" > ~/backup_debinstall.txt` then `sudo apt remove r-cran-*`

Note that if you have the conda env activated youll end up with ` -I/home/per202/anaconda3/include ` in the compilation. Not trustworthy... Deactivate. this Veeery likely causes issues.

`cd ~/src/tmp/R-4.0.2`

```sh
./configure --prefix=/usr/local --enable-R-shlib CFLAGS=" -g -Og -ggdb"  --enable-java=no --with-recommended-packages=no
```

Modify `launch.json`; see appendix section below for configuration

Kept having 'rnorm' not found; and libR.so had  `libRblas.so => not found`. Needed to put in /etc/environment: `export LD_LIBRARY_PATH="/usr/local/lib/R/lib:$LD_LIBRARY_PATH"` to solve this. ldd -v then reports finding libRblas.so.

Talk about extremely annoying `dotnet test --filter TestGetPathInitSearchLog --blame RDotNet.Tests/RDotNet.Tests.csproj` was not reporting why this failed. I needed to use detailed verbose output `dotnet test -v d --filter TestGetPathInitSearchLog --blame RDotNet.Tests/RDotNet.Tests.csproj` to GET THE RELEVANT AND CRAFTED EXCEPTION MESSAGE. 

`[xUnit.net 00:00:00.46]       System.ArgumentException : This 64-bit process failed to load the library libR.so. No further error message from the dynamic library loader ` likely requires:

```sh
export LD_LIBRARY_PATH="/usr/local/lib/R/lib:$LD_LIBRARY_PATH"
export R_HOME="/usr/local/lib/R"
```

Yes, does the job. 


## Appendix

```json
{
    // Use IntelliSense to learn about possible attributes.
    // Hover to view descriptions of existing attributes.
    // For more information, visit: https://go.microsoft.com/fwlink/?linkid=830387
    "version": "0.2.0",
    "configurations": [
        {
            "name": ".NET Core SimpleTest",
            "type": "coreclr",
            "request": "launch",
            // "preLaunchTask": "build",
            "env": {
                "R_HOME": "/usr/lib/R"
            },
            "program": "${workspaceFolder}/TestApps/SimpleTest/bin/Debug/netcoreapp3.1/SimpleTest",
            "args": [],
            "cwd": "${workspaceFolder}",
            "stopAtEntry": false,
            "console": "internalConsole",
            // "pipeTransport": {
            //     "pipeCwd": "${workspaceFolder}",
            //     "pipeProgram": "enter the fully qualified path for the pipe program name, for example '/usr/bin/ssh'",
            //     "pipeArgs": [],
            //     "debuggerPath": "enter the path for the debugger on the target machine, for example ~/vsdbg/vsdbg"
            // }
        },
        {
            // https://code.visualstudio.com/docs/cpp/launch-json-reference
            // Not used as such but looking like a useful source: https://www.justinmklam.com/posts/2017/10/vscode-debugger-setup/
            // Maybe: https://marketplace.visualstudio.com/items?itemName=webfreak.debug
            // I seemed to manage to get a successful attach to libwaa.so even from python, 
            // but python itself keeps breaking in VSCode (only once if cli gdb is used.) and stuck wiith Unable to open epoll_wait.c file. Who cares.
            // Trying to find a way to tell GDB not to load the symbols for python program.
            // https://stackoverflow.com/questions/31763639/how-to-prevent-gdb-from-loading-debugging-symbol-for-a-large-library
            // I follow the instructions in https://developer.mozilla.org/en-US/docs/Archive/Mozilla/Using_gdb_on_wimpy_computers . May need adaptation to make it 
            // Nope. Need to use symbolLoadInfo below, but still stuck on libc6 exception
            // https://gist.github.com/asroy/ca018117e5dbbf53569b696a8c89204f
            "name": "(gdb) Debug Stresstest application",
            "type": "cppdbg",
            "request": "launch",
            "program": "${workspaceFolder}/TestApps/StressTest/bin/Debug/netcoreapp3.1/StressTest",
            "MIMode": "gdb",
            "miDebuggerPath": "gdb",
            "cwd": "${workspaceFolder}/TestApps/StressTest/bin/Debug/netcoreapp3.1/",
            "additionalSOLibSearchPath": "/usr/local/lib/R/lib",
            "symbolLoadInfo":{
                "loadAll": false,
                "exceptionList": "libR.so"
            },
            "setupCommands": [
                {
                    "description": "Enable pretty-printing for gdb",
                    "text": "-enable-pretty-printing",
                    "ignoreFailures": true
                }
            ],
            "environment": [
                {"name": "R_HOME", "value": "/usr/local/lib/R"}
            ]
        },
        {
            "name": "(gdb) Debug Repro application",
            "type": "cppdbg",
            "request": "launch",
            "program": "${workspaceFolder}/TestApps/Repro/bin/Debug/netcoreapp3.1/Repro",
            "MIMode": "gdb",
            "miDebuggerPath": "gdb",
            "cwd": "${workspaceFolder}/TestApps/Repro/bin/Debug/netcoreapp3.1/",
            "additionalSOLibSearchPath": "/usr/local/lib/R/lib",
            "symbolLoadInfo":{
                "loadAll": false,
                "exceptionList": "libR.so"
            },
            "setupCommands": [
                {
                    "description": "Enable pretty-printing for gdb",
                    "text": "-enable-pretty-printing",
                    "ignoreFailures": true
                }
            ],
            "environment": [
                {"name": "R_HOME", "value": "/usr/local/lib/R"}
            ]
        }
    ]
}
```
