if (object_id('p_GetChangeLogWorkstationUser') is not null)
begin
drop procedure p_GetChangeLogWorkstationUser
end
go
CREATE PROCEDURE [dbo].[p_GetChangeLogWorkstationUser]
AS
BEGIN

DECLARE @RepWorkstation  TABLE(
    [WorkstationUser] nvarchar(256) not null
);
INSERT INTO @RepWorkstation VALUES('< All >')
INSERT @RepWorkstation
SELECT DISTINCT [WorkstationUser]
  FROM [AuditableEvents]
  
SELECT * FROM @RepWorkstation
END
 
