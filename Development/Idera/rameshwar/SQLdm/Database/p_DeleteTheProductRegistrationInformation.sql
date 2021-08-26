-- SQLdm 10.1 (Praveen Suhalka)

-- CWF Registration
-- delete the SQLdm's web framework registration information, when SQLdm is unregistered from the web framework.


-- exec p_DeleteTheProductRegistrationInformation 

if (object_id('p_DeleteTheProductRegistrationInformation') is not null)
begin
	drop procedure [p_DeleteTheProductRegistrationInformation]
end
go

create procedure [dbo].[p_DeleteTheProductRegistrationInformation]
as
begin
		delete from WebFramework;
end
go