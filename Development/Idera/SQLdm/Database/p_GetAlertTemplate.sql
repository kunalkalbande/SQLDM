if (object_id('p_GetAlertTemplate') is not null)
begin
drop procedure [p_GetAlertTemplate]
end
/****** Object:  StoredProcedure [dbo].[p_GetAlertTemplate]    Script Date: 3/19/2019 11:42:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[p_GetAlertTemplate] 
(
 @TagID int,
 @ServerID int
)

as
BEGIN 
SELECT TG.Name as TagName, MS.InstanceName,AL.Name as TemplateName FROM
 MonitoredSQLServers MS INNER JOIN AlertInstanceTemplate AT ON AT.SQLServerID = MS.SQLServerID
 LEFT OUTER JOIN ServerTags ST ON ST.SQLServerId = MS.SQLServerID
 LEFT OUTER JOIN Tags TG ON TG.Id = ST.TagId
 INNER JOIN AlertTemplateLookup AL ON AL.TemplateID = AT.TemplateID
 WHERE (MS.SQLServerID = @ServerID OR @ServerID = 0) AND (TG.Id =  @TagID OR  @TagID  = 0) and MS.Active=1

 END