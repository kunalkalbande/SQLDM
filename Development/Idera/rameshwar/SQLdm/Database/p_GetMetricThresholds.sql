if (object_id('p_GetMetricThresholds') is not null)
begin
drop procedure [p_GetMetricThresholds]
end
go

create procedure [p_GetMetricThresholds](
	@UserViewID int,
	@SQLServerID int,
	@Metric int = null
)
as
begin
	DECLARE @e int

	IF (@UserViewID IS NULL) 
	BEGIN
		SELECT a.[SQLServerID],a.[Metric],[Enabled],[WarningThreshold],[CriticalThreshold],[Data],[UTCSnoozeStart],[UTCSnoozeEnd],[SnoozeStartUser],[SnoozeEndUser],[InfoThreshold], b.[ThresholdInstanceName], a.[ThresholdEnabled] 
		--10.0 srishti purohit -- for baseline alert modifications
		,a.IsBaselineEnabled, a.[BaselineWarningThreshold],
			a.[BaselineCriticalThreshold],
			a.[BaselineInfoThreshold]
			FROM 
				[MetricThresholds] a 
				left join [MetricThresholdInstances] b 
					on a.ThresholdInstanceID = b.InstanceID
			WHERE (@SQLServerID is NULL or a.[SQLServerID] = @SQLServerID) AND
				(@Metric IS NULL or @Metric = a.[Metric])
	END
	ELSE
	BEGIN
		-- returns default metric threshold entries
		SELECT a.[UserViewID],a.[Metric],[Enabled],[WarningThreshold],[CriticalThreshold],[Data],[InfoThreshold], b.ThresholdInstanceName, a.ThresholdEnabled
		--10.0 srishti purohit -- for baseline alert modifications
		, a.[IsBaselineEnabled], a.[BaselineWarningThreshold],
			a.[BaselineCriticalThreshold],
			a.[BaselineInfoThreshold] 
			FROM [DefaultMetricThresholds] a
				left join [MetricThresholdInstances] b 
					on a.ThresholdInstanceID = b.InstanceID
			WHERE [UserViewID] = @UserViewID AND
				(@Metric IS NULL or @Metric = [Metric])
	END

	SELECT @e = @@error
	RETURN @e
end

 
