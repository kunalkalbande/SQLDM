---------- SetMonitoredSQLServerOleAutomationOutOfProcess.sql
-- 
-- Copyright Idera, Inc. 2005-2009
-- All Rights Reserved.
-- 
-- SQLdm uses Ole Automation to collect operating system statistics.  This
-- script allows you to configure SQLdm to run these Ole Automation queries 
-- out of process.
-- 
-- Run this script on your SQLdm Repository SQL Server instance.   Before running
-- this script:
--    1. Change SQLdmRepository to your SQLdm repository database name.
--    2. Change 'MONITORED_SQL_SERVER' to the name of the monitored SQL Server 
--       for which you want SQLdm to run Ole Automation queries out of process.
-- 

USE [SQLdmRepository]

UPDATE [MonitoredSQLServers]
SET [OutOfProcOleAutomation] = 1
WHERE [InstanceName] = 'MONITORED_SQL_SERVER'