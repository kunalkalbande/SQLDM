if (object_id('P_UpdateTemplateData') is not null)
	BEGIN
		DROP PROCEDURE [dbo].[P_UpdateTemplateData]
	END 
GO

create procedure [dbo].[P_UpdateTemplateData]
as
BEGIN

DECLARE @PerformanceTemplate Int;
SELECT @PerformanceTemplate  = TemplateID FROM AlertTemplateLookup WHERE Name = 'Performance Template'

DECLARE @DefaultTemplate Int;
SELECT @DefaultTemplate  = TemplateID FROM AlertTemplateLookup WHERE Name = 'Default Template'

DECLARE @AmazonRDSTemplate Int;
SELECT @AmazonRDSTemplate  = TemplateID FROM AlertTemplateLookup WHERE Name = 'Amazon RDS DBaaS Template'

DECLARE @AzureSQLTemplate Int;
SELECT @AzureSQLTemplate  = TemplateID FROM AlertTemplateLookup WHERE Name = 'Azure SQL DBaaS Template'

---START:Informational Threshold Disabled for DBaas Alerts---
UPDATE dbo.DefaultMetricThresholds
SET InfoThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="GE" Enabled="false"><Value xsi:type="xsd:int">0</Value></Threshold>'
WHERE Metric =131 And UserViewID = @DefaultTemplate

UPDATE dbo.DefaultMetricThresholds
SET InfoThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="GE" Enabled="false"><Value xsi:type="xsd:int">0</Value></Threshold>'
WHERE Metric =132 And UserViewID = @DefaultTemplate

UPDATE dbo.DefaultMetricThresholds
SET InfoThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="GE" Enabled="false"><Value xsi:type="xsd:int">0</Value></Threshold>'
WHERE Metric =133 And UserViewID = @DefaultTemplate

UPDATE dbo.DefaultMetricThresholds
SET InfoThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="GE" Enabled="false"><Value xsi:type="xsd:int">0</Value></Threshold>'
WHERE Metric =134 And UserViewID = @DefaultTemplate

UPDATE dbo.DefaultMetricThresholds
SET InfoThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="EQ" Enabled="false"><Value xsi:type="xsd:boolean">true</Value></Threshold>'
WHERE Metric =135 And UserViewID = @DefaultTemplate

UPDATE dbo.DefaultMetricThresholds
SET InfoThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="GE" Enabled="false"><Value xsi:type="xsd:int">0</Value></Threshold>'
WHERE Metric =136 And UserViewID = @DefaultTemplate

UPDATE dbo.DefaultMetricThresholds
SET InfoThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="GE" Enabled="false"><Value xsi:type="xsd:int">0</Value></Threshold>'
WHERE Metric =137 And UserViewID = @DefaultTemplate


--START 5.4.2

---START:Informational Threshold Disabled for DBaas Alerts---
UPDATE dbo.DefaultMetricThresholds
SET InfoThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="LE" Enabled="true"><Value xsi:type="xsd:int">25</Value></Threshold>'
WHERE Metric =175

UPDATE dbo.DefaultMetricThresholds
SET InfoThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="LE" Enabled="true"><Value xsi:type="xsd:int">25</Value></Threshold>'
WHERE Metric =176

UPDATE dbo.DefaultMetricThresholds
SET InfoThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="LE" Enabled="true"><Value xsi:type="xsd:int">20</Value></Threshold>'
WHERE Metric =177

UPDATE dbo.DefaultMetricThresholds
SET InfoThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="LE" Enabled="true"><Value xsi:type="xsd:int">25</Value></Threshold>'
WHERE Metric =178

UPDATE dbo.DefaultMetricThresholds
SET InfoThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="LE" Enabled="true"><Value xsi:type="xsd:int">25</Value></Threshold>'
WHERE Metric =179

UPDATE dbo.DefaultMetricThresholds
SET InfoThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="LE" Enabled="true"><Value xsi:type="xsd:int">25</Value></Threshold>'
WHERE Metric =180

UPDATE dbo.DefaultMetricThresholds
SET WarningThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="LE" Enabled="false"><Value xsi:type="xsd:int">0</Value></Threshold>',
      CriticalThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="LE" Enabled="false"><Value xsi:type="xsd:int">0</Value></Threshold>'
WHERE Metric=175 

UPDATE dbo.DefaultMetricThresholds
SET WarningThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="LE" Enabled="false"><Value xsi:type="xsd:int">0</Value></Threshold>',
      CriticalThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="LE" Enabled="false"><Value xsi:type="xsd:int">0</Value></Threshold>'
WHERE Metric=176 

UPDATE dbo.DefaultMetricThresholds
SET WarningThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="LE" Enabled="false"><Value xsi:type="xsd:int">0</Value></Threshold>',
      CriticalThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="LE" Enabled="false"><Value xsi:type="xsd:int">0</Value></Threshold>'
