@echo off
set exit_code=0

dotnet run --project tests/minimal/minimal.csproj

@if errorlevel 0 goto SimpleTest
echo "WARN: errorlevel for minimal is %errorlevel%, not 0"

@IF '%errorlevel%'=='-1073740791' set exit_code=%errorlevel%
@IF '%errorlevel%'=='-1073740791' goto MinimalFailedBufOvrrun

:SimpleTest
dotnet run --project TestApps/SimpleTest/SimpleTest.csproj 

@if errorlevel 0 goto all_exit
@echo "WARN: errorlevel for SimpleTest is %errorlevel%, not 0"

@IF '%errorlevel%'=='-1073740791' set exit_code=%errorlevel%
@IF '%errorlevel%'=='-1073740791' goto SimpleTestFailedBufOvrrun

@if errorlevel 0 goto all_exit


:MinimalFailedBufOvrrun
@echo "ERROR: exit code -1073740791 (0xC0000409) on minimal.csproj. It means STATUS_STACK_BUFFER_OVERRUN. Shows up running netcoreapp3.1 with .NETCore on Appveyor infrastructure at 2020-12. Not reproducible on local PC."
goto all_exit

:SimpleTestFailedBufOvrrun
@echo "ERROR: exit code -1073740791 (0xC0000409). It means STATUS_STACK_BUFFER_OVERRUN. Shows up running netcoreapp3.1 with .NETCore on Appveyor infrastructure at 2020-12. Not reproducible on local PC."

:all_exit
@exit /b %exit_code%
