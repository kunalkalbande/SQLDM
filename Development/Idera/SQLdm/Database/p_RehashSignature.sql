---Query that rehashes all the existing entries in dbo.SQLSignature and dbo.SQLStatements---
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

if (object_id('p_RehashSignature') is not null)
begin
drop procedure [p_RehashSignature]
end
go

CREATE PROCEDURE [dbo].[p_RehashSignature]
AS
BEGIN


IF (SELECT count([Internal_Value]) FROM [dbo].[RepositoryInfo] WHERE [Name] = 'SQLSignatureRehashed' ) = 0
BEGIN
	INSERT INTO [dbo].[RepositoryInfo] ([Name], [Internal_Value],[Character_Value]) 
		VALUES ('TotalSignatureID', 0, Convert(NVARCHAR(30), GetDate(), 121)) 
    INSERT INTO [dbo].[RepositoryInfo] ([Name], [Internal_Value],[Character_Value]) 
		VALUES ('SignatureIDRehashed', 0, Convert(NVARCHAR(30), GetDate(), 121)) 

	DECLARE @TotalSignatureID INT 
	DECLARE @SignatureIDRehashed INT
	SET @SignatureIDRehashed = 0 

	SELECT @TotalSignatureID = ISNULL(MAX(SQLSignatureID), 0) FROM SQLSignatures
	UPDATE RepositoryInfo SET Internal_Value = @TotalSignatureID WHERE [Name] = 'TotalSignatureID'
	
	WHILE(@SignatureIDRehashed < @TotalSignatureID)
		BEGIN
	
			DECLARE @ENDPOINT INT
			SELECT  TOP 10000 @ENDPOINT = ISNULL(MAX(SQLSignatures.SQLSignatureID) ,@TotalSignatureID)
			FROM SQLSignatures 
			WHERE SQLSignatures.SQLSignatureID > @SignatureIDRehashed  
				
			UPDATE SQLSignatures 
			SET SQLSignatureHash = 
					dbo.fn_ComputeSHA1(ISNULL(SQLSignatures.SQLSignature,SQLSignaturesOverflow.SQLSignatureOverflow))  
						FROM SQLSignatures LEFT JOIN SQLSignaturesOverflow 
							ON SQLSignatures.SQLSignatureID = SQLSignaturesOverflow.SQLSignatureID 
								WHERE SQLSignatures.SQLSignatureID > @SignatureIDRehashed  AND SQLSignatures.SQLSignatureID <=  @ENDPOINT
					
			SET @SignatureIDRehashed = @ENDPOINT
			UPDATE RepositoryInfo SET Internal_Value = @SignatureIDRehashed WHERE [Name] = 'SignatureIDRehashed'
				
		END

		INSERT INTO [dbo].[RepositoryInfo] ([Name], [Internal_Value],[Character_Value])
			 VALUES ('SQLSignatureRehashed', 1, Convert(NVARCHAR(30), GetDate(), 121))
END
END