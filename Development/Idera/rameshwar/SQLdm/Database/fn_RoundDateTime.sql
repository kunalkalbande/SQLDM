if (object_id('fn_RoundDateTime') is not null)
begin
drop function fn_RoundDateTime
end
go
create function fn_RoundDateTime(
	@Interval int,
	@Time DateTime	
)
	returns DateTime
begin
	select @Time = Case
		When @Interval = 1 Then dateadd(hour, datediff(hour, 0, @Time), 0)
		When @Interval = 2 Then dateadd(day, datediff(day, 0, @Time), 0)
		When @Interval >= 3 Then dateadd(month, datediff(month, 0, @Time), 0) -- SQLdm9.1 (Ankit Srivastava) -- Rally Defect DE43423 -- added equal operator as well
		-- Always discard seconds and milliseconds.
		Else dateadd(minute, datediff(minute, 0, @Time), 0)
	End
	
	return @Time
end
go
