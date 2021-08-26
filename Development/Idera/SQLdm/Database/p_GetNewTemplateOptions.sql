if (object_id('p_GetNewTemplateOptions') is not null)
begin
drop procedure p_GetNewTemplateOptions
end
go

CREATE PROCEDURE [dbo].p_GetNewTemplateOptions
as
BEGIN
  

-- These selects are broken up on purpose to ensure the order that the data is returned
SELECT 'Template', [TemplateID], [Name] from [dbo].[AlertTemplateLookup] order by [Default] DESC

SELECT 'Server', [SQLServerID], [InstanceName] FROM [dbo].[MonitoredSQLServers]

END

