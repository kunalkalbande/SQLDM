if (object_id('[p_SaveDMHistorySetting]') is not null)
begin
drop procedure [p_SaveDMHistorySetting]
end
go
create procedure [dbo].[p_SaveDMHistorySetting] 
 @key nvarchar(max),
 @value nvarchar(max), 
 @UserId nvarchar(max)
AS
BEGIN
 if(exists(Select [UserID], [Key] from [dbo].[UserSessionSettings] 
   where [UserID] = @UserId and [Key] = @key))
 begin
  update [dbo].[UserSessionSettings]set
  [Value] = @value
  where UserID = @UserId and [Key] = @key
 end
else
 begin
  INSERT INTO [dbo].[UserSessionSettings](
  [UserID],
  [Key],
  [Value]) VALUES (@UserId,@key,@value)
 end
END
