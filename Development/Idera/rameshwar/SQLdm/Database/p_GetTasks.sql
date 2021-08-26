-- Get desired to-do items from Tasks table.
if (object_id('p_GetTasks') is not null)
begin
drop procedure p_GetTasks
end
go
CREATE PROCEDURE [dbo].p_GetTasks(
	@FromDate datetime,
	@Status TINYINT , -- A bitmap of desired states.
	@Severity tinyint,  -- A bitmap of desired severities.
	@ServerXML nvarchar(max), -- Null for all????
	@Owner nvarchar(256) = null -- Null for all
)
AS
begin

	declare @IntermediateTable table(InstanceName nvarchar(255))
	declare @xmlDoc int

	exec sp_xml_preparedocument @xmlDoc output, @ServerXML

	insert into @IntermediateTable
	select InstanceName
		from openxml(@xmlDoc, '//Server', 1) with (InstanceName nvarchar(255)) 

	exec sp_xml_removedocument @xmlDoc


	SELECT	T.[TaskID], T.[Status], T.[Subject], T.[Message], T.[ServerName],
			T.[Comments], T.[Owner], T.[CreatedOn], T.[CompletedOn],
			T.[Metric], T.[Severity], T.[Value], T.[EventID]
	FROM [Tasks] T
	LEFT OUTER JOIN [MonitoredSQLServers] M ON
		T.[ServerName] = M.[InstanceName]
	INNER JOIN [MetricMetaData] MD ON
		T.[Metric] = MD.[Metric]
	WHERE ( 
			(M.Active is null or M.[Active] = 1) and 
			[CreatedOn] >= @FromDate and
			[Status] & @Status <> 0 and
			[Severity] & @Severity <> 0 and
			(@Owner is null or LOWER([Owner]) = LOWER(@Owner)) and
			(T.[Metric] = 52 or [ServerName] collate database_default in (select InstanceName collate database_default from @IntermediateTable)) and
			MD.[Deleted] = 0
	)
end

-- go
--exec [p_GetTasks] 0, 24, 2, null, null