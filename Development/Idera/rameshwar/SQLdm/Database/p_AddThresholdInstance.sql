
if (object_id('p_AddThresholdInstance') is not null)
begin
drop procedure p_AddThresholdInstance
end
go

CREATE PROCEDURE [dbo].p_AddThresholdInstance(
	@InstanceType int,
	@InstanceName nvarchar(256) = null,
	@InstanceID int out
)
AS
BEGIN
	IF (@InstanceName is null OR @InstanceName = '')
	BEGIN
		select @InstanceID = -1
		RETURN 
	END
	
	if (not exists(select InstanceID from MetricThresholdInstances where InstanceType = @InstanceType and ThresholdInstanceName = @InstanceName))
	begin
		insert into MetricThresholdInstances (InstanceType, ThresholdInstanceName) values(@InstanceType, @InstanceName)
		
		select @InstanceID = SCOPE_IDENTITY()
	end
	else
	begin
		select @InstanceID = InstanceID from MetricThresholdInstances where InstanceType = @InstanceType and ThresholdInstanceName = @InstanceName
	end

END	

