if (object_id('p_GetCustomCountersForTemplate') is not null)
begin
drop procedure p_GetCustomCountersForTemplate
end
go

create PROCEDURE [dbo].[p_GetCustomCountersForTemplate]
(
@SqlServerID int
)
AS
begin
	declare @e int

		BEGIN
		create table #AlertThresholds(
					MetricID INT, 
					MetricName VARCHAR(300),
					Enabled BIT,
					TemplateName VARCHAR(300),
					WarningThresholdSrc NVARCHAR(200), 
					CriticalThresholdSrc NVARCHAR(200),
					InfoThresholdSrc NVARCHAR(200),
					criticalThresholdEnabled NVARCHAR(256),
					warningThresholdEnabled NVARCHAR(256),
					infoThresholdEnabled NVARCHAR(256),

					WarningThresholdTrg NVARCHAR(200), 
					CriticalThresholdTrg NVARCHAR(200),
					InfoThresholdTrg NVARCHAR(200)
		)
		insert into #AlertThresholds 
		Exec p_GetAlertThreshold @SqlServerID

		SElect MetricID from #AlertThresholds
		where MetricID in (SELECT Metric from CustomCounterDefinition) and Enabled=1

		drop Table #AlertThresholds
		END
	

	SET @e = @@ERROR

	RETURN @e
end
 
GO


