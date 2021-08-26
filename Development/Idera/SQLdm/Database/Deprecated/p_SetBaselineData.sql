if (object_id('[p_SetBaselineData]') is not null)
begin
drop procedure p_SetBaselineData
end
go

create procedure [dbo].p_SetBaselineData
	@SQLServerID int,
	@StartTime datetime,
	@EndTime datetime,
	@UTCCollectionDateTime datetime,
	@BaselineXML ntext
as
begin
	declare @err int
	declare @xmlDoc int
	
	begin transaction
	
	delete from BaselineData where SQLServerID = @SQLServerID

	exec sp_xml_preparedocument @xmlDoc output, @BaselineXML

	insert into BaselineData ([SQLServerID],[UTCCollectionDateTime],[ItemID],[RowCount],[Average],[Deviation])	
	select @SQLServerID,
		   @UTCCollectionDateTime,
		   [ItemID],
		   [RowCount],
		   [Average],
	       [Deviation]
		from openxml(@xmlDoc, '//Item', 1) 
			with ([ItemID] int,
				  [RowCount] int,
				  [Average] decimal(38,9),
				  [Deviation] decimal(38,9)) 

	exec sp_xml_removedocument @xmlDoc

	select @err = @@ERROR
	if (@err = 0)
	begin 
		update MonitoredSQLServers 
			set RefRangeStartTimeUTC = @StartTime,
				RefRangeEndTimeUTC = @EndTime
		where SQLServerID = @SQLServerID
	
		select @err = @@ERROR
	end

	if @err <> 0
		rollback
	else
		commit

	return @err
end
go


