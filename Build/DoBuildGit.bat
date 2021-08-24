@echo off
if "%COMPUTERNAME%" == "SQLDM7BLDVM" goto DOSTARTUP
echo. NOTE:  This script can only be run from the SQLDM7BLDVM machine
goto END

REM *******************************************
REM ********** DO STARTUP ********************
REM ******************************************
:DOSTARTUP

SET BUILDTYPE=%1
SET BUILDNUMBER=%2

SET MAVEN_OPTS="-Dhttps.protocols=TLSv1.2"

if (%BUILDNUMBER%)==() (
	echo. BUILD ERROR: Missing build number parameter
	goto END
)

if (%BUILDTYPE%)==(dev) GOTO BUILD_DEV
if (%BUILDTYPE%)==(official) GOTO BUILD_OFFICIAL
if (%BUILDTYPE%)==(incremental) GOTO BUILD_INCREMENTAL
if (%BUILDTYPE%)==(installersOnly) GOTO BUILD_INSTALLERSONLY
if (%BUILDTYPE%)==(consoleInstallersOnly) GOTO BUILD_CONSOLEINSTALLERSONLY
if (%BUILDTYPE%)==(consoleAndFull) GOTO BUILD_CONSOLEANDFULL
if (%BUILDTYPE%)==(cdImageOnly) GOTO BUILD_CDIMAGEONLY
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
if (%3)==() GOTO DOBUILD_INSTALLERSONLY_ERROR
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
if (%3)==() GOTO DOBUILD_CONSOLEINSTALLERSONLY_ERROR
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
if (%3)==() GOTO DOBUILD_CDIMAGEONLY_ERROR
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
if not defined VCINSTALLDIR call "C:\Program Files\Microsoft Visual Studio 8\Common7\Tools\vsvars32.bat"
set SQLdmBuild=C:\GitHub\SQLdm

REM **********************************
REM Nuke the entire tree except build
REM **********************************
echo.
echo. Deleting development, documentation and install directories
rd /q /s C:\GitHub\SQLdm

echo.
echo. Fetching the lastest from GitHub

"C:\Program Files\Git\bin\git.exe" clone -b 10.5 https://idera-build:ch1ck3nr0t1!@github.com/IderaInc/SQLdm.git C:\GitHub\SQLdm


REM **********************************
REM Execute the build script
REM **********************************
echo.
echo. Building the specified target.
"C:\Program Files (x86)\Nant\bin\nant.exe" -f:C:\GitHub\SQLdm\Build\SQLdmGit.build %BUILDTARGET% -D:SQLdm.buildnumber=%BUILDNUMBER% -l:C:\GitHub\sqldm\Build\build.log -logger:NAnt.Core.MailLogger
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
if not defined VCINSTALLDIR call "C:\Program Files (x86)\Microsoft Visual Studio 8\Common7\Tools\vsvars32.bat"
set SQLdmBuild=C:\GitHub\SQLdm

REM **********************************
REM Execute the build script
REM **********************************
echo.
echo. Building the specified target.
"C:\Program Files (x86)\Nant\bin\nant.exe" -f:C:\GitHub\SQLdm\Build\SQLdmGit.build %BUILDTARGET% -D:SQLdm.buildnumber=%BUILDNUMBER% -l:C:\GitHub\sqldm\Build\build.log -logger:NAnt.Core.MailLogger
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
if not defined VCINSTALLDIR call "C:\Program Files\Microsoft Visual Studio 8\Common7\Tools\vsvars32.bat"
set SQLdmBuild=C:\GitHub\SQLdm

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
if not defined VCINSTALLDIR call "C:\Program Files\Microsoft Visual Studio 8\Common7\Tools\vsvars32.bat"
set SQLdmBuild=C:\GitHub\SQLdm

REM **********************************
REM Execute the build script
REM **********************************
echo.
echo. Building the specified target.
"C:\Program Files (x86)\Nant\bin\nant.exe" -f:C:\GitHub\SQLdm\Build\SQLdmGit.build %BUILDTARGET% -D:SQLdm.BuildLabelIncrementLetter=%3 -D:SQLdm.buildnumber=%BUILDNUMBER% -l:C:\GitHub\sqldm\Build\build.log -logger:NAnt.Core.MailLogger
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
if not defined VCINSTALLDIR call "C:\Program Files (x86)\Microsoft Visual Studio 8\Common7\Tools\vsvars32.bat"
set SQLdmBuild=C:\GitHub\SQLdm

REM **********************************
REM Execute the build script
REM **********************************
echo.
echo. Building the specified target.
"C:\Program Files (x86)\Nant\bin\nant.exe" -f:C:\GitHub\SQLdm\Build\SQLdmGit.build %BUILDTARGET% -D:SQLdm.BuildLabelIncrementLetter=%3 -D:SQLdm.buildnumber=%BUILDNUMBER% -l:C:\GitHub\sqldm\Build\build.log -logger:NAnt.Core.MailLogger
GOTO END

REM ******************************************
REM ********** END OF BUILD EXECUTION ********
REM ******************************************
:END
echo. Build script execution is complete
