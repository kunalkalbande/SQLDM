--retrieves those databases on which at least one query  
-- has been fired for the given instance

-- @SQLServerID is the Instance ID
-- @StartIndex is the index from where databases needs to be retrieved
-- @RecordsCount is the number of databases to be retrieved

-- exec p_GetDatabasesForServer 9,2,2

if (object_id('p_GetDatabasesForServer') is not null)
begin
	drop procedure [p_GetDatabasesForServer]
end
go

create procedure [dbo].[p_GetDatabasesForServer]
	@SQLServerID int,
	@StartIndex int = 1,
	@RecordsCount int = -1,
	@IsDeletedDBs bit =0 --SQLdm kit1 (Barkha Khatri)
as
begin
	IF (@RecordsCount = -1)
		SELECT
			DISTINCT(QMS.DatabaseID),
			SSDN.DatabaseName,
			SSDN.SystemDatabase AS IsSystemDatabase
		FROM
			[QueryMonitorStatistics] AS QMS 
			INNER JOIN
			[SQLServerDatabaseNames] AS SSDN
			ON QMS.DatabaseID = SSDN.DatabaseID
		WHERE
			QMS.SQLServerID = @SQLServerID and SSDN.IsDeleted=@IsDeletedDBs -- SQLdm kit1 (Barkha khatri)
		ORDER BY
			SSDN.DatabaseName
	ELSE
		begin
			WITH DatabaseNames AS
			(
				SELECT
					DISTINCT(QMS.DatabaseID),
					SSDN.DatabaseName,
					SSDN.SystemDatabase AS IsSystemDatabase,
					DENSE_RANK() OVER (ORDER BY SSDN.DatabaseName) AS RecordRank
				FROM
					[QueryMonitorStatistics] AS QMS 
					INNER JOIN
					[SQLServerDatabaseNames] AS SSDN
					ON QMS.DatabaseID = SSDN.DatabaseID
				WHERE
					QMS.SQLServerID = @SQLServerID  and SSDN.IsDeleted=@IsDeletedDBs -- SQLdm kit1 (Barkha khatri)
			)

			SELECT
				DatabaseID,
				DatabaseName,
				IsSystemDatabase
			FROM
				DatabaseNames
			WHERE
				RecordRank BETWEEN @StartIndex AND (@StartIndex + @RecordsCount - 1)
			ORDER BY
				DatabaseName
		end
end
go