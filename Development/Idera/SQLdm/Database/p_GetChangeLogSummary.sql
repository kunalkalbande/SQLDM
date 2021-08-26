if (object_id('p_GetChangeLogSummary') is not null)
begin
drop procedure [p_GetChangeLogSummary]
end
go

CREATE PROCEDURE [dbo].[p_GetChangeLogSummary]
		@AuditName nvarchar(128),
		@UTCStart DateTime,
		@UTCEnd DateTime,
		@UTCOffset int,
		@Workstation nvarchar(256),
		@WorkstationUser nvarchar(256),
		@SQLUser nvarchar(256)
AS
BEGIN

DECLARE @SQLString NVARCHAR(MAX) -- String that will contain the main SQL to be executed

SELECT @SQLString = 'SELECT 
	  [AuditableEventID]
	  ,AE.[ActionID]
	  ,[Action] = AA.Name
      ,dateadd(mi, @UTCOffset, [DateTime]) as [DateTime]
      ,[Workstation]
      ,[WorkstationUser]
      ,[SQLUser]
      ,AE.[Name]
      ,[MetaData]
      ,[Header]
  FROM [dbo].[AuditableEvents] AE, [dbo].AuditableActions AA
  WHERE AE.ActionID = AA.ActionID 
  AND  [DateTime] BETWEEN @UTCStart and @UTCEnd'

IF (@AuditName != '< All >')
BEGIN
	SELECT @SQLString = @SQLString + ' AND AA.Name = @AuditName'
END

IF (@Workstation != '< All >')
BEGIN
	SELECT @SQLString = @SQLString + ' AND AE.Workstation = @Workstation'
END

IF (@WorkstationUser != '< All >')
BEGIN
	SELECT @SQLString = @SQLString + ' AND AE.WorkstationUser = @WorkstationUser'
END

IF (@SQLUser != '< All >')
BEGIN
	SELECT @SQLString = @SQLString + ' AND AE.SQLUser = @SQLUser'
END

SELECT @SQLString = @SQLString + ' ORDER BY [DateTime] DESC'

declare @ParamDefinition nvarchar(500)
SET @ParamDefinition = N'@AuditName nvarchar(128),
						@UTCStart DateTime,
						@UTCEnd DateTime,
						@UTCOffset int,						
						@Workstation nvarchar(256),
						@WorkstationUser nvarchar(256),
						@SQLUser nvarchar(256)';
EXECUTE sp_executesql @SQLString, 
					  @ParamDefinition,
					  @AuditName = @AuditName,
					  @UTCStart = @UTCStart,
					  @UTCEnd = @UTCEnd,
					  @UTCOffset = @UTCOffset,					  
					  @Workstation = @Workstation,
					  @WorkstationUser = @WorkstationUser,
					  @SQLUser = @SQLUser

END