WHERE Metric=177

UPDATE dbo.DefaultMetricThresholds
SET WarningThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="LE" Enabled="false"><Value xsi:type="xsd:int">0</Value></Threshold>',
      CriticalThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="LE" Enabled="false"><Value xsi:type="xsd:int">0</Value></Threshold>'
WHERE Metric=178

UPDATE dbo.DefaultMetricThresholds
SET WarningThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="LE" Enabled="false"><Value xsi:type="xsd:int">0</Value></Threshold>',
      CriticalThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="LE" Enabled="false"><Value xsi:type="xsd:int">0</Value></Threshold>'
WHERE Metric=179

UPDATE dbo.DefaultMetricThresholds
SET WarningThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="LE" Enabled="false"><Value xsi:type="xsd:int">0</Value></Threshold>',
      CriticalThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="LE" Enabled="false"><Value xsi:type="xsd:int">0</Value></Threshold>'
WHERE Metric=180 

--END 5.4.2

UPDATE dbo.DefaultMetricThresholds
SET InfoThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="LE" Enabled="false"><Value xsi:type="xsd:int">1000</Value></Threshold>'
WHERE Metric =138 And UserViewID = @DefaultTemplate

UPDATE dbo.DefaultMetricThresholds
SET InfoThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="LE" Enabled="false"><Value xsi:type="xsd:int">1000</Value></Threshold>'
WHERE Metric =139 And UserViewID = @DefaultTemplate

UPDATE dbo.DefaultMetricThresholds
SET InfoThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="GE" Enabled="false"><Value xsi:type="xsd:int">1</Value></Threshold>'
WHERE Metric =140 And UserViewID = @DefaultTemplate

UPDATE dbo.DefaultMetricThresholds
SET InfoThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="GE" Enabled="false"><Value xsi:type="xsd:int">5</Value></Threshold>'
WHERE Metric =141 And UserViewID = @DefaultTemplate

UPDATE dbo.DefaultMetricThresholds
SET InfoThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="LE" Enabled="false"><Value xsi:type="xsd:int">999</Value></Threshold>'
WHERE Metric =142 And UserViewID = @DefaultTemplate

UPDATE dbo.DefaultMetricThresholds
SET InfoThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="GE" Enabled="false"><Value xsi:type="xsd:int">1</Value></Threshold>'
WHERE Metric =143 And UserViewID = @DefaultTemplate

UPDATE dbo.DefaultMetricThresholds
SET InfoThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="GE" Enabled="false"><Value xsi:type="xsd:int">1</Value></Threshold>'
WHERE Metric =144 And UserViewID = @DefaultTemplate

UPDATE dbo.DefaultMetricThresholds
SET InfoThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="LE" Enabled="false"><Value xsi:type="xsd:int">999</Value></Threshold>'
WHERE Metric =145 And UserViewID = @DefaultTemplate

---END:Informational Threshold Disabled for DBaas Alerts---

UPDATE dbo.DefaultMetricThresholds
SET WarningThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="EQ" Enabled="true"><Value xsi:type="ArrayOfAnyType"><anyType xsi:type="ServiceState">ContinuePending</anyType><anyType xsi:type="ServiceState">PausePending</anyType><anyType xsi:type="ServiceState">NotInstalled</anyType><anyType xsi:type="ServiceState">StartPending</anyType><anyType xsi:type="ServiceState">StopPending</anyType><anyType xsi:type="ServiceState">TruncatedFunctionalityAvailable</anyType></Value></Threshold>',
      CriticalThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="EQ" Enabled="true"><Value xsi:type="ArrayOfAnyType"><anyType xsi:type="ServiceState">UnableToMonitor</anyType><anyType xsi:type="ServiceState">UnableToConnect</anyType><anyType xsi:type="ServiceState">Stopped</anyType></Value></Threshold>',
	  InfoThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="EQ" Enabled="false"><Value xsi:type="ArrayOfAnyType" /></Threshold>'
WHERE Metric=10

UPDATE dbo.DefaultMetricThresholds
SET WarningThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="EQ" Enabled="true"><Value xsi:type="ArrayOfAnyType"><anyType xsi:type="ServiceState">ContinuePending</anyType><anyType xsi:type="ServiceState">NotInstalled</anyType><anyType xsi:type="ServiceState">Paused</anyType><anyType xsi:type="ServiceState">PausePending</anyType><anyType xsi:type="ServiceState">StartPending</anyType><anyType xsi:type="ServiceState">StopPending</anyType><anyType xsi:type="ServiceState">TruncatedFunctionalityAvailable</anyType></Value></Threshold>',
      CriticalThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="EQ" Enabled="true"><Value xsi:type="ArrayOfAnyType"><anyType xsi:type="ServiceState">UnableToMonitor</anyType><anyType xsi:type="ServiceState">UnableToConnect</anyType><anyType xsi:type="ServiceState">Stopped</anyType></Value></Threshold>',
	  InfoThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="EQ" Enabled="false"><Value xsi:type="ArrayOfAnyType" /></Threshold>'
