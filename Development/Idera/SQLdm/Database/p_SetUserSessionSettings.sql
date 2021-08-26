-- set User Session Settings
-- example
-- EXEC [dbo].[p_SetUserSessionSettings] @settings='<Settings><Setting Key="keytest" Value="value"/></Settings>', @UserId='NIAD-BLR-01\Admininstrator'

if (object_id('p_SetUserSessionSettings') is not null)
begin
drop procedure p_SetUserSessionSettings
end
go


CREATE Procedure [dbo].[p_SetUserSessionSettings]
			@settings xml,
			@UserId nvarchar(max)
as
begin
if(exists(Select [UserID], [Key] from [dbo].[UserSessionSettings] 
			where [UserID] COLLATE SQL_Latin1_General_CP1_CI_AS = @UserId and [Key] COLLATE SQL_Latin1_General_CP1_CI_AS in (select s.v.value('@Key', 'nvarchar(max)') as [Key] 
from @settings.nodes('Settings/Setting')  s(v))))
	begin
		update [dbo].[UserSessionSettings] set
		[Value] = s.v.value('@Value', 'nvarchar(max)')
		from [dbo].[UserSessionSettings] as uss cross apply @settings.nodes('Settings/Setting') as s(v)
		where uss.UserID COLLATE SQL_Latin1_General_CP1_CI_AS  = @UserId and uss.[Key] COLLATE SQL_Latin1_General_CP1_CI_AS = s.v.value('@Key', 'nvarchar(max)')
	end

else
	begin
		INSERT INTO [dbo].[UserSessionSettings](
		[UserID],
		[Key],
		[Value])
		SELECT @UserId, s.v.value('@Key', 'nvarchar(max)'), s.v.value('@Value', 'nvarchar(max)') 
		from @settings.nodes('Settings/Setting') as s(v);
	end
end

GO


