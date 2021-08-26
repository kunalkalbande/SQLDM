--p_PopulateMetricThresholds 92
IF (OBJECT_ID('p_PopulateMetricThresholds') IS NOT NULL)
BEGIN
DROP PROCEDURE p_PopulateMetricThresholds
END
GO
CREATE PROCEDURE [dbo].p_PopulateMetricThresholds
	  @MetricID BIGINT
AS
BEGIN
	--DECLARE @retVal INT
	--		,@WarningThresholdXML NVARCHAR(MAX)
	--		,@WarningThresholdXMLRef INT
	--		,@CriticalThresholdXML NVARCHAR(MAX)
	--		,@CriticalThresholdXMLRef INT
	--		,@InfoThresholdXML NVARCHAR(MAX)
	--		,@InfoThresholdXMLRef INT
	--		;
			
	SELECT SQLServerID, Metric, CONVERT(XML,WarningThreshold).value('(/Threshold/Value)[1]','numeric') WarningThreshold,CONVERT(XML,CriticalThreshold).value('(/Threshold/Value)[1]','numeric') CriticalThreshold,CONVERT(XML,InfoThreshold).value('(/Threshold/Value)[1]','numeric') InfoThreshold   FROM MetricThresholds WHERE Metric = @MetricID;
	--IF(@WarningThresholdXML !='' AND @CriticalThresholdXML !='' AND @InfoThresholdXML !='') BEGIN
	--	--Putting xml in sql cache
	--	EXEC sp_xml_preparedocument @WarningThresholdXMLRef OUTPUT, @WarningThresholdXML;
	--	EXEC sp_xml_preparedocument @CriticalThresholdXMLRef OUTPUT, @CriticalThresholdXML;
	--	EXEC sp_xml_preparedocument @InfoThresholdXMLRef OUTPUT, @InfoThresholdXML;
		
	--	SELECT W.MetricID,W.WarningThreshold,C.CriticalThreshold, I.InfoThreshold, W.StateNum WarningState, C.StateNum CriticalState,I.StateNum InfoState
	--	FROM
	--	(SELECT @MetricID MetricID, Value WarningThreshold,@WarningState StateNum FROM OPENXML(@WarningThresholdXMLRef,'/Threshold',2) WITH(Value NUMERIC)) W
	--	 JOIN 
	--	(SELECT @MetricID MetricID, Value CriticalThreshold, @CriticalState StateNum FROM OPENXML(@CriticalThresholdXMLRef,'/Threshold',2) WITH(Value NUMERIC)) C
	--	ON (W.MetricID = C.MetricID)
	--	 JOIN 
	--	(SELECT  @MetricID MetricID, Value InfoThreshold, @InfoState StateNum FROM OPENXML(@InfoThresholdXMLRef,'/Threshold',2) WITH(Value NUMERIC)) I
	--	ON (C.MetricID = I.MetricID)
	--	--Removing xml from sql cache
	--	EXEC sp_xml_removedocument @WarningThresholdXMLRef;
	--	EXEC sp_xml_removedocument @CriticalThresholdXMLRef;
	--	EXEC sp_xml_removedocument @InfoThresholdXMLRef;
	--END
	
END
