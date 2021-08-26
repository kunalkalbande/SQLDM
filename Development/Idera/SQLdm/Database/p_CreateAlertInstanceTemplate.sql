if (object_id('p_CreateAlertInstanceTemplate') is not null)
begin
drop procedure p_CreateAlertInstanceTemplate
end
go

create procedure p_CreateAlertInstanceTemplate
as
BEGIN
CREATE TABLE AlertInstanceTemplate
(
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[SQLServerID] [int] NULL,
	[TemplateID] [int] NULL
	)
END


