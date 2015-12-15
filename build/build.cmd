@echo off

set msbuildLocation="C:\Program Files (x86)\MSBuild\14.0\Bin\msbuild.exe"
set logFile=build.log

if not exist %msbuildLocation% (
	echo Cannot find %msbuildLocation%. Please install Visual Studio 2015 Community Edition and Windows 10 SDK
	exit 1
)

del %logFile% /F /Q

if %ERRORLEVEL% NEQ 0 (
	echo Cannot delete %logFile%.
	exit 1
)

%msbuildLocation% "build-wrapper.proj" ^
	/target:build ^
	/Property:someproperty=somevalueRelease ^
	/fl /flp:logfile=%logFile%;verbosity=normal
#	/fl /flp:logfile=%logFile%;verbosity=diagnostic

if %ERRORLEVEL% NEQ 0 (
	echo Build failed. See the build log.
	pause
	exit 1
)