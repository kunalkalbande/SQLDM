-- SQLdm 9.0 (Abhishek Joshi)

-- CWF Registration
-- get the product's web framework registration information

-- exec p_GetTheProductRegistrationInformation

if (object_id('p_GetTheProductRegistrationInformation') is not null)
begin
	drop procedure [p_GetTheProductRegistrationInformation]
end
go

create procedure [dbo].[p_GetTheProductRegistrationInformation]
as
begin
	select top 1
		WebFrameworkID,
		HostName,
		Port,
		UserName,
		Password,
		InstanceName,
		ProductID   
	from 
		[WebFramework]
end
go
