-- SQLdm 9.0 (Abhishek Joshi)
-- retrieves the query plan for a QueryStatisticsID

-- @QueryStatisticsID is the QueryStatisticsID column of the QueryMonitorStatistics table

-- exec p_GetQueryPlan @QueryStatisticsID = 7

if (object_id('p_GetQueryPlan') is not null)
begin
	drop procedure [p_GetQueryPlan]
end
go

create procedure [dbo].[p_GetQueryPlan]
	@QueryStatisticsID int -- SQLdm 9.0 (Ankit Srivastava) - Query Plan Grpahical View - Changed the paramter from statementid to statisticsid
as
begin
	SELECT 
		SQP.PlanID, 
		SQP.SQLStatementID, 
		-- Start -SQLdm 9.0 (Ankit Srivastava) - Query Plan Grpahical View - Changed the selected column and the where condition
		CASE 
			WHEN SQP.Overflow=0
				THEN SQP.PlanXML
			ELSE 
				SQPO.PlanXMLOverflow
		END
		AS PlanXML, 
		SQP.IsActualPlan --SQLdm 10.0 (Tarun Sapra) - Display estimated query plan - Flag to tell if the plan is actual or estimated one
	FROM 
		[QueryMonitorStatistics] QMS
		RIGHT JOIN SQLQueryPlans AS SQP ON SQP.PlanID=QMS.PlanID
		LEFT JOIN SQLQueryPlansOverflow AS SQPO ON SQP.PlanID = SQPO.PlanID
	WHERE 
		QMS.QueryStatisticsID = @QueryStatisticsID
	-- End -SQLdm 9.0 (Ankit Srivastava) - Query Plan Grpahical View - Changed the selected column and the where condition
end
go
