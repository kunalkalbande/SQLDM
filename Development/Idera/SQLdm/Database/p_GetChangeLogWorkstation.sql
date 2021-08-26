if (object_id('p_GetChangeLogWorkstation') is not null)
begin
drop procedure p_GetChangeLogWorkstation
end
go
CREATE PROCEDURE [dbo].[p_GetChangeLogWorkstation]
AS
BEGIN

DECLARE @RepWorkstation  TABLE(
    [Workstation] nvarchar(256) not null
);
INSERT INTO @RepWorkstation VALUES('< All >')
INSERT @RepWorkstation
SELECT DISTINCT [Workstation]
  FROM [AuditableEvents]
  
SELECT * FROM @RepWorkstation
END
 
