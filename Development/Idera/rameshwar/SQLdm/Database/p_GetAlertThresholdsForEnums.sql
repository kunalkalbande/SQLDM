if (object_id('[p_GetAlertThresholdsForEnums]') is not null)
begin
drop procedure [p_GetAlertThresholdsForEnums]
end
go

CREATE proc [dbo].[p_GetAlertThresholdsForEnums] 
@Metric int,
@Server int = null
as
begin
--script for getting enum metric thresholds
declare @SQLServerID int, @ThresholdEnabled bit, @InfoThresholdXML nvarchar(2048), @WarningThresholdXML nvarchar(2048), @CriticalThresholdXML nvarchar(2048)
declare @AlertThresholds table(SQLServerID int, InfoThreshold nvarchar(2048), WarningThreshold nvarchar(2048), CriticalThreshold nvarchar(2048)) 
declare @InfoThresholdValue nvarchar(2048), @WarningThresholdValue nvarchar(2048), @CriticalThresholdValue nvarchar(2048)
declare @InfoThresholdEnabled bit, @WarningThresholdEnabled bit, @CriticalThresholdEnabled bit
declare @xmlDoc int
declare @Now datetime

-- build a table containing metric threshold values for each server
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
			@InfoThresholdValue = Value,  
			@InfoThresholdEnabled = case when UPPER(Enabled) = N'TRUE' then 1 else 0 end  
			from openxml(@xmlDoc, '/Threshold', 3)
			with (Value nvarchar(2048), Enabled nvarchar(16))
		exec sp_xml_removedocument @xmlDoc
		exec sp_xml_preparedocument @xmlDoc output, @WarningThresholdXML
		select 
			@WarningThresholdValue = Value,  
			@WarningThresholdEnabled = case when UPPER(Enabled) = N'TRUE' then 1 else 0 end  
			from openxml(@xmlDoc, '/Threshold', 3)
			with (Value nvarchar(2048), Enabled nvarchar(16))
		exec sp_xml_removedocument @xmlDoc
		exec sp_xml_preparedocument @xmlDoc output, @CriticalThresholdXML
		select 
			@CriticalThresholdValue = Value,
			@CriticalThresholdEnabled = case when UPPER(Enabled) = N'TRUE' then 1 else 0 end  
			from openxml(@xmlDoc, '/Threshold', 3)
			with (Value nvarchar(2048), Enabled nvarchar(16))
		exec sp_xml_removedocument @xmlDoc

		if @InfoThresholdEnabled = 0
			select @InfoThresholdValue = ''
		if @WarningThresholdEnabled = 0
			select @WarningThresholdValue = ''
		if @CriticalThresholdEnabled = 0
			select @CriticalThresholdValue = ''

		if (@InfoThresholdEnabled <> 0 or @WarningThresholdEnabled <> 0 or @CriticalThresholdEnabled <> 0)
			insert into @AlertThresholds (SQLServerID,InfoThreshold,WarningThreshold,CriticalThreshold)
				values(@SQLServerID, @InfoThresholdValue, @WarningThresholdValue, @CriticalThresholdValue)
	end

	fetch read_threshold_entry into @SQLServerID, @ThresholdEnabled, @InfoThresholdXML, @WarningThresholdXML, @CriticalThresholdXML
end
Close read_threshold_entry 
deallocate read_threshold_entry 

select @Metric as 'MetricID',SQLServerID,InfoThreshold,WarningThreshold,CriticalThreshold  from @AlertThresholds

end