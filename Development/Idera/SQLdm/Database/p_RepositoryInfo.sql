if (object_id('[p_RepositoryInfo]') is not null)
begin
drop procedure [p_RepositoryInfo]
end
go
create procedure [dbo].[p_RepositoryInfo]
as
begin
	select 
		'Instance Name' as 'Name',
		NULL as 'Internal_Value',
		CONVERT(nvarchar(128), serverproperty('servername')) as 'Character_Value'
	union
	select 
		'Repository Version' as 'Name',
		NULL as 'Internal_Value',
		dbo.fn_GetDatabaseVersion() as 'Character_Value'
	union
	select 
		'Active Servers' as 'Name',
		count(SQLServerID) as 'Internal_Value',
		cast(count(SQLServerID) as nvarchar(256)) as 'Character_Value'
	from MonitoredSQLServers (NOLOCK)
	where Active=1
	union
	select
		Name,
		Internal_Value,
		Character_Value
	from
		[RepositoryInfo] (NOLOCK)
end

