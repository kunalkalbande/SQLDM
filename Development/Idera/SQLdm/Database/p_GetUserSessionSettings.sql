-- set User Session Settings
-- example
-- EXEC [dbo].[p_GetUserSessionSettings]  @UserId='NIAD-BLR-01\Admininstrator'


if (object_id('p_GetUserSessionSettings') is not null)
begin
drop procedure p_GetUserSessionSettings
end
go
create procedure [dbo].[p_GetUserSessionSettings]
		(@UserId nvarchar (max))
as
begin
Select US.[Key], US.[Value]
from
(
	select uss.[Key], uss.[Value]
	from [dbo].[UserSessionSettings] as uss
	where uss.[UserID] = @UserId COLLATE SQL_Latin1_General_CP1_CI_AS
) as US
end

GO


