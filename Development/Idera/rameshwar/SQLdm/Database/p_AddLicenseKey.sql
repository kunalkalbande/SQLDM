if (object_id('p_AddLicenseKey') is not null)
begin
drop procedure p_AddLicenseKey
end
go
CREATE PROCEDURE [dbo].[p_AddLicenseKey](
	@LicenseKey nvarchar(255),
	@ReturnLicenseID uniqueidentifier output
)
as 
begin
	declare @e int
	declare @id uniqueidentifier

	select @id = NEWID()

	INSERT INTO [LicenseKeys]
           ([LicenseID]
           ,[LicenseKey]
           ,[DateAddedUtc])
     VALUES
           (@id,
           @LicenseKey,
           GETUTCDATE())

	select @e = @@error

	IF (@e = 0)
	begin
		select @ReturnLicenseID = @id 
	end

	return @e
end




