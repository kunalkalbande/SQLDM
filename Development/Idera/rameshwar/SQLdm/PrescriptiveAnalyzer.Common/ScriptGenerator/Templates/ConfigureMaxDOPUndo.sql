declare @value bigint;
declare @maxDOP bigint;
set @maxDOP = $(ObservedMaxDOP);
select @value = cast(value_in_use as bigint) from sys.configurations where configuration_id = 1539; -- maximum degree of parallelism
if @maxDOP <> @value 
begin
	declare @showAdvOpts bigint;
	select @showAdvOpts = cast(value_in_use as bigint) from sys.configurations where configuration_id = 518; -- show advanced options
	if 0 = @showAdvOpts
	begin
		exec sp_configure 'show advanced options', 1;
		reconfigure with override;
	end;
	exec sp_configure 'max degree of parallelism', @maxDOP;
	reconfigure with override;
	if 0 = @showAdvOpts
	begin
		exec sp_configure 'show advanced options', 0;
		reconfigure with override;
	end;
end;
