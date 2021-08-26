--retrieves those clients (hosts) through which at least one 
-- query has been fired on the given instance

-- @SQLServerID is the Instance ID
-- @StartIndex is the index from where clients needs to be retrieved
-- @RecordsCount is the number of clients to be retrieved

-- exec p_GetClientsForServer 9,1,2
if (object_id('p_GetClientsForServer') is not null)
begin
	drop procedure [p_GetClientsForServer]
end
go

create procedure [dbo].[p_GetClientsForServer]
	@SQLServerID int,
	@StartIndex int = 1,
	@RecordsCount int = -1
as
begin
	IF (@RecordsCount = -1)
		SELECT
			DISTINCT(QMS.HostNameID) AS ClientID,	
			HN.HostName	 AS ClientName
		FROM
			[QueryMonitorStatistics] AS QMS 
			INNER JOIN 
			[HostNames] AS HN
			ON QMS.HostNameID = HN.HostNameID
		WHERE
			QMS.SQLServerID = @SQLServerID
		ORDER BY
			HN.HostName
	ELSE
		begin
			WITH ClientNames AS
			(
				SELECT
					DISTINCT(QMS.HostNameID) AS ClientID,	
					HN.HostName	 AS ClientName,
					DENSE_RANK() OVER (ORDER BY HN.HostName) AS RecordRank
				FROM
					[QueryMonitorStatistics] AS QMS 
					INNER JOIN 
					[HostNames] AS HN
					ON QMS.HostNameID = HN.HostNameID
				WHERE
					QMS.SQLServerID = @SQLServerID
			)
	
			SELECT
				ClientID,
				ClientName
			FROM
				ClientNames
			WHERE
				RecordRank BETWEEN @StartIndex AND (@StartIndex + @RecordsCount - 1)
			ORDER BY
				ClientName
		end
end
go