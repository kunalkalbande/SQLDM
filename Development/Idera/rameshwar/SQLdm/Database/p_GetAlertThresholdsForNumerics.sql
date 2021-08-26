if (object_id('[p_GetAlertThresholdsForNumerics]') is not null)
begin
drop procedure [p_GetAlertThresholdsForNumerics]
end
go

CREATE proc [dbo].[p_GetAlertThresholdsForNumerics] 
@Metric int,
@Server int = null
as
begin
--script for getting numeric metric thresholds
declare @SQLServerID int, @ThresholdEnabled bit, @InfoThresholdXML nvarchar(2048), @WarningThresholdXML nvarchar(2048), @CriticalThresholdXML nvarchar(2048)
declare @AlertThresholds table(SQLServerID int, Op nvarchar(16), InfoThreshold int, WarningThreshold int, CriticalThreshold int) 
declare @InfoThresholdValue float, @WarningThresholdValue float, @CriticalThresholdValue float
declare @InfoThresholdEnabled bit, @WarningThresholdEnabled bit, @CriticalThresholdEnabled bit
declare @xmlDoc int
declare @Now datetime
declare @Op nvarchar(16)

-- build a table containing metric threshold values for each server that 
-- has a reorg alert (Metric)
declare read_threshold_entry insensitive cursor 
for
	select 
		SQLServerID, 
		Enabled, 
		InfoThreshold,
		WarningThreshold,
		CriticalThreshold
	from 
		MetricThresholds MT (nolock)
	where Metric = @Metric and SQLServerID=isnull(@Server,SQLServerID)
--		)
for read only
set nocount on 
open read_threshold_entry 
fetch read_threshold_entry into @SQLServerID, @ThresholdEnabled, @InfoThresholdXML, @WarningThresholdXML, @CriticalThresholdXML
while @@fetch_status = 0 
begin
	--print @SQLServerID
	if @ThresholdEnabled <> 0 
	begin
		exec sp_xml_preparedocument @xmlDoc output, @InfoThresholdXML
		select 
			@Op = Op,
			@InfoThresholdValue = Value,  
			@InfoThresholdEnabled = case when UPPER(Enabled) = N'TRUE' then 1 else 0 end  
			from openxml(@xmlDoc, '/Threshold', 3)
			with (Op nvarchar(16), Value float, Enabled nvarchar(16))
		exec sp_xml_removedocument @xmlDoc
		exec sp_xml_preparedocument @xmlDoc output, @WarningThresholdXML
		select 
			@Op = Op,
			@WarningThresholdValue = Value,  
			@WarningThresholdEnabled = case when UPPER(Enabled) = N'TRUE' then 1 else 0 end  
			from openxml(@xmlDoc, '/Threshold', 3)
			with (Op nvarchar(16), Value float, Enabled nvarchar(16))
		exec sp_xml_removedocument @xmlDoc
		exec sp_xml_preparedocument @xmlDoc output, @CriticalThresholdXML
		select
			@Op = Op,
			@CriticalThresholdValue = Value,
			@CriticalThresholdEnabled = case when UPPER(Enabled) = N'TRUE' then 1 else 0 end  
			from openxml(@xmlDoc, '/Threshold', 3)
			with (Op nvarchar(16), Value float, Enabled nvarchar(16))
		exec sp_xml_removedocument @xmlDoc

		if @InfoThresholdEnabled = 0
			select @InfoThresholdValue = 6000000
		if @WarningThresholdEnabled = 0
			select @WarningThresholdValue = 6000000
		if @CriticalThresholdEnabled = 0
			select @CriticalThresholdValue = 6000000

		if (@InfoThresholdEnabled <> 0 or @WarningThresholdEnabled <> 0 or @CriticalThresholdEnabled <> 0)
			insert into @AlertThresholds (SQLServerID, Op, InfoThreshold, WarningThreshold, CriticalThreshold)
				values(@SQLServerID, @Op, @InfoThresholdValue, @WarningThresholdValue, @CriticalThresholdValue)
	end

	fetch read_threshold_entry into @SQLServerID, @ThresholdEnabled, @InfoThresholdXML, @WarningThresholdXML, @CriticalThresholdXML
end
Close read_threshold_entry 
deallocate read_threshold_entry 

select @Metric as 'MetricID', SQLServerID, Op, InfoThreshold, WarningThreshold, CriticalThreshold from @AlertThresholds
end