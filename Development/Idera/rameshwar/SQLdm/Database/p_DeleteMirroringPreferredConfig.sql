if (object_id('p_DeleteMirroringPreferredConfig') is not null)
begin
drop procedure [p_DeleteMirroringPreferredConfig]
end
go

CREATE PROCEDURE p_DeleteMirroringPreferredConfig 
	-- Add the parameters for the stored procedure here
	@MirroringGuid uniqueidentifier
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	delete from MirroringPreferredConfig 
	where MirroringGuid = @MirroringGuid
END
GO
