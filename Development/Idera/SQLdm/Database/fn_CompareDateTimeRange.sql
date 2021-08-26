if (object_id('fn_CompareDateTimeRange') is not null)
begin
drop function fn_CompareDateTimeRange
end
go
create function fn_CompareDateTimeRange(
	@InputDateTime datetime,
	@StartDate datetime,
	@EndDate datetime,
	@StartTime datetime,
	@EndTime datetime,
	@Sun bit,
	@Mon bit,
	@Tue bit,
	@Wed bit,
	@Thu bit,
	@Fri bit,
	@Sat bit) RETURNS BIT
begin
	declare @ret bit
	declare @idt datetime
	declare @id datetime
	declare @it datetime
	declare @stt datetime
	declare @ett datetime
   
    --SQLdm 10.1 (pulkit puri)--start
	--Applying 360 to baseline 
	 declare @IsMidnightCrossed bit
	set @IsMidnightCrossed=0
     --SQLdm 10.1 (pulkit puri)--end
	select @idt = @InputDateTime
	select @id = dateadd(dd,0, datediff(dd,0,@idt))

	if (@StartDate is not null)
	begin
		if (@id < dateadd(dd,0, datediff(dd,0,@StartDate)))
			return 0
	end		

	if (@EndDate is not null)
	begin
		if (@id > dateadd(dd,0, datediff(dd,0,@EndDate)))
			return 0
	end	
	
	if (@StartTime is not null and @EndTime is not null)
	begin
		set @stt = dateadd(dd, -datediff(dd,0,@StartTime), @StartTime) 		
		set @ett = dateadd(dd, -datediff(dd,0,@EndTime), @EndTime)

		if (@stt <> @ett)
		begin
			set @it = @idt - @id
			if (@stt <= @ett)
			begin
				if (@it < @stt or @it > @ett)
					return 0
			end
			else
			begin
				if (@it > @ett and @it < @stt)
					return 0
			end
		end
	end	 
			
	if (@ret is null)
	begin
	--SQLdm 10.1 (Pulkit Puri) Taking previous day if mid night crossed
	if ((@ett<=@stt)and(@it<=@ett))
	begin
			set @IsMidnightCrossed=1
	end
		set @ret = case (datepart(dw,@idt)-@IsMidnightCrossed)
		    when 0 then @Sat--day shift from sunday-1 will lead to 0
			when 1 then @Sun
			when 2 then @Mon
			when 3 then @Tue
			when 4 then @Wed
			when 5 then @Thu
			when 6 then @Fri
		    when 7 then @Sat
			else 1
		end
	end
	if (@ret is null)
		set @ret = 1

	return @ret;
end
