if (object_id('p_DeleteLicenseKey') is not null)
begin
drop procedure p_DeleteLicenseKey
end
go
CREATE PROCEDURE [dbo].[p_DeleteLicenseKey](
	@LicenseKey nvarchar(255)
)
AS
BEGIN
	DECLARE @e INT

	if (@LicenseKey IS NULL)
	BEGIN
		-- delete em all
		DELETE FROM [LicenseKeys]
	END
	ELSE
	BEGIN
		-- delete a specific key
		DELETE FROM [LicenseKeys] WHERE ([LicenseKey] = @LicenseKey)
	END


	SELECT @e = @@error
	RETURN @e
END