WHERE Metric=12

UPDATE dbo.DefaultMetricThresholds
SET WarningThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="EQ" Enabled="true"><Value xsi:type="ArrayOfAnyType"><anyType xsi:type="ServiceState">ContinuePending</anyType><anyType xsi:type="ServiceState">PausePending</anyType><anyType xsi:type="ServiceState">NotInstalled</anyType><anyType xsi:type="ServiceState">StartPending</anyType><anyType xsi:type="ServiceState">StopPending</anyType><anyType xsi:type="ServiceState">TruncatedFunctionalityAvailable</anyType></Value></Threshold>',
      CriticalThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="EQ" Enabled="true"><Value xsi:type="ArrayOfAnyType"><anyType xsi:type="ServiceState">UnableToMonitor</anyType><anyType xsi:type="ServiceState">UnableToConnect</anyType><anyType xsi:type="ServiceState">Stopped</anyType></Value></Threshold>',
	  InfoThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="EQ" Enabled="false"><Value xsi:type="ArrayOfAnyType" /></Threshold>'
WHERE Metric=18

UPDATE dbo.DefaultMetricThresholds
SET WarningThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="EQ" Enabled="true"><Value xsi:type="ArrayOfAnyType"><anyType xsi:type="DBStatus">Loading</anyType><anyType xsi:type="DBStatus">Offline</anyType><anyType xsi:type="DBStatus">Recovering</anyType><anyType xsi:type="DBStatus">PreRecovery</anyType></Value></Threshold>',
      CriticalThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="EQ" Enabled="true"><Value xsi:type="ArrayOfAnyType"><anyType xsi:type="DBStatus">Suspect</anyType><anyType xsi:type="DBStatus">Inaccessible</anyType><anyType xsi:type="DBStatus">EmergencyMode</anyType></Value></Threshold>',
	  InfoThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="EQ" Enabled="false"><Value xsi:type="ArrayOfAnyType" /></Threshold>'
WHERE Metric=14

UPDATE dbo.DefaultMetricThresholds
SET WarningThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="EQ" Enabled="true"><Value xsi:type="ArrayOfAnyType"><anyType xsi:type="ServiceState">ContinuePending</anyType><anyType xsi:type="ServiceState">PausePending</anyType><anyType xsi:type="ServiceState">NotInstalled</anyType><anyType xsi:type="ServiceState">StartPending</anyType><anyType xsi:type="ServiceState">StopPending</anyType><anyType xsi:type="ServiceState">TruncatedFunctionalityAvailable</anyType></Value></Threshold>',
      CriticalThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="EQ" Enabled="true"><Value xsi:type="ArrayOfAnyType"><anyType xsi:type="ServiceState">UnableToMonitor</anyType><anyType xsi:type="ServiceState">UnableToConnect</anyType><anyType xsi:type="ServiceState">Stopped</anyType></Value></Threshold>',
	  InfoThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="EQ" Enabled="false"><Value xsi:type="ArrayOfAnyType" /></Threshold>'
WHERE Metric=19

UPDATE dbo.DefaultMetricThresholds
SET WarningThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="EQ" Enabled="true"><Value xsi:type="ArrayOfAnyType" /></Threshold>',
      CriticalThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="EQ" Enabled="true"><Value xsi:type="ArrayOfAnyType"><anyType xsi:type="OSMetricsStatus">Disabled</anyType><anyType xsi:type="OSMetricsStatus">OLEAutomationUnavailable</anyType><anyType xsi:type="OSMetricsStatus">UnavailableDueToLightweightPooling</anyType><anyType xsi:type="OSMetricsStatus">WMIServiceUnreachable</anyType></Value></Threshold>',
	  InfoThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="EQ" Enabled="false"><Value xsi:type="ArrayOfAnyType" /></Threshold>'
WHERE Metric=23

UPDATE dbo.DefaultMetricThresholds
SET WarningThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="EQ" Enabled="false"><Value xsi:type="OptionStatus">Enabled</Value></Threshold>',
      CriticalThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="EQ" Enabled="true"><Value xsi:type="OptionStatus">Enabled</Value></Threshold>',
	  InfoThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="EQ" Enabled="false"><Value xsi:type="OptionStatus">Enabled</Value></Threshold>'
WHERE Metric=49

