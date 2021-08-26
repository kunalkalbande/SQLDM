-- SQLdm 9.0 (Abhishek Joshi)

-- CWF Registration
-- insert the product's web framework registration information, if it's registered for the first time,
-- else, update the registration information

-- exec p_AddTheProductRegistrationInformation @HostName = 'QA-LOVEJOY',
--                                             @Port = '9292',
--                                             @UserName = 'QA-LOVEJOY\Administrator',
--                                             @Password = 'control*88',
--                                             @ProductID = 2

if (object_id('p_AddTheProductRegistrationInformation') is not null)
begin
	drop procedure [p_AddTheProductRegistrationInformation]
end
go

create procedure [dbo].[p_AddTheProductRegistrationInformation]
	@HostName nvarchar(255),
	@Port nvarchar(4),
	@UserName nvarchar(100),
	@Password nvarchar(100),
	@InstanceName nvarchar(200),
	@ProductID int
as
begin
	if exists (select top 1 HostName from [WebFramework] where ProductID = @ProductID)
		update [WebFramework]
		set HostName = @HostName,
			Port =  @Port,
			UserName = @UserName,
			Password = @Password,
			InstanceName = @InstanceName
		where ProductID = @ProductID
	else
		delete from WebFramework;
		insert into [WebFramework](HostName, Port, UserName, Password,InstanceName, ProductID)
		values(@HostName, @Port, @UserName, @Password, @InstanceName,@ProductID)
end
go
