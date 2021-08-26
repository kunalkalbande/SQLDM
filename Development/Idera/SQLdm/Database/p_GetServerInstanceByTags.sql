IF (object_id('p_GetServerInstanceByTags') IS NOT NULL)
BEGIN
DROP PROCEDURE p_GetServerInstanceByTags
END
GO

CREATE PROCEDURE [dbo].[p_GetServerInstanceByTags] 
(  
 @TagsName nvarchar(max)  
)  
AS  
BEGIN  

SELECT ST.SQLServerId, TG.Name FROM Tags TG 
INNER JOIN ServerTags ST ON TG.Id = ST.TagId
INNER JOIN (select Value from dbo.fn_Split(@TagsName,',')) As TagParameters ON TagParameters.Value = TG.Name
 
END  