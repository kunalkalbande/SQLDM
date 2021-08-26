declare @showAdvOpts bigint;
declare @is_advanced bit;

select @is_advanced = is_advanced from sys.configurations where name = '$(Configuration)';
select @showAdvOpts = cast(value_in_use as bigint) from sys.configurations where configuration_id = 518; -- show advanced options

if ((0 = @showAdvOpts) and (1 = @is_advanced))
begin
	exec sp_configure 'show advanced options', 1;
	reconfigure with override;
end;

exec sp_configure '$(Configuration)',$(DefaultValue);
reconfigure with override;

if ((0 = @showAdvOpts) and (1 = @is_advanced))
begin
	exec sp_configure 'show advanced options', 0;
	reconfigure with override;
end;
