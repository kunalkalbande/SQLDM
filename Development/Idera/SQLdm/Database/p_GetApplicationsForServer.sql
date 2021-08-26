--retrieves those applications which fired at least 
-- one query on the given instance

-- @SQLServerID is the Instance ID
-- @StartIndex is the index from where applications needs to be retrieved
-- @RecordsCount is the number of applications to be retrieved

-- exec p_GetApplicationsForServer 9,2,3

if (object_id('p_GetApplicationsForServer') is not null)
begin
	drop procedure [p_GetApplicationsForServer]
end
go

create procedure [dbo].[p_GetApplicationsForServer]
	@SQLServerID int,
	@StartIndex int = 1,
	@RecordsCount int = -1
as
begin
	IF (@RecordsCount = -1)
		SELECT
			DISTINCT QMS.ApplicationNameID,
			AN.ApplicationName
		FROM
			[QueryMonitorStatistics] AS QMS 
			INNER JOIN 
			[ApplicationNames] AS AN 
			ON QMS.ApplicationNameID = AN.ApplicationNameID
		WHERE
			QMS.SQLServerID = @SQLServerID
		ORDER BY
			AN.ApplicationName
	ELSE
		begin
			WITH AppNames AS
			(
				SELECT
					DISTINCT QMS.ApplicationNameID,
					AN.ApplicationName,
					DENSE_RANK() OVER (ORDER BY AN.ApplicationName) AS RecordRank
				FROM
					[QueryMonitorStatistics] AS QMS 
					INNER JOIN 
					[ApplicationNames] AS AN 
					ON QMS.ApplicationNameID = AN.ApplicationNameID
				WHERE
					QMS.SQLServerID = @SQLServerID
				
			)

			SELECT
				ApplicationNameID,
				ApplicationName
			FROM
				AppNames
			WHERE
				RecordRank BETWEEN @StartIndex AND (@StartIndex + @RecordsCount - 1)
				ORDER BY
					ApplicationName
		end
end
go