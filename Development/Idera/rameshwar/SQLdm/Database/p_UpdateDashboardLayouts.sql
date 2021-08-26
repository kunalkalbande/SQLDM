
if (object_id('p_UpdateDashboardLayouts') is not null)
begin
drop procedure p_UpdateDashboardLayouts
end
go

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

create procedure p_UpdateDashboardLayouts
AS
BEGIN
IF (SELECT COUNT(*) FROM [dbo].DashboardLayouts) = 0
BEGIN
	declare @xmlConfig nvarchar(max)
	SET IDENTITY_INSERT [dbo].DashboardLayouts ON
	SELECT @xmlConfig = '<?xml version="1.0" encoding="utf-16"?><DashboardConfiguration xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">'
		 + '<Panels>'
			 + '<DashboardPanelConfiguration><Column>0</Column><Row>0</Row><Panel>Cpu</Panel></DashboardPanelConfiguration>'
			 + '<DashboardPanelConfiguration><Column>1</Column><Row>0</Row><Panel>ServerWaits</Panel></DashboardPanelConfiguration>'
			 + '<DashboardPanelConfiguration><Column>0</Column><Row>1</Row><Panel>Memory</Panel></DashboardPanelConfiguration>'
			 + '<DashboardPanelConfiguration><Column>1</Column><Row>1</Row><Panel>Cache</Panel></DashboardPanelConfiguration>'
			 + '<DashboardPanelConfiguration><Column>0</Column><Row>2</Row><Panel>Network</Panel></DashboardPanelConfiguration>'
			 + '<DashboardPanelConfiguration><Column>1</Column><Row>2</Row><Panel>Sessions</Panel></DashboardPanelConfiguration>'
			 + '<DashboardPanelConfiguration><Column>0</Column><Row>3</Row><Panel>Disk</Panel></DashboardPanelConfiguration>'
			 + '<DashboardPanelConfiguration><Column>1</Column><Row>3</Row><Panel>Databases</Panel></DashboardPanelConfiguration>'
			 + '<DashboardPanelConfiguration><Column>0</Column><Row>3</Row><Panel>SQLServerPhysicalIO</Panel></DashboardPanelConfiguration>'
		 + '</Panels><Name>SQL Server 2005 and later</Name><Columns>2</Columns><Rows>4</Rows></DashboardConfiguration>'
	INSERT INTO [dbo].DashboardLayouts (DashboardLayoutID, Name, Configuration) VALUES (1, 'SQL Server 2005 and later', @xmlConfig)
	SELECT @xmlConfig = '<?xml version="1.0" encoding="utf-16"?><DashboardConfiguration xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">'
		 + '<Panels>'
			 + '<DashboardPanelConfiguration><Column>0</Column><Row>0</Row><Panel>Cpu</Panel></DashboardPanelConfiguration>'
			 + '<DashboardPanelConfiguration><Column>1</Column><Row>0</Row><Panel>LockWaits</Panel></DashboardPanelConfiguration>'
			 + '<DashboardPanelConfiguration><Column>0</Column><Row>1</Row><Panel>Memory</Panel></DashboardPanelConfiguration>'
			 + '<DashboardPanelConfiguration><Column>1</Column><Row>1</Row><Panel>Cache</Panel></DashboardPanelConfiguration>'
			 + '<DashboardPanelConfiguration><Column>0</Column><Row>2</Row><Panel>Network</Panel></DashboardPanelConfiguration>'
			 + '<DashboardPanelConfiguration><Column>1</Column><Row>2</Row><Panel>Sessions</Panel></DashboardPanelConfiguration>'
			 + '<DashboardPanelConfiguration><Column>0</Column><Row>3</Row><Panel>Disk</Panel></DashboardPanelConfiguration>'
			 + '<DashboardPanelConfiguration><Column>1</Column><Row>3</Row><Panel>Databases</Panel></DashboardPanelConfiguration>'
			 + '<DashboardPanelConfiguration><Column>0</Column><Row>3</Row><Panel>SQLServerPhysicalIO</Panel></DashboardPanelConfiguration>'
		 + '</Panels><Name>SQL Server 2000</Name><Columns>2</Columns><Rows>4</Rows></DashboardConfiguration>'
	INSERT INTO [dbo].DashboardLayouts (DashboardLayoutID, Name, Configuration) VALUES (2, 'SQL Server 2000', @xmlConfig)
	SELECT @xmlConfig = '<?xml version="1.0" encoding="utf-16"?><DashboardConfiguration xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">'
		 + '<Panels>'
			 + '<DashboardPanelConfiguration><Column>0</Column><Row>0</Row><Panel>Memory</Panel></DashboardPanelConfiguration>'
			 + '<DashboardPanelConfiguration><Column>1</Column><Row>0</Row><Panel>Disk</Panel></DashboardPanelConfiguration>'
			 + '<DashboardPanelConfiguration><Column>0</Column><Row>1</Row><Panel>Databases</Panel></DashboardPanelConfiguration>'
			 + '<DashboardPanelConfiguration><Column>1</Column><Row>1</Row><Panel>FileActivity</Panel></DashboardPanelConfiguration>'
			 + '<DashboardPanelConfiguration><Column>0</Column><Row>2</Row><Panel>TempDB</Panel></DashboardPanelConfiguration>'
			 + '<DashboardPanelConfiguration><Column>1</Column><Row>2</Row><Panel>Sessions</Panel></DashboardPanelConfiguration>'
			 + '<DashboardPanelConfiguration><Column>0</Column><Row>3</Row><Panel>Cpu</Panel></DashboardPanelConfiguration>'
			 + '<DashboardPanelConfiguration><Column>1</Column><Row>3</Row><Panel>LockWaits</Panel></DashboardPanelConfiguration>'
			 + '<DashboardPanelConfiguration><Column>0</Column><Row>3</Row><Panel>SQLServerPhysicalIO</Panel></DashboardPanelConfiguration>'
		 + '</Panels><Name>Activity</Name><Columns>2</Columns><Rows>4</Rows></DashboardConfiguration>'
	INSERT INTO [dbo].DashboardLayouts (DashboardLayoutID, Name, Configuration) VALUES (3, 'Activity', @xmlConfig)
	SET IDENTITY_INSERT [dbo].DashboardLayouts OFF
END

END