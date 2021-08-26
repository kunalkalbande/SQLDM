@echo off

if "%PROCESSOR_ARCHITECTURE%" == "x86" (
	copy /y ..\\..\\..\\..\\Libraries\manage.dll ..\\..\\..\\bin\\debug\\manage.dll
) else (
	copy /y ..\\..\\..\\..\\Libraries\manage_x64.dll ..\\..\\..\\bin\\debug\\manage.dll
)