if (object_id('p_GetCloudProviders') is not null)
begin
drop procedure p_GetCloudProviders
end
go

create procedure p_GetCloudProviders
as
begin
	select CloudProviderId,CloudProviderName from CloudProviders
end