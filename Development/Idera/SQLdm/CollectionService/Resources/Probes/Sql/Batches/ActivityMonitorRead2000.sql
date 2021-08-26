-- Read Query Monitor Trace Segment

if (select isnull(object_id('tempdb..#TraceEvents'), 0)) = 0 
	create table #TraceEvents (
		colid int, 
		datlength int, 
		datatext image) 

declare @switchToAlternate bit

set @switchToAlternate = 0

select @counter = 0 

Loop: 

if (@switchToAlternate = 0)
begin
truncate table #TraceEvents 
insert into #TraceEvents 
	exec sp_trace_getdata @P1, 1 
end
set @rowcount = @@rowcount

if (@enableAlternate = 1)
begin
	if (@rowcount =0 or exists(select * from #TraceEvents where datlength = 0))
		set @switchToAlternate = 1
end
	
if @switchToAlternate = 1
begin
truncate table #TraceEvents 
insert into #TraceEvents 
	exec sp_trace_getdata @P2, 1 
	
set @rowcount = @@rowcount	
end

if @rowcount <> 0 and not exists (select * from #TraceEvents where datlength = 0)
begin 
	select @duration = 0, @reads = 0, @writes = 0, @cpu = 0 
	
	select 
		@eventclass = 
		case 
			when colid = 65526 
				then ascii(convert(nvarchar(1),convert(varbinary(1), datatext))) 
			else @eventclass 
		end, 
		@sqltext = 
		case 
			when colid = 1 
				then rtrim(convert(nvarchar(3061),convert(varbinary(6122), datatext))) 
			else @sqltext 
		end, 
		@ntusername = 
		case 
			when colid = 6 
				then rtrim(convert(nvarchar(128),convert(varbinary(256), datatext))) 
			else @ntusername 
		end, 
		@ntdomainname = 
		case 
			when colid = 7 
				then rtrim(convert(nvarchar(128),convert(varbinary(256), datatext))) 
			else @ntdomainname 
		end, 
		@hostname = 
		case 
			when colid = 8 
				then rtrim(convert(nvarchar(128),convert(varbinary(256), datatext))) 
			else @hostname 
		end, 
		@appname = 
		case 
			when colid = 10 
				then rtrim(convert(nvarchar(128),convert(varbinary(256), datatext))) 
			else @appname 
		end, 
		@sqlname = 
		case 
			when colid = 11 
				then rtrim(convert(nvarchar(255),convert(varbinary(512), datatext))) 
			else @sqlname 
		end 
	from #TraceEvents

	-- Set DBID from hex data
	select 
		@hex = convert(varbinary(10), datatext) 
	from 
		#TraceEvents 
	where 
		colid = 3 

	if @@rowcount > 0 
		select 
			@DBID = convert(bigint,substring(@hex, 1, 1)) 
					+ (convert(bigint,substring(@hex, 2, 1)) * 255) 
					+ (convert(bigint,substring(@hex, 3, 1)) * 65025) 
					+ (convert(bigint,substring(@hex, 4, 1)) * 16581375) 

	-- Set spid from hex data
	select 
		@hex = convert(varbinary(10), datatext) 
	from 
		#TraceEvents 
	where 
		colid = 12 

	if @@rowcount > 0 
		select 
			@spid = convert(bigint,substring(@hex, 1, 1)) 
					+ (convert(bigint,substring(@hex, 2, 1)) * 255) 
					+ (convert(bigint,substring(@hex, 3, 1)) * 65025) 
					+ (convert(bigint,substring(@hex, 4, 1)) * 16581375) 


	-- Set duration from hex data
	select 
		@hex = convert(varbinary(10), datatext) 
	from 
		#TraceEvents 
	where 
		colid = 13 

	if @@rowcount > 0 
		select 
			@duration = convert(bigint,substring(@hex, 1, 1)) 
						+ (convert(bigint,substring(@hex, 2, 1)) * 255) 
						+ (convert(bigint,substring(@hex, 3, 1)) * 65025) 
						+ (convert(bigint,substring(@hex, 4, 1)) * 16581375) 

	-- Set reads from hex data
	select 
		@hex = convert(varbinary(10), datatext) 
	from 
		#TraceEvents 
	where 
		colid = 16 

	if @@rowcount > 0 
		select 
			@reads = convert(bigint,substring(@hex, 1, 1)) 
					+ (convert(bigint,substring(@hex, 2, 1)) * 255) 
					+ (convert(bigint,substring(@hex, 3, 1)) * 65025) 
					+ (convert(bigint,substring(@hex, 4, 1)) * 16581375) 

	-- Set writes from hex data
	select 
		@hex = convert(varbinary(10), datatext) 
	from 
		#TraceEvents 
	where 
		colid = 17 

	if @@rowcount > 0 
		select 
			@writes = convert(bigint,substring(@hex, 1, 1)) 
					+ (convert(bigint,substring(@hex, 2, 1)) * 255) 
					+ (convert(bigint,substring(@hex, 3, 1)) * 65025) 
					+ (convert(bigint,substring(@hex, 4, 1)) * 16581375) 
	
	-- Set cpu from hex data
	select 
		@hex = convert(varbinary(10), datatext) 
	from 
		#TraceEvents 
	where 
		colid = 18 

	if @@rowcount > 0 
		select 
			@cpu = convert(bigint,substring(@hex, 1, 1)) 
					+ (convert(bigint,substring(@hex, 2, 1)) * 255) 
					+ (convert(bigint,substring(@hex, 3, 1)) * 65025) 
					+ (convert(bigint,substring(@hex, 4, 1)) * 16581375) 

	--Set date/time from hex data
	select 
		@hex = convert(varbinary(30), datatext) 
	from 
		#TraceEvents 
	where 
		colid = 15 

	if @@rowcount > 0 
	begin 
		select @mm = RTRIM(convert(varchar(2),convert(bigint,substring(@hex, 3, 1)))) 
		select @dd = RTRIM(convert(varchar(2),convert(bigint,substring(@hex, 7, 1)))) 
		select @hh = RTRIM(convert(varchar(2),convert(bigint,substring(@hex, 9, 1)))) 
		select @mi = RTRIM(convert(varchar(2),convert(bigint,substring(@hex, 11, 1)))) 
		select @ss = RTRIM(convert(varchar(2),convert(bigint,substring(@hex, 13, 1)))) 
		select @date = datepart(year,getdate())
		select @date = @date + '-' + REPLICATE('0', 2 - LEN(@mm)) + @mm + '-' + REPLICATE('0', 2 - LEN(@dd)) + @dd + ' ' 
		select @time = REPLICATE('0', 2 - LEN(@hh)) + @hh + ':' + REPLICATE('0', 2 - LEN(@mi)) + @mi + ':' + REPLICATE('0', 2 - LEN(@ss)) + @ss 
	end 
	
	if (@eventclass is not null) 
	begin 
		select 
			'eventclass' = 
			case 
				when @eventclass in ('41', '45') 
					and charindex (' (', convert(varchar(100),substring(@sqltext, 1, 100))) = 0 
					and (charindex ('EXEC ', upper(convert(varchar(100),substring(@sqltext, 1, 100)))) > 0 
					or charindex ('EXECUTE ', upper(convert(varchar(100),substring(@sqltext, 1, 100)))) > 0) 
						then 43 
				else @eventclass 
			end,
			'duration(ms)' = cast(@duration as bigint),
			'completion time' = dateadd(mi,datediff(mi,getdate(),getutcdate()),cast(@date + ' ' + @time as datetime)),
			'DBName' = db_name(@DBID),
			'nt username' = RTRIM(@ntdomainname) + '\' + @ntusername, 
			'host name' = @hostname, 
			'app name' = @appname, 
			'sql user name' = @sqlname, 
			'reads' = cast(isnull(@reads,0) as bigint), 
			'writes' = cast(isnull(@writes,0) as bigint), 
			'cpu' = cast(isnull(@cpu,0) as bigint), 
			'sql 1' = substring(@sqltext, 1, 3060),
			'spid' = @spid,
			'DatabaseID' = @DBID

		select @appname = ''
		select @counter = @counter + 1 
	end 

	if @counter < 10000 
		goto Loop 
end 
