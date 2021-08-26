/****** Object:  StoredProcedure [dbo].[p_AddSQLQueryPlan]   Script Date: 4/30/2018 3:50:10 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

if (object_id('p_AddSQLQueryPlan') is not null)
begin
drop procedure [p_AddSQLQueryPlan]
end
go

CREATE PROCEDURE [dbo].[p_AddSQLQueryPlan]
	@StatementID INT,
	@QueryStatisticsID INT,
	@QueryPlan NVARCHAR(4000), 
	@IsActualPlan BIT = 1
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	IF @QueryPlan IS NULL OR LEN(@QueryPlan) = 0 -- If the plan is null, return
		RETURN
	
	DECLARE @PlanID INT;
	EXEC p_InsertSQLQueryPlan @QueryPlan, @StatementID, @PlanID OUTPUT, @IsActualPlan

	UPDATE QueryMonitorStatistics
	SET PlanID = @PlanID
	WHERE  QueryStatisticsID = @QueryStatisticsID

END
GO
