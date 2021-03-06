/****** Object:  StoredProcedure [dbo].[p_GetQueryPlanInformation]    Script Date: 4/30/2018 3:48:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

if (object_id('p_GetQueryPlanInformation') is not null)
begin
drop procedure [p_GetQueryPlanInformation]
end
go

CREATE procedure [dbo].[p_GetQueryPlanInformation]
	@SQLSignatureID int,
	@StatementType int,	
	@QueryStatisticsID int
as
begin
IF @QueryStatisticsID IS NULL
BEGIN
SELECT TOP 1
		SQP.PlanID, 
		SQP.SQLStatementID, 
		-- Start Query Plan Grpahical View - Changed the selected column and the where condition
		CASE 
			WHEN SQP.Overflow=0
				THEN SQP.PlanXML
			ELSE 
				SQPO.PlanXMLOverflow
		END
		AS PlanXML, 
		SQP.IsActualPlan -- Display estimated query plan - Flag to tell if the plan is actual or estimated one
	FROM 
		[QueryMonitorStatistics] QMS
		RIGHT JOIN SQLQueryPlans AS SQP ON SQP.PlanID=QMS.PlanID
		LEFT JOIN SQLQueryPlansOverflow AS SQPO ON SQP.PlanID = SQPO.PlanID
	WHERE 
		QMS.SQLSignatureID = @SQLSignatureID AND QMS.StatementType = @StatementType
END
ELSE
BEGIN
SELECT TOP 1
		SQP.PlanID, 
		SQP.SQLStatementID, 
		-- Start Query Plan Grpahical View - Changed the selected column and the where condition
		CASE 
			WHEN SQP.Overflow=0
				THEN SQP.PlanXML
			ELSE 
				SQPO.PlanXMLOverflow
		END
		AS PlanXML, 
		SQP.IsActualPlan -- Display estimated query plan - Flag to tell if the plan is actual or estimated one
	FROM 
		[QueryMonitorStatistics] QMS
		RIGHT JOIN SQLQueryPlans AS SQP ON SQP.PlanID=QMS.PlanID
		LEFT JOIN SQLQueryPlansOverflow AS SQPO ON SQP.PlanID = SQPO.PlanID
	WHERE 
		QMS.SQLSignatureID = @SQLSignatureID AND QMS.StatementType = @StatementType AND QMS.QueryStatisticsID = @QueryStatisticsID
END
END