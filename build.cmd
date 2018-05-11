@echo off

REM .paket\paket.bootstrapper.exe is not used with .NET core. paket needs to be used in transparent mode.
REM if not exist .paket\paket.bootstrapper.exe goto pktbootnotfound
REM .paket\paket.bootstrapper.exe
REM if not errorlevel 0 goto pktbootfailed

.paket\paket.exe restore
if not errorlevel 0 goto pktrestorefailed

packages\FAKE\tools\FAKE.exe %* --fsiargs -d:MONO build.fsx 
REM packages\FAKE\tools\FAKE.exe Test --fsiargs -d:MONO build.fsx 
if not errorlevel 0 goto fakefailed

REM ".\packages\NuGet.CommandLine\tools\NuGet.exe" push bin\R.NET.Community.1.6.2.nupkg

set exit_code=0
goto leave

:pktbootnotfound
echo command not found: .paket\paket.bootstrapper.exe
set exit_code=1
goto leave

:pktbootfailed
echo command failed: .paket\paket.bootstrapper.exe
set exit_code=1
goto leave

:pktrestorefailed
echo command failed: .paket\paket.exe restore
set exit_code=1
goto leave

:fakefailed
echo command failed: packages\FAKE\tools\FAKE.exe %* --fsiargs -d:MONO build.fsx 
set exit_code=1
goto leave

:leave
exit /b %exit_code%