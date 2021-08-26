IF (object_id('p_GetReportServers') is not null)
BEGIN
drop procedure p_GetReportServers
END
GO

CREATE PROCEDURE [dbo].[p_GetReportServers]
	@VMOnly bit = 0,
	@MinimumServerVersion int = 8
AS
BEGIN

declare @IsSQLLogin bit, @SQLLoginName nvarchar(128),@WindowsSID varbinary(85)
select @IsSQLLogin = case when isnull(type,'N') = 'S' then 1 else 0 end from sys.server_principals where principal_id = suser_id()
select @SQLLoginName = suser_sname(), @WindowsSID = suser_sid()

exec p_GetUserTokenInternal @IsSQLLogin, @SQLLoginName, @WindowsSID, 1, @VMOnly, @MinimumServerVersion

END

GO

grant EXECUTE on [p_GetReportServers] to [SQLdmConsoleUser]
