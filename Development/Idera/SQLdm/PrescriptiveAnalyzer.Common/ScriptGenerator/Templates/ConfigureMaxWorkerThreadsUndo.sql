declare @value bigint;
declare @threads bigint;
set @threads = $(CurrentMaxWorkerThreads);
select @value = cast(value as bigint) from sys.configurations where configuration_id = 503; -- max worker threads
if @threads <> @value 
begin
	declare @showAdvOpts bigint;
	select @showAdvOpts = cast(value_in_use as bigint) from sys.configurations where configuration_id = 518; -- show advanced options
	if 0 = @showAdvOpts
	begin
		exec sp_configure 'show advanced options', 1;
		reconfigure with override;
	end;
	exec sp_configure 'max worker threads', @threads;
	reconfigure with override;
	if 0 = @showAdvOpts
	begin
		exec sp_configure 'show advanced options', 0;
		reconfigure with override;
	end;
end;
