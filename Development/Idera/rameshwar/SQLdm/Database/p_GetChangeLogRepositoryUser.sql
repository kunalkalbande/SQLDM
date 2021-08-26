if (object_id('p_GetChangeLogRepositoryUser') is not null)
begin
drop procedure p_GetChangeLogRepositoryUser
end
go
CREATE PROCEDURE [dbo].[p_GetChangeLogRepositoryUser]
AS
BEGIN

DECLARE @RepRepository  TABLE(
    [SQLUser] nvarchar(256) not null
);
INSERT INTO @RepRepository VALUES('< All >')
INSERT @RepRepository
SELECT DISTINCT [SQLUser]
  FROM [AuditableEvents]
  
SELECT * FROM @RepRepository
END

