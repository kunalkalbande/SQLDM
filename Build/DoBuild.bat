@echo off
if "%COMPUTERNAME%" == "SQLDM7BLDVM" goto DOSTARTUP
echo. NOTE:  This script can only be run from the SQLDM5BLDVM machine
goto END

REM ******************************************
REM ********** DO STARTUP ********************
REM ******************************************
:DOSTARTUP

if (%1)==(dev) GOTO BUILD_DEV
if (%1)==(official) GOTO BUILD_OFFICIAL
if (%1)==(incremental) GOTO BUILD_INCREMENTAL
if (%1)==(installersOnly) GOTO BUILD_INSTALLERSONLY
if (%1)==(consoleInstallersOnly) GOTO BUILD_CONSOLEINSTALLERSONLY
if (%1)==(consoleAndFull) GOTO BUILD_CONSOLEANDFULL
if (%1)==(cdImageOnly) GOTO BUILD_CDIMAGEONLY
echo. BUILD ERROR: Missing or invalid command line parameter
echo.
echo. Syntax:
echo.    DOBUILD type 
echo.      type = dev, official, incremental, installersOnly or consoleAndFull
echo.
goto END

:BUILD_DEV
echo.
echo. Starting Development Build
set BUILDTARGET="Build.Dev"
goto DOBUILD

:BUILD_OFFICIAL
echo.
echo. Starting Official Build
set BUILDTARGET="Build.Official"
goto DOBUILD

:BUILD_CONSOLEANDFULL
echo.
echo. Starting ConsoleAndFull Build
set BUILDTARGET="Build.ConsoleAndTypicalInstall"
goto DOBUILD

:BUILD_INCREMENTAL
echo.
echo. Starting Incremental Build
set BUILDTARGET="Build.Incremental"
goto DOBUILD_INCREMENTAL

:BUILD_INSTALLERSONLY
echo.
echo. Starting Installers Only Build
set BUILDTARGET="Build.InstallersOnly"
if (%2)==() GOTO DOBUILD_INSTALLERSONLY_ERROR
goto DOBUILD_INSTALLERSONLY
:DOBUILD_INSTALLERSONLY_ERROR
echo.
echo. BUILD ERROR: Missing or invalid command line parameter
echo.
echo. Syntax:
echo.    DOBUILD installersOnly increment_letter
echo.      increment_letter = incremental letter for the version A, B, etc. 
echo.
goto END

:BUILD_CONSOLEINSTALLERSONLY
echo.
echo. Starting Console Installers Only Build
set BUILDTARGET="Build.ConsoleInstallOnly"
if (%2)==() GOTO DOBUILD_CONSOLEINSTALLERSONLY_ERROR
goto DOBUILD_CONSOLEINSTALLERSONLY
:DOBUILD_CONSOLEINSTALLERSONLY_ERROR
echo.
echo. BUILD ERROR: Missing or invalid command line parameter
echo.
echo. Syntax:
echo.    DOBUILD consoleInstallersOnly increment_letter
echo.      increment_letter = incremental letter for the version A, B, etc. 
echo.
goto END

:BUILD_CDIMAGEONLY
echo.
echo. Starting CD Image Only Build
set BUILDTARGET="Build.CDImageOnly"
if (%2)==() GOTO DOBUILD_CDIMAGEONLY_ERROR
goto DOBUILD_CDIMAGEONLY
:DOBUILD_CDIMAGEONLY_ERROR
echo.
echo. BUILD ERROR: Missing or invalid command line parameter
echo.
echo. Syntax:
echo.    DOBUILD cdImageOnly increment_letter
echo.      increment_letter = incremental letter for the version A, B, etc. 
echo.
goto END

REM ******************************************
REM ********** DO BUILD **********************
REM ******************************************
:DOBUILD

REM **********************************
REM Setup user environment so the 
REM script has the appropriate 
REM permissions
REM **********************************
echo.
echo. Setting up build user environment
p4 set p4port=perforce01.redhouse.hq:1666
p4 set p4user=build
p4 set p4tickets="c:\p4tickets.txt"
p4 set p4client=build_SQLDM7BLDVM
if not defined VCINSTALLDIR call "C:\Program Files\Microsoft Visual Studio 8\Common7\Tools\vsvars32.bat"
set SQLdmBuild=C:\Perforce\SQLdm\Main

REM **********************************
REM Nuke the entire tree except build
REM **********************************
echo.
echo. Deleting development, documentation and install directories
rd /q /s C:\Perforce\SQLdm\Main\Build\Output
rd /q /s C:\Perforce\SQLdm\Main\Build\Temp
rd /q /s C:\Perforce\SQLdm\Main\Development
rd /q /s C:\Perforce\SQLdm\Main\Documentation
rd /q /s C:\Perforce\SQLdm\Main\Documents
rd /q /s C:\Perforce\SQLdm\Main\Install
rd /q /s C:\Perforce\SQLdm\Main\Libraries
rd /q /s C:\Perforce\SQLdm\Main\Redist
rd /q /s C:\Perforce\SQLdm\Main\Scripts