UPDATE dbo.DefaultMetricThresholds
SET WarningThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="EQ" Enabled="true"><Value xsi:type="OptionStatus">Disabled</Value></Threshold>',
      CriticalThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="EQ" Enabled="false"><Value xsi:type="OptionStatus">Disabled</Value></Threshold>',
	  InfoThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="EQ" Enabled="false"><Value xsi:type="OptionStatus">Disabled</Value></Threshold>'
WHERE Metric=50

UPDATE dbo.DefaultMetricThresholds
SET WarningThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="EQ" Enabled="true"><Value xsi:type="OptionStatus">Disabled</Value></Threshold>',
      CriticalThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="EQ" Enabled="false"><Value xsi:type="OptionStatus">Disabled</Value></Threshold>',
	  InfoThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="EQ" Enabled="false"><Value xsi:type="OptionStatus">Disabled</Value></Threshold>'
WHERE Metric=53


UPDATE dbo.DefaultMetricThresholds
SET WarningThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="EQ" Enabled="true"><Value xsi:type="OptionStatus">Disabled</Value></Threshold>',
      CriticalThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="EQ" Enabled="false"><Value xsi:type="OptionStatus">Disabled</Value></Threshold>',
	  InfoThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="EQ" Enabled="false"><Value xsi:type="OptionStatus">Disabled</Value></Threshold>'
WHERE Metric=54


UPDATE dbo.DefaultMetricThresholds
SET WarningThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="EQ" Enabled="true"><Value xsi:type="ArrayOfAnyType"><anyType xsi:type="MirroringStateEnum">Suspended</anyType></Value></Threshold>',
      CriticalThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="EQ" Enabled="true"><Value xsi:type="ArrayOfAnyType"><anyType xsi:type="MirroringStateEnum">Disconnected</anyType><anyType xsi:type="MirroringStateEnum">PendingFailover</anyType></Value></Threshold>',
	  InfoThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="EQ" Enabled="false"><Value xsi:type="ArrayOfAnyType" /></Threshold>'
WHERE Metric=72


UPDATE dbo.DefaultMetricThresholds
SET WarningThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="EQ" Enabled="true"><Value xsi:type="ArrayOfAnyType"><anyType xsi:type="JobStepCompletionStatus">Retry</anyType><anyType xsi:type="JobStepCompletionStatus">Cancelled</anyType></Value></Threshold>',
      CriticalThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="EQ" Enabled="true"><Value xsi:type="ArrayOfAnyType"><anyType xsi:type="JobStepCompletionStatus">Failed</anyType><anyType xsi:type="JobStepCompletionStatus">Unknown</anyType></Value></Threshold>',
	  InfoThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="EQ" Enabled="false"><Value xsi:type="ArrayOfAnyType" /></Threshold>'
WHERE Metric=88

UPDATE dbo.DefaultMetricThresholds
SET WarningThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="EQ" Enabled="true"><Value xsi:type="xsd:boolean">true</Value></Threshold>',
      CriticalThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="EQ" Enabled="false"><Value xsi:type="xsd:boolean">true</Value></Threshold>',
	  InfoThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="EQ" Enabled="false"><Value xsi:type="xsd:boolean">true</Value></Threshold>'
WHERE Metric=94


UPDATE dbo.DefaultMetricThresholds
SET WarningThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="EQ" Enabled="true"><Value xsi:type="xsd:boolean">true</Value></Threshold>',
      CriticalThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="EQ" Enabled="false"><Value xsi:type="xsd:boolean">true</Value></Threshold>',
	  InfoThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="EQ" Enabled="false"><Value xsi:type="xsd:boolean">true</Value></Threshold>'
WHERE Metric=95


UPDATE dbo.DefaultMetricThresholds
SET WarningThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="EQ" Enabled="false"><Value xsi:type="xsd:boolean">true</Value></Threshold>',
      CriticalThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="EQ" Enabled="false"><Value xsi:type="xsd:boolean">true</Value></Threshold>',
	  InfoThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="EQ" Enabled="true"><Value xsi:type="xsd:boolean">true</Value></Threshold>'
WHERE Metric=96



UPDATE dbo.DefaultMetricThresholds
SET WarningThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="EQ" Enabled="true"><Value xsi:type="xsd:boolean">true</Value></Threshold>',
      CriticalThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="EQ" Enabled="false"><Value xsi:type="xsd:boolean">true</Value></Threshold>',
	  InfoThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="EQ" Enabled="false"><Value xsi:type="xsd:boolean">true</Value></Threshold>'
WHERE Metric=97



UPDATE dbo.DefaultMetricThresholds
SET WarningThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="EQ" Enabled="true"><Value xsi:type="xsd:boolean">true</Value></Threshold>',
      CriticalThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="EQ" Enabled="false"><Value xsi:type="xsd:boolean">true</Value></Threshold>',
	  InfoThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="EQ" Enabled="false"><Value xsi:type="xsd:boolean">true</Value></Threshold>'
