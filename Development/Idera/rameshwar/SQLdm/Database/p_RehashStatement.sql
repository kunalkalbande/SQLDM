---Query that rehashes all the existing entries in dbo.SQLStatement and dbo.SQLStatements---
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

if (object_id('p_RehashStatement') is not null)
begin
drop procedure [p_RehashStatement]
end
go

CREATE PROCEDURE [dbo].[p_RehashStatement]
AS
BEGIN
IF (SELECT count([Internal_Value]) FROM [dbo].[RepositoryInfo] WHERE [Name] = 'SQLStatementRehashed' ) = 0
BEGIN
	INSERT INTO [dbo].[RepositoryInfo] ([Name], [Internal_Value],[Character_Value]) 
		VALUES ('TotalStatementID', 0, Convert(NVARCHAR(30), GetDate(), 121)) 
    INSERT INTO [dbo].[RepositoryInfo] ([Name], [Internal_Value],[Character_Value]) 
		VALUES ('StatementIDRehashed', 0, Convert(NVARCHAR(30), GetDate(), 121)) 

	DECLARE @TotalStatementID INT 
	DECLARE @StatementIDRehashed INT
	SET @StatementIDRehashed = 0 

	SELECT @TotalStatementID = ISNULL(MAX(SQLStatementID), 0) FROM SQLStatements
	UPDATE RepositoryInfo SET Internal_Value = @TotalStatementID WHERE [Name] = 'TotalStatementID'
	
	WHILE(@StatementIDRehashed < @TotalStatementID)
		BEGIN
	
			DECLARE @ENDPOINT INT
			SELECT  TOP 10000 @ENDPOINT = ISNULL(MAX(SQLStatements.SQLStatementID) ,@TotalStatementID)
			FROM SQLStatements 
			WHERE SQLStatements.SQLStatementID > @StatementIDRehashed  
				
			UPDATE SQLStatements 
			SET SQLStatementHash = 
					dbo.fn_ComputeSHA1(ISNULL(SQLStatements.SQLStatement,SQLStatementsOverflow.SQLStatementOverflow))  
						FROM SQLStatements LEFT JOIN SQLStatementsOverflow 
							ON SQLStatements.SQLStatementID = SQLStatementsOverflow.SQLStatementID 
								WHERE SQLStatements.SQLStatementID > @StatementIDRehashed  AND SQLStatements.SQLStatementID <=  @ENDPOINT
					
			SET @StatementIDRehashed = @ENDPOINT
			UPDATE RepositoryInfo SET Internal_Value = @StatementIDRehashed WHERE [Name] = 'StatementIDRehashed'
				
		END

		INSERT INTO [dbo].[RepositoryInfo] ([Name], [Internal_Value],[Character_Value])
			 VALUES ('SQLStatementRehashed', 1, Convert(NVARCHAR(30), GetDate(), 121))
END
END