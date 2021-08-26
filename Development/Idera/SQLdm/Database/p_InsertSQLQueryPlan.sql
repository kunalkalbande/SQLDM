--Added new procedure in SQLdm 9.0 by Ankit Srivastava -- inserting query plan 
--declare @planId int
--EXEC p_InsertSQLQueryPlan 'QRWTWT23142425wtwtee,234,@planId
 
if (object_id('p_InsertSQLQueryPlan') is not null)
begin
drop procedure p_InsertSQLQueryPlan
end
go
create procedure [dbo].[p_InsertSQLQueryPlan]
	@QueryPlan nvarchar(4000),
	@SQLStatementID int,
	@PlanID int output, 
	@IsActualPlan bit = 1 --SQLdm 10.0 (Tarun Sapra) - Display Estimated Query Plan - Flag to tell if the query plan is actual or estimated
as
begin

	if @QueryPlan is null or len(@QueryPlan) = 0 
		return

	DECLARE @PlanFieldLenInSQLQueryPlans INT
	SET @PlanFieldLenInSQLQueryPlans = 0;
	
	-- SQLdm 10.0.2 (Gaurav Karwal): dynamically picking up the field length of plan
	SELECT @PlanFieldLenInSQLQueryPlans = isnull(max_length,0) FROM sys.columns with(nolock) WHERE LOWER(OBJECT_NAME(object_id)) = 'sqlqueryplans' and LOWER(name) = 'planxml';
	
	
	
	if (@QueryPlan is not null or @QueryPlan <> 'NULL')
	begin
	-- SQLdm 10.0.2 (Gaurav Karwal): if the field length has been updated to max, then go ahead else restrict to the max field length in sqlqueryplans table
		if (@PlanFieldLenInSQLQueryPlans = -1 OR LEN(@QueryPlan) <= @PlanFieldLenInSQLQueryPlans)
		begin
			if not exists(Select PlanID from SQLQueryPlans with (nolock) where [PlanXML]=@QueryPlan and [SQLStatementID]=@SQLStatementID) --SQLdm 9.0 (Ankit Srivastava) - SQL Query Plan View -validating against the combination of both the columns
			BEGIN
				
				insert into SQLQueryPlans([PlanXML],[SQLStatementID],[Overflow],[IsActualPlan]) values (@QueryPlan ,@SQLStatementID,0,@IsActualPlan) --SQLdm 10.0 (Tarun Sapra) - Display Estimated Query Plan - Flag to tell if the query plan is actual or estimated
				select @PlanID = scope_identity()
			END
			ELSE
				Select @PlanID=PlanID from SQLQueryPlans with (nolock) where [PlanXML]=@QueryPlan and [SQLStatementID]=@SQLStatementID --SQLdm 9.0 (Ankit Srivastava) - SQL Query Plan View - Selection on based of both the columns
		end
		else
		begin
		if not exists(Select SQLQueryPlans.PlanID from SQLQueryPlans with (nolock) Inner Join SQLQueryPlansOverflow with (nolock) ON SQLQueryPlans.PlanID =SQLQueryPlansOverflow.PlanID where [PlanXMLOverflow]=@QueryPlan and [SQLStatementID]=@SQLStatementID) --SQLdm 9.0 (Ankit Srivastava) - SQL Query Plan View -validating against the combination of both the columns
			BEGIN
				BEGIN TRANSACTION;
					BEGIN TRY	--SQLdm 10.0.2 (Barkha Khatri) both inserts are added in transaction for consisitency
						insert into SQLQueryPlans([SQLStatementID],[Overflow],[IsActualPlan]) values (@SQLStatementID,1,@IsActualPlan) --SQLdm 10.0 (Tarun Sapra) - Display Estimated Query Plan - Flag to tell if the query plan is actual or estimated
						select @PlanID = scope_identity()
						insert into SQLQueryPlansOverflow([PlanXMLOverflow],[PlanID]) values (@QueryPlan,@PlanID)
					END TRY
					BEGIN CATCH
						IF @@TRANCOUNT > 0
								ROLLBACK TRANSACTION;
					END CATCH;
					
				IF @@TRANCOUNT > 0
				COMMIT TRANSACTION;

			END
		else
		Select @PlanID = SQP.[PlanID]  from SQLQueryPlans SQP with (nolock)INNER JOIN SQLQueryPlansOverflow with (nolock) ON SQP.PlanID =SQLQueryPlansOverflow.PlanID where [PlanXMLOverflow]=@QueryPlan and SQP.[SQLStatementID]=@SQLStatementID --SQLdm 9.0 (Ankit Srivastava) - SQL Query Plan View - Selection on based of both the columns SQLdm 10.0.2 (Barkha Khatri) Inner join and nolock added 
		end
	end

	
end

GO