WHERE Metric=104


UPDATE dbo.DefaultMetricThresholds
SET WarningThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="EQ" Enabled="true"><Value xsi:type="xsd:boolean">true</Value></Threshold>',
      CriticalThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="EQ" Enabled="false"><Value xsi:type="xsd:boolean">true</Value></Threshold>',
	  InfoThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="EQ" Enabled="false"><Value xsi:type="xsd:boolean">true</Value></Threshold>'
WHERE Metric=105

UPDATE dbo.DefaultMetricThresholds
SET WarningThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="EQ" Enabled="false"><Value xsi:type="xsd:boolean">true</Value></Threshold>',
      CriticalThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="EQ" Enabled="false"><Value xsi:type="xsd:boolean">true</Value></Threshold>',
	  InfoThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="EQ" Enabled="true"><Value xsi:type="xsd:boolean">true</Value></Threshold>'
WHERE Metric=106


UPDATE dbo.DefaultMetricThresholds
SET WarningThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="EQ" Enabled="false"><Value xsi:type="ArrayOfAnyType" /></Threshold>',
      CriticalThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="EQ" Enabled="true"><Value xsi:type="ArrayOfAnyType"><anyType xmlns:q1="urn:vim25" xsi:type="q1:VirtualMachinePowerState">suspended</anyType><anyType xmlns:q2="urn:vim25" xsi:type="q2:VirtualMachinePowerState">poweredOff</anyType></Value></Threshold>',
	  InfoThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="EQ" Enabled="false"><Value xsi:type="ArrayOfAnyType" /></Threshold>'
WHERE Metric=107


UPDATE dbo.DefaultMetricThresholds
SET WarningThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="EQ" Enabled="true"><Value xsi:type="ArrayOfAnyType"><anyType xmlns:q1="urn:vim25" xsi:type="q1:HostSystemPowerState">unknown</anyType></Value></Threshold>',
      CriticalThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="EQ" Enabled="true"><Value xsi:type="ArrayOfAnyType"><anyType xmlns:q1="urn:vim25" xsi:type="q1:HostSystemPowerState">poweredOff</anyType><anyType xmlns:q2="urn:vim25" xsi:type="q2:HostSystemPowerState">standBy</anyType></Value></Threshold>',
	  InfoThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="EQ" Enabled="false"><Value xsi:type="ArrayOfAnyType" /></Threshold>'
WHERE Metric=108


UPDATE dbo.DefaultMetricThresholds
SET WarningThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="EQ" Enabled="false"><Value xsi:type="xsd:boolean">true</Value></Threshold>',
      CriticalThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="EQ" Enabled="true"><Value xsi:type="xsd:boolean">true</Value></Threshold>',
	  InfoThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="EQ" Enabled="false"><Value xsi:type="xsd:boolean">true</Value></Threshold>'
WHERE Metric=118


UPDATE dbo.DefaultMetricThresholds
SET WarningThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="EQ" Enabled="true"><Value xsi:type="ArrayOfAnyType"><anyType xsi:type="ServiceState">ContinuePending</anyType><anyType xsi:type="ServiceState">NotInstalled</anyType><anyType xsi:type="ServiceState">Paused</anyType><anyType xsi:type="ServiceState">PausePending</anyType><anyType xsi:type="ServiceState">StartPending</anyType><anyType xsi:type="ServiceState">StopPending</anyType><anyType xsi:type="ServiceState">TruncatedFunctionalityAvailable</anyType></Value></Threshold>',
      CriticalThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="EQ" Enabled="true"><Value xsi:type="ArrayOfAnyType"><anyType xsi:type="ServiceState">UnableToMonitor</anyType><anyType xsi:type="ServiceState">UnableToConnect</anyType><anyType xsi:type="ServiceState">Stopped</anyType></Value></Threshold>',
	  InfoThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="EQ" Enabled="false"><Value xsi:type="ArrayOfAnyType" /></Threshold>'
WHERE Metric=128


UPDATE dbo.DefaultMetricThresholds
SET WarningThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="EQ" Enabled="true"><Value xsi:type="ArrayOfAnyType"><anyType xsi:type="ServiceState">ContinuePending</anyType><anyType xsi:type="ServiceState">NotInstalled</anyType><anyType xsi:type="ServiceState">Paused</anyType><anyType xsi:type="ServiceState">PausePending</anyType><anyType xsi:type="ServiceState">StartPending</anyType><anyType xsi:type="ServiceState">StopPending</anyType><anyType xsi:type="ServiceState">TruncatedFunctionalityAvailable</anyType></Value></Threshold>',
      CriticalThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="EQ" Enabled="true"><Value xsi:type="ArrayOfAnyType"><anyType xsi:type="ServiceState">UnableToMonitor</anyType><anyType xsi:type="ServiceState">UnableToConnect</anyType><anyType xsi:type="ServiceState">Stopped</anyType></Value></Threshold>',
	  InfoThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="EQ" Enabled="false"><Value xsi:type="ArrayOfAnyType" /></Threshold>'
