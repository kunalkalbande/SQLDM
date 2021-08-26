if (object_id('p_ImportUpdateEarliestDateImported') is not null)
begin
drop procedure p_ImportUpdateEarliestDateImported
end
go
CREATE PROCEDURE [dbo].[p_ImportUpdateEarliestDateImported]
(
	@SQLServerID int,
	@EarliestDateImportedFromLegacySQLdm datetime
)
AS
BEGIN
	DECLARE @error int

	UPDATE 	[dbo].[MonitoredSQLServers]
	SET
	       	[EarliestDateImportedFromLegacySQLdm] = @EarliestDateImportedFromLegacySQLdm
	WHERE
		[SQLServerID] = @SQLServerID

	SELECT @error = @@error

	IF @error != 0 GOTO HANDLE_ERROR

	RETURN(0)

	HANDLE_ERROR:
		RAISERROR('An error occurred while updating monitored SQL Server import date.', 10, 1)
        RETURN(@error)			
END