REM **********************************
REM Make sure we have the latest build 
REM script.  Everything else should be 
REM handled within the build script
REM **********************************
echo.
echo. Fetching the latest build script
p4 sync -f //SQLdm/Main/Build/...

REM **********************************
REM Execute the build script
REM **********************************
echo.
echo. Building the specified target.
"C:\Program Files (x86)\Nant\bin\nant.exe" -f:SQLdm.build %BUILDTARGET% -l:C:\Perforce\sqldm\Main\Build\build.log -logger:NAnt.Core.MailLogger
GOTO END

REM ******************************************
REM ********** DO BUILD INCREMENTAL **********
REM ******************************************
:DOBUILD_INCREMENTAL

REM **********************************
REM Setup user environment so the 
REM script has the appropriate 
REM permissions
REM **********************************
echo.
echo. Setting up build user environment
p4 set p4port=perforce01.redhouse.hq:1666
p4 set p4user=build
p4 set p4tickets="c:\p4tickets.txt"
p4 set p4client=build_SQLDM7BLDVM
if not defined VCINSTALLDIR call "C:\Program Files\Microsoft Visual Studio 8\Common7\Tools\vsvars32.bat"
set SQLdmBuild=C:\Perforce\SQLdm\Main

REM **********************************
REM Execute the build script
REM **********************************
echo.
echo. Building the specified target.
"C:\Program Files (x86)\Nant\bin\nant.exe" -f:SQLdm.build %BUILDTARGET% -l:C:\Perforce\sqldm\Main\Build\build.log -logger:NAnt.Core.MailLogger
GOTO END

REM **********************************************
REM ********** DO BUILD INSTALLERS ONLY **********
REM **********************************************
:DOBUILD_INSTALLERSONLY

REM **********************************
REM Setup user environment so the 
REM script has the appropriate 
REM permissions
REM **********************************
echo.
echo. Setting up build user environment
p4 set p4port=perforce01.redhouse.hq:1666
p4 set p4user=build
p4 set p4tickets="c:\p4tickets.txt"
p4 set p4client=build_SQLDM7BLDVM
if not defined VCINSTALLDIR call "C:\Program Files\Microsoft Visual Studio 8\Common7\Tools\vsvars32.bat"
set SQLdmBuild=C:\Perforce\SQLdm\Main

REM **********************************************
REM ********** DO BUILD INSTALLERS ONLY **********
REM **********************************************
:DOBUILD_CONSOLEINSTALLERSONLY

REM **********************************
REM Setup user environment so the 
REM script has the appropriate 
REM permissions
REM **********************************
echo.
echo. Setting up build user environment
p4 set p4port=perforce01.redhouse.hq:1666
p4 set p4user=build
p4 set p4tickets="c:\p4tickets.txt"
p4 set p4client=build_SQLDM7BLDVM
if not defined VCINSTALLDIR call "C:\Program Files\Microsoft Visual Studio 8\Common7\Tools\vsvars32.bat"
set SQLdmBuild=C:\Perforce\SQLdm\Main

REM **********************************
REM Execute the build script
REM **********************************
echo.
echo. Building the specified target.
"C:\Program Files (x86)\Nant\bin\nant.exe" -f:SQLdm.build %BUILDTARGET% -D:SQLdm.BuildLabelIncrementLetter=%2 -l:C:\Perforce\sqldm\Main\Build\build.log -logger:NAnt.Core.MailLogger
GOTO END

REM ********************************************
REM ********** DO BUILD CD IMAGE ONLY **********
REM ********************************************
:DOBUILD_CDIMAGEONLY

REM **********************************
REM Setup user environment so the 
REM script has the appropriate 
REM permissions
REM **********************************
echo.
echo. Setting up build user environment
p4 set p4port=perforce01.redhouse.hq:1666
p4 set p4user=build
p4 set p4tickets="c:\p4tickets.txt"
p4 set p4client=build_SQLDM7BLDVM
if not defined VCINSTALLDIR call "C:\Program Files (x86)\Microsoft Visual Studio 8\Common7\Tools\vsvars32.bat"
set SQLdmBuild=C:\Perforce\SQLdm\Main

REM **********************************
REM Execute the build script
REM **********************************
echo.
echo. Building the specified target.
"C:\Program Files (x86)\Nant\bin\nant.exe" -f:SQLdm.build %BUILDTARGET% -D:SQLdm.BuildLabelIncrementLetter=%2 -l:C:\Perforce\sqldm\Main\Build\build.log -logger:NAnt.Core.MailLogger
GOTO END

REM ******************************************
REM ********** END OF BUILD EXECUTION ********
REM ******************************************
:END
echo. Build script execution is complete