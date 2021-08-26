/****** Object:  StoredProcedure [dbo].[p_GetInfoToSavePlan]   Script Date: 4/30/2018 3:49:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

if (object_id('p_GetInfoToSavePlan') is not null)
begin
drop procedure [p_GetInfoToSavePlan]
end
go

CREATE PROCEDURE [dbo].[p_GetInfoToSavePlan] 
	@SignatureID INT,
	@StatementType INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT TOP 1
		QMS.SQLStatementID,
		QMS.QueryStatisticsID
	FROM QueryMonitorStatistics AS QMS
	WHERE QMS.SQLSignatureID = @SignatureID AND QMS.StatementType = @StatementType

END
GO