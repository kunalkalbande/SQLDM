if (object_id('fn_CompareDaysSelection') is not null)
begin
drop function fn_CompareDaysSelection
end
go
create function fn_CompareDaysSelection(
	@daysScheduled int,
	@daysTocompare int, 
	@DayDifferenceAfterUTCConversion DECIMAL) RETURNS BIT
BEGIN
	DECLARE @result bit 
	SET @result = 0
	declare @Sun bit, @Mon bit, @Tue bit, @Wed bit, @Thu bit, @Fri bit, @Sat bit
	declare @SunTocompare bit, @MonTocompare bit, @TueTocompare bit, @WedTocompare bit, @ThuTocompare bit, @FriTocompare bit, @SatTocompare bit
	DECLARE @Days int
	SET @Days = 0 
	declare @bitpointer int
	set @bitpointer = 3

	if (@DayDifferenceAfterUTCConversion >= 1)
	BEGIN
		if ((@daysScheduled & 1) = 1) SET @Days = @Days | 128;
        if ((@daysScheduled & 4) = 4) SET @Days = @Days | 1;
		while(@bitpointer <= 7)
			BEGIN
				if ((@daysScheduled & Power(2, @bitpointer)) = Power(2, @bitpointer))
					BEGIN
						set @Days = @Days | Power(2, (@bitpointer + 7) % 8);
					END
					SET @bitpointer = @bitpointer + 1
			END
	END
	else if (@DayDifferenceAfterUTCConversion <= -1)
	BEGIN
	if ((@daysScheduled & 1) = 1) SET @Days = @Days | 4;
    SET @bitpointer = 2
	WHILE(@bitpointer <= 7)
    BEGIN
		if ((@daysScheduled & Power(2, @bitpointer)) = Power(2, @bitpointer))
		BEGIN
			SET @Days = @Days | Power(2, (@bitpointer + 1) % 8);
		END
		SET @bitpointer = @bitpointer + 1
	END
	END
	ELSE
	BEGIN
		SET @Days = @daysScheduled
	END

	set @Sun = case when (@Days & 1) > 0 then 1 else 0 end
	set @Mon = case when (@Days & 4) > 0 then 1 else 0 end
	set @Tue = case when (@Days & 8) > 0 then 1 else 0 end
	set @Wed = case when (@Days & 16) > 0 then 1 else 0 end
	set @Thu = case when (@Days & 32) > 0 then 1 else 0 end
	set @Fri = case when (@Days & 64) > 0 then 1 else 0 end
	set @Sat = case when (@Days & 128) > 0 then 1 else 0 end

	set @SunTocompare = case when (@daysTocompare = 1) then 1 else 0 end
	set @MonTocompare = case when (@daysTocompare = 2) then 1 else 0 end
	set @TueTocompare = case when (@daysTocompare = 3) then 1 else 0 end
	set @WedTocompare = case when (@daysTocompare = 4) then 1 else 0 end
	set @ThuTocompare = case when (@daysTocompare = 5) then 1 else 0 end
	set @FriTocompare = case when (@daysTocompare = 6) then 1 else 0 end
	set @SatTocompare = case when (@daysTocompare = 7) then 1 else 0 end
		
	SET @result =  case 
	when (@Sun = @SunTocompare and @Sun = 1) then @Sun
	when (@Mon = @MonTocompare and @Mon = 1) then @Mon
	when (@Tue = @TueTocompare and @Tue = 1) then @Tue 
	when (@Wed = @WedTocompare and @Wed = 1) then @Wed
	when (@Thu = @ThuTocompare and @Thu = 1) then @Thu 
	when (@Fri = @FriTocompare and @Fri = 1) then @Fri 
	when (@Sat = @SatTocompare and @Sat = 1) then @Sat
	else 0 end

	return @result
END