WHERE Metric=129


UPDATE dbo.DefaultMetricThresholds
SET WarningThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="GE" Enabled="true"><Value xsi:type="xsd:int">1</Value></Threshold>',
      CriticalThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="GE" Enabled="false"><Value xsi:type="xsd:int">1</Value></Threshold>',
	  InfoThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="GE" Enabled="false"><Value xsi:type="xsd:int">1</Value></Threshold>'
WHERE Metric=51

UPDATE dbo.DefaultMetricThresholds
SET WarningThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="GE" Enabled="true"><Value xsi:type="xsd:int">5</Value></Threshold>',
      CriticalThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="GE" Enabled="false"><Value xsi:type="xsd:int">15</Value></Threshold>',
	  InfoThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="GE" Enabled="false"><Value xsi:type="xsd:int">1</Value></Threshold>'
WHERE Metric=51 AND UserViewID = @PerformanceTemplate

UPDATE dbo.DefaultMetricThresholds
SET WarningThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="GE" Enabled="true"><Value xsi:type="xsd:int">10</Value></Threshold>',
      CriticalThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="GE" Enabled="false"><Value xsi:type="xsd:int">20</Value></Threshold>',
	  InfoThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="GE" Enabled="false"><Value xsi:type="xsd:int">5</Value></Threshold>'
WHERE Metric=71 AND UserViewID <> @PerformanceTemplate AND UserViewID <> @DefaultTemplate

UPDATE dbo.DefaultMetricThresholds
SET WarningThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="GE" Enabled="true"><Value xsi:type="xsd:int">20</Value></Threshold>',
      CriticalThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="GE" Enabled="false"><Value xsi:type="xsd:int">30</Value></Threshold>',
	  InfoThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="GE" Enabled="false"><Value xsi:type="xsd:int">10</Value></Threshold>'
WHERE Metric=71 AND UserViewID = @PerformanceTemplate

UPDATE dbo.DefaultMetricThresholds
SET WarningThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="GE" Enabled="true"><Value xsi:type="xsd:int">1</Value></Threshold>',
      CriticalThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="GE" Enabled="false"><Value xsi:type="xsd:int">1</Value></Threshold>',
	  InfoThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="GE" Enabled="false"><Value xsi:type="xsd:int">1</Value></Threshold>'
WHERE Metric=73


UPDATE dbo.DefaultMetricThresholds
SET WarningThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="GE" Enabled="true"><Value xsi:type="xsd:int">1</Value></Threshold>',
      CriticalThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="GE" Enabled="false"><Value xsi:type="xsd:int">1</Value></Threshold>',
	  InfoThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="GE" Enabled="false"><Value xsi:type="xsd:int">1</Value></Threshold>'
WHERE Metric=74


UPDATE dbo.DefaultMetricThresholds
SET WarningThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="GE" Enabled="false"><Value xsi:type="xsd:int">1</Value></Threshold>',
      CriticalThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="GE" Enabled="true"><Value xsi:type="xsd:int">1</Value></Threshold>',
	  InfoThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="GE" Enabled="false"><Value xsi:type="xsd:int">1</Value></Threshold>'
WHERE Metric=75


UPDATE dbo.DefaultMetricThresholds
SET WarningThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="GE" Enabled="false"><Value xsi:type="xsd:int">1</Value></Threshold>',
      CriticalThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="GE" Enabled="true"><Value xsi:type="xsd:int">1</Value></Threshold>',
	  InfoThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="GE" Enabled="false"><Value xsi:type="xsd:int">1</Value></Threshold>'
WHERE Metric=77


UPDATE dbo.DefaultMetricThresholds
SET WarningThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="GE" Enabled="false"><Value xsi:type="xsd:int">1</Value></Threshold>',
      CriticalThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="GE" Enabled="true"><Value xsi:type="xsd:int">1</Value></Threshold>',
	  InfoThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="GE" Enabled="false"><Value xsi:type="xsd:int">1</Value></Threshold>'
WHERE Metric=78


UPDATE dbo.DefaultMetricThresholds
SET WarningThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="EQ" Enabled="false"><Value xsi:type="xsd:boolean">true</Value></Threshold>',
      CriticalThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="EQ" Enabled="true"><Value xsi:type="xsd:boolean">true</Value></Threshold>',
	  InfoThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="EQ" Enabled="false"><Value xsi:type="xsd:boolean">true</Value></Threshold>'
