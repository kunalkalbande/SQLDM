
IF (object_id('p_UpdateSQLsafeLastDefragActionId') IS NOT NULL)
BEGIN
DROP PROCEDURE p_UpdateSQLsafeLastDefragActionId
END
GO

CREATE PROCEDURE [dbo].[p_UpdateSQLsafeLastDefragActionId]
(
	@Id	INT,
	@ActionId INT
)
AS
BEGIN
	if exists (select [LastDefragActionId] from MonitoredServerSQLsafeInstance where SQLServerID = @Id)
	BEGIN
		update MonitoredServerSQLsafeInstance set LastDefragActionId = @ActionId where SQLServerID = @Id
	END
END