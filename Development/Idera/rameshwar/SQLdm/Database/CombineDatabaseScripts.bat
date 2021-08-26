@echo off

rem --------------------------------------------------
rem -- SQLdm Repository Script Generator
rem --------------------------------------------------

echo. > SQLdmDatabaseInstall.sql
for %%b in (DatabaseCreate.sql, fn_*.sql, SQL_Signature.sql, p_*.sql, DatabaseLoad.sql) do if not exist %%b (call :MISSINGFILE %%b&goto :eof) else (call :APPEND %%b)
goto :eof

:APPEND
find /V /I "DEBUGONLY" %* >> SQLdmDatabaseInstall.sql    
echo. >> SQLdmDatabaseInstall.sql
echo GO >> SQLdmDatabaseInstall.sql
goto :eof

:MISSINGFILE
echo The required file [%*] could not be found
echo. The required file [%*] could not be found > SQLdmDatabaseInstall.sql  


