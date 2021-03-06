/****** Object:  StoredProcedure [dbo].[p_GetAllAuditableEvents]    Script Date: 02/07/2013 10:44:04 ******/
if (object_id('p_GetAllAuditableEvents') is not null)
begin
drop procedure p_GetAllAuditableEvents
end
go

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[p_GetAllAuditableEvents]
	@UTCStart DateTime,
	@UTCEnd DateTime,
	@TopCount INTEGER
AS
BEGIN
  SELECT TOP (@TopCount)
	  [AuditableEventID]
	  ,AE.[ActionID]
	  ,[Action] = AA.Name
      ,[DateTime]
      ,[Workstation]
      ,[WorkstationUser]
      ,[SQLUser]
      ,AE.[Name]
      ,[MetaData]
      ,[Header]
  FROM [dbo].[AuditableEvents] AE, [dbo].[AuditableActions] AA
  WHERE AE.ActionID = AA.ActionID 
  AND  [DateTime] BETWEEN @UTCStart and @UTCEnd
  ORDER BY [DateTime] desc
  
end