WHERE Metric=116


UPDATE dbo.DefaultMetricThresholds
SET WarningThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="EQ" Enabled="false"><Value xsi:type="xsd:int">1</Value></Threshold>',
      CriticalThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="EQ" Enabled="true"><Value xsi:type="xsd:int">1</Value></Threshold>',
	  InfoThreshold = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="EQ" Enabled="false"><Value xsi:type="xsd:int">1</Value></Threshold>'
WHERE Metric=124


UPDATE dbo.DefaultMetricThresholds 
SET WarningThreshold='<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="EQ" Enabled="false"><Value xsi:type="xsd:int">1</Value></Threshold>',
       CriticalThreshold='<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="EQ" Enabled="true"><Value xsi:type="xsd:int">1</Value></Threshold>',
	   InfoThreshold='<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="EQ" Enabled="false"><Value xsi:type="xsd:int">1</Value></Threshold>'
WHERE Metric=125

UPDATE dbo.DefaultMetricThresholds
SET WarningThreshold='<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="GE" Enabled="true"><Value xsi:type="xsd:int">10</Value></Threshold>',
       CriticalThreshold='<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="GE" Enabled="true"><Value xsi:type="xsd:int">30</Value></Threshold>',
	   InfoThreshold='<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="GE" Enabled="false"><Value xsi:type="xsd:int">5</Value></Threshold>'
WHERE Metric=130
     

UPDATE dbo.DefaultMetricThresholds
SET WarningThreshold='<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="EQ" Enabled="false"><Value xsi:type="xsd:boolean">true</Value></Threshold>',
       CriticalThreshold='<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="EQ" Enabled="true"><Value xsi:type="xsd:boolean">true</Value></Threshold>',
	   InfoThreshold='<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="EQ" Enabled="false"><Value xsi:type="xsd:boolean">true</Value></Threshold>'
WHERE Metric=135
     

UPDATE dbo.DefaultMetricThresholds
SET WarningThreshold='<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="GE" Enabled="true"><Value xsi:type="xsd:int">500</Value></Threshold>',
       CriticalThreshold='<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="GE" Enabled="true"><Value xsi:type="xsd:int">1000</Value></Threshold>',
	   InfoThreshold='<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="GE" Enabled="false"><Value xsi:type="xsd:int">100</Value></Threshold>'
WHERE Metric=110

UPDATE dbo.DefaultMetricThresholds
SET WarningThreshold='<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="LE" Enabled="true"><Value xsi:type="xsd:int">500</Value></Threshold>',
       CriticalThreshold='<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="LE" Enabled="true"><Value xsi:type="xsd:int">100</Value></Threshold>',
	   InfoThreshold='<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="LE" Enabled="false"><Value xsi:type="xsd:int">1000</Value></Threshold>'
WHERE Metric=111


UPDATE dbo.DefaultMetricThresholds
SET WarningThreshold='<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="GE" Enabled="true"><Value xsi:type="xsd:int">900</Value></Threshold>',
       CriticalThreshold='<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="GE" Enabled="true"><Value xsi:type="xsd:int">1800</Value></Threshold>',
	   InfoThreshold='<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="GE" Enabled="false"><Value xsi:type="xsd:int">600</Value></Threshold>'
WHERE Metric=117


UPDATE dbo.DefaultMetricThresholds
SET WarningThreshold='<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="GE" Enabled="true"><Value xsi:type="xsd:int">900</Value></Threshold>',
       CriticalThreshold='<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="GE" Enabled="true"><Value xsi:type="xsd:int">1800</Value></Threshold>',
	   InfoThreshold='<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="GE" Enabled="false"><Value xsi:type="xsd:int">600</Value></Threshold>'
WHERE Metric=119


UPDATE dbo.DefaultMetricThresholds
SET WarningThreshold='<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="GE" Enabled="true"><Value xsi:type="xsd:int">900</Value></Threshold>',
       CriticalThreshold='<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="GE" Enabled="true"><Value xsi:type="xsd:int">1800</Value></Threshold>',
	   InfoThreshold='<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="GE" Enabled="false"><Value xsi:type="xsd:int">600</Value></Threshold>'
WHERE Metric=120


UPDATE dbo.DefaultMetricThresholds
SET WarningThreshold='<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="GE" Enabled="false"><Value xsi:type="xsd:int">1</Value></Threshold>',
       CriticalThreshold='<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="GE" Enabled="true"><Value xsi:type="xsd:int">1</Value></Threshold>',
	   InfoThreshold='<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="GE" Enabled="false"><Value xsi:type="xsd:int">1</Value></Threshold>'
WHERE Metric=11

UPDATE dbo.DefaultMetricThresholds
SET WarningThreshold='<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="GE" Enabled="false"><Value xsi:type="xsd:int">1</Value></Threshold>',
       CriticalThreshold='<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="GE" Enabled="true"><Value xsi:type="xsd:int">1</Value></Threshold>',
	   InfoThreshold='<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="GE" Enabled="false"><Value xsi:type="xsd:int">1</Value></Threshold>'
WHERE Metric=80

UPDATE dbo.DefaultMetricThresholds
SET WarningThreshold='<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="GE" Enabled="true"><Value xsi:type="xsd:int">1</Value></Threshold>',
       CriticalThreshold='<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="GE" Enabled="false"><Value xsi:type="xsd:int">1</Value></Threshold>',
	   InfoThreshold='<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Op="GE" Enabled="false"><Value xsi:type="xsd:int">1</Value></Threshold>'
WHERE Metric=35


UPDATE dbo.DefaultMetricThresholds
SET Enabled=0
WHERE Metric=27 AND UserViewID = @DefaultTemplate

UPDATE dbo.DefaultMetricThresholds
SET Enabled=0
WHERE Metric=28 AND UserViewID = @DefaultTemplate

UPDATE dbo.DefaultMetricThresholds
SET Enabled=0
WHERE Metric=88 AND UserViewID = @DefaultTemplate

UPDATE dbo.DefaultMetricThresholds
SET Enabled=0
WHERE Metric=130 AND UserViewID=@AmazonRDSTemplate

UPDATE dbo.DefaultMetricThresholds
SET Enabled=0
WHERE Metric=130 AND UserViewID=@AzureSQLTemplate


Delete from DefaultMetricThresholds
WHERE Metric=56


Delete from DefaultMetricThresholds
WHERE Metric=48

-- Update Baseline Alerts for All template
UPDATE DefaultMetricThresholds SET IsBaselineEnabled = 1 WHERE Metric = 146
UPDATE DefaultMetricThresholds SET IsBaselineEnabled = 1 WHERE Metric = 147
UPDATE DefaultMetricThresholds SET IsBaselineEnabled = 1 WHERE Metric = 148
UPDATE DefaultMetricThresholds SET IsBaselineEnabled = 1 WHERE Metric = 149
UPDATE DefaultMetricThresholds SET IsBaselineEnabled = 1 WHERE Metric = 150
UPDATE DefaultMetricThresholds SET IsBaselineEnabled = 1 WHERE Metric = 151
UPDATE DefaultMetricThresholds SET IsBaselineEnabled = 1 WHERE Metric = 152
UPDATE DefaultMetricThresholds SET IsBaselineEnabled = 1 WHERE Metric = 153
UPDATE DefaultMetricThresholds SET IsBaselineEnabled = 1 WHERE Metric = 154
UPDATE DefaultMetricThresholds SET IsBaselineEnabled = 1 WHERE Metric = 155
UPDATE DefaultMetricThresholds SET IsBaselineEnabled = 1 WHERE Metric = 156
UPDATE DefaultMetricThresholds SET IsBaselineEnabled = 1 WHERE Metric = 157
UPDATE DefaultMetricThresholds SET IsBaselineEnabled = 1 WHERE Metric = 158
UPDATE DefaultMetricThresholds SET IsBaselineEnabled = 1 WHERE Metric = 159
UPDATE DefaultMetricThresholds SET IsBaselineEnabled = 1 WHERE Metric = 160
UPDATE DefaultMetricThresholds SET IsBaselineEnabled = 1 WHERE Metric = 161
UPDATE DefaultMetricThresholds SET IsBaselineEnabled = 1 WHERE Metric = 162
UPDATE DefaultMetricThresholds SET IsBaselineEnabled = 1 WHERE Metric = 163
UPDATE DefaultMetricThresholds SET IsBaselineEnabled = 1 WHERE Metric = 164
UPDATE DefaultMetricThresholds SET IsBaselineEnabled = 1 WHERE Metric = 165
UPDATE DefaultMetricThresholds SET IsBaselineEnabled = 1 WHERE Metric = 166
UPDATE DefaultMetricThresholds SET IsBaselineEnabled = 1 WHERE Metric = 167
UPDATE DefaultMetricThresholds SET IsBaselineEnabled = 1 WHERE Metric = 168
UPDATE DefaultMetricThresholds SET IsBaselineEnabled = 1 WHERE Metric = 169
UPDATE DefaultMetricThresholds SET IsBaselineEnabled = 1 WHERE Metric = 170
UPDATE DefaultMetricThresholds SET IsBaselineEnabled = 1 WHERE Metric = 171
UPDATE DefaultMetricThresholds SET IsBaselineEnabled = 1 WHERE Metric = 172
UPDATE DefaultMetricThresholds SET IsBaselineEnabled = 1 WHERE Metric = 173
UPDATE DefaultMetricThresholds SET IsBaselineEnabled = 1 WHERE Metric = 174
--
END

GO