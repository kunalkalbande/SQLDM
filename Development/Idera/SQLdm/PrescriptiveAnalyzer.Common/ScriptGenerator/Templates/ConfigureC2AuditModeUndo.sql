declare @value bigint;
select @value = cast(value_in_use as bigint) from sys.configurations where configuration_id = 544; -- c2 audit mode
if 0 = @value 
begin
	declare @showAdvOpts bigint;
	select @showAdvOpts = cast(value_in_use as bigint) from sys.configurations where configuration_id = 518; -- show advanced options
	if 0 = @showAdvOpts
	begin
		exec sp_configure 'show advanced options', 1;
		reconfigure with override;
	end;
	exec sp_configure 'c2 audit mode', 1;
	reconfigure with override;
	if 0 = @showAdvOpts
	begin
		exec sp_configure 'show advanced options', 0;
		reconfigure with override;
	end;
end;