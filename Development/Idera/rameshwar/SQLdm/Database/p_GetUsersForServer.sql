--retrieves those users who fired at least one 
-- query on the given instance

-- @SQLServerID is the Instance ID
-- @StartIndex is the index from where users needs to be retrieved
-- @RecordsCount is the number of users to be retrieved

-- exec p_GetUsersForServer 9,2,1

if (object_id('p_GetUsersForServer') is not null)
begin
	drop procedure [p_GetUsersForServer]
end
go

create procedure [dbo].[p_GetUsersForServer]
	@SQLServerID int,
	@StartIndex int = 1,
	@RecordsCount int = -1
as
begin
	IF (@RecordsCount = -1)
		SELECT
			DISTINCT(QMS.LoginNameID) AS UserID,	
			LN.LoginName	AS UserName
		FROM
			[QueryMonitorStatistics] AS QMS 
			INNER JOIN 
			[LoginNames] AS LN
			ON QMS.LoginNameID = LN.LoginNameID
		WHERE
			QMS.SQLServerID = @SQLServerID
		ORDER BY
			LN.LoginName
	ELSE
		begin
			WITH UserNames AS
			(
				SELECT
					DISTINCT(QMS.LoginNameID) AS UserID,	
					LN.LoginName	AS UserName,
					DENSE_RANK() OVER (ORDER BY LN.LoginName) AS RecordRank
				FROM
					[QueryMonitorStatistics] AS QMS 
					INNER JOIN 
					[LoginNames] AS LN
					ON QMS.LoginNameID = LN.LoginNameID
				WHERE
					QMS.SQLServerID = @SQLServerID
			)

			SELECT
				UserID,
				UserName
			FROM
				UserNames
			WHERE
				RecordRank BETWEEN @StartIndex AND (@StartIndex + @RecordsCount - 1)
			ORDER BY
				UserName
		end
end
go