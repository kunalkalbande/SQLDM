declare @value bigint;
select @value = cast(value_in_use as bigint) from sys.configurations where configuration_id = 1534; -- user options
if 512 = (@value & 512)
begin
	set @value = @value & ~512;
	exec sp_configure 'user options', @value;
	reconfigure with override;
end;