if (object_id('p_AddAdvanceFilteringAlert') is not null)
begin
drop procedure p_AddAdvanceFilteringAlert
end
go
CREATE PROCEDURE [dbo].[p_AddAdvanceFilteringAlert] 
(
	-- Table column params
	@ServerName				nvarchar(256),
	@Metric					int,	
	@CurrentThresholdType	varchar(MAX),
	@DBName varchar(150)
)
AS
BEGIN
	SELECT 1;
END
 



