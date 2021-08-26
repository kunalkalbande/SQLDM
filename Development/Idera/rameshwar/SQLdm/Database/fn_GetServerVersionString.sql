if (object_id('fn_GetServerVersionString') is not null)
begin
drop function fn_GetServerVersionString
end
go
create function fn_GetServerVersionString(
	@InputVersionString nvarchar(50)) 
	returns nvarchar(50)
begin

	declare 
		@ReturnVersionString nvarchar(50),
		@Major int,
		@Minor int,
		@Build int,
		@Rev int
	
	select @InputVersionString = rtrim(ltrim(@InputVersionString))

	select @Major = 0
	select @Minor = charindex('.',@InputVersionString,@Major + 1)
	select @Build = charindex('.',@InputVersionString,@Minor + 1)
	select @Rev = charindex('.',@InputVersionString,@Build + 1)

	select @Major = cast(substring(@InputVersionString,@Major,@Minor) as int)
	select @Minor = cast(substring(@InputVersionString,@Minor+1,@Build - @Minor - 1) as int)
	select @Build = case when @Rev > 0 then cast(substring(@InputVersionString,@Build+1,@Rev - @Build - 1) as int) 
					else cast(substring(@InputVersionString,@Build+1,len(@InputVersionString) - @Build) as int) end
	select @Rev = case when @Rev > 0 then cast(substring(@InputVersionString,@Rev+1,len(@InputVersionString) - @Rev) as int) 
					else 0 end
	
	select @ReturnVersionString = 'SQL Server ' +
		case 
			when @Major = 7 then '7 '
			when @Major = 8 then '2000 '
			when @Major = 9 then '2005 '
			when @Major = 10 and @Minor = 50 then '2008 R2 '
			when @Major = 10 then '2008 '
			when @Major = 11 then '2012 '
			when @Major = 12 then '2014 ' --added by Gaurav Karwal for SQL 2014 SQLdm 8.6
			when @Major = 13 then '2016 ' --added by Srishti Purohit for SQL 2016 SQLdm 10.1
			when @Major = 14 then '2017 ' --added for SQL 2017 SQLdm 10.3
			when @Major = 15 then '2019 ' --adeed for SQL 2019 SQLdm 10.5
			when @Major > 15 then cast(@Major as nvarchar(4)) + ' '
		end +
		case 
			when @Major = 7 and @Build = 623 then '' 
			when @Major = 7 and @Build > 623 and @Build < 699 then 'RTM+'
			when @Major = 7 and @Build = 699 then 'SP1' 
			when @Major = 7 and @Build > 699 and @Build < 842 then 'SP1+'
			when @Major = 7 and @Build = 842 then 'SP2' 
			when @Major = 7 and @Build > 842 and @Build < 961 then 'SP2+'
			when @Major = 7 and @Build = 961 then 'SP3' 
			when @Major = 7 and @Build > 961 and @Build < 1063 then 'SP3+'
			when @Major = 7 and @Build = 1063 then 'SP4' 
			when @Major = 7 and @Build > 1063 then 'SP4+'
			when @Major = 8 and @Build = 194 then '' 
			when @Major = 8 and @Build > 194 and @Build < 384 then 'RTM+'
			when @Major = 8 and @Build = 384 then 'SP1' 
			when @Major = 8 and @Build > 384 and @Build < 534 then 'SP1+'
			when @Major = 8 and @Build = 534 then 'SP2' 
			when @Major = 8 and @Build > 534 and @Build < 760 then 'SP2+'
			when @Major = 8 and @Build = 760 then 'SP3' 
			when @Major = 8 and @Build > 760 and @Build < 2039 then 'SP3+'
			when @Major = 8 and @Build = 2039 then 'SP4' 
			when @Major = 8 and @Build > 2039 then 'SP4+'
			when @Major = 9 and @Build = 1314 then 'CTP' 
			when @Major = 9 and @Build > 1314 and @Build < 1399 then 'CTP+'
			when @Major = 9 and @Build = 1399 then '' 
			when @Major = 9 and @Build > 1399 and @Build < 2047 then 'RTM+'
			when @Major = 9 and @Build = 2047 then 'SP1' 
			when @Major = 9 and @Build > 2047 and @Build < 3042 then 'SP1+'
			when @Major = 9 and @Build = 3042 then 'SP2' 
			when @Major = 9 and @Build > 3042 and @Build < 4035 then 'SP2+'
			when @Major = 9 and @Build = 4035 then 'SP3' 
			when @Major = 9 and @Build = 4035 then 'SP3'
			when @Major = 9 and @Build > 4035 and @Build < 5000 then 'SP3+'
			when @Major = 9 and @Build = 5000 then 'SP4' 
			when @Major = 9 and @Build > 5000 then 'SP4+'
			when @Major = 10 and @Minor = 50 and @Build = 1092 then 'CTP'
			when @Major = 10 and @Minor = 50 and @Build between 1093 and 1599 then 'CTP+'
			when @Major = 10 and @Minor = 50 and @Build = 1600 then ''
			when @Major = 10 and @Minor = 50 and @Build between 1601 and 2499 then 'RTM+'
			when @Major = 10 and @Minor = 50 and @Build = 2500 then 'SP1'
			when @Major = 10 and @Minor = 50 and @Build = 4000 then 'SP2'
			when @Major = 10 and @Minor = 50 and @Build > 4000 and @Build < 6000 then 'SP2+' -- added by Biresh Mishra
			when @Major = 10 and @Minor = 50 and @Build = 6000 then 'SP3' -- added by Biresh Mishra
			when @Major = 10 and @Minor = 50 and @Build > 6000 then 'SP3+' -- added by Biresh Mishra
			when @Major = 10 and @Build = 1600 then '' 
			when @Major = 10 and @Build > 1600 and @Build < 2531 then 'RTM+'
			when @Major = 10 and @Build = 2531 then 'SP1' 
			when @Major = 10 and @Build > 2531 and @Build < 4000 then 'SP1+'
			when @Major = 10 and @Build = 4000 then 'SP2'
			when @Major = 10 and @Build > 4000 and @Build < 5500 then 'SP2+' 
			when @Major = 10 and @Build = 5500 then 'SP3'
			when @Major = 10 and @Build > 5500 and @Build < 6000 then 'SP3+' -- added by Biresh Mishra
			when @Major = 10 and @Build = 6000 then 'SP4' -- added by Biresh Mishra
			when @Major = 10 and @Build > 6000 then 'SP4+' -- added by Biresh Mishra
			when @Major = 11 and @Build = 1750 then 'RC0'
			when @Major = 11 and @Build > 1750 and @Build < 2100 then 'RC0+'						
			when @Major = 11 and @Build = 2100 then ''
			when @Major = 11 and @Build > 2100 and @Build < 3000 then 'RTM+' -- added by Biresh Mishra
			when @Major = 11 and @Build = 3000 then 'SP1' -- added by Biresh Mishra
			when @Major = 11 and @Build > 3000 and @Build < 5058 then 'SP1+' -- added by Biresh Mishra
			when @Major = 11 and @Build = 5058 then 'SP2' -- added by Biresh Mishra
			when @Major = 11 and @Build > 5058 and @Build < 6020 then 'SP2+' -- added by Biresh Mishra
			when @Major = 11 and @Build = 6020 then 'SP3' -- added by Biresh Mishra
			when @Major = 11 and @Build > 6020 then 'SP3+' -- added by Biresh Mishra
			--added by Gaurav Karal for SQL 2014
			when @Major = 12 and @Build >= 2000 and @Build < 2342 then 'RTM'
			when @Major = 12 and @Build >= 2342 and @Build < 2370 then 'RTM CU1'
			when @Major = 12 and @Build >= 2370 and @Build < 2402 then 'RTM CU2'
			when @Major = 12 and @Build >= 2402 and @Build < 2430 then 'RTM CU3'
			when @Major = 12 and @Build >= 2430 and @Build < 2456 then 'RTM CU4'
			when @Major = 12 and @Build >= 2456 and @Build < 2480 then 'RTM CU5'
			when @Major = 12 and @Build >= 2480 and @Build < 2495 then 'RTM CU6'
			when @Major = 12 and @Build >= 2495 and @Build < 2546 then 'RTM CU7'
			when @Major = 12 and @Build >= 2546 and @Build < 2553 then 'RTM CU8'
			when @Major = 12 and @Build >= 2553 and @Build < 2556 then 'RTM CU9'
			when @Major = 12 and @Build >= 2556 and @Build < 2560 then 'RTM CU10'
			when @Major = 12 and @Build >= 2560 and @Build < 2564 then 'RTM CU11'
			when @Major = 12 and @Build >= 2564 and @Build < 2568 then 'RTM CU12'
			when @Major = 12 and @Build >= 2568 and @Build < 2569 then 'RTM CU13'
			when @Major = 12 and @Build >= 2569 and @Build < 4100 then 'RTM CU14'
			when @Major = 12 and @Build >= 4100 and @Build < 4416 then 'SP1'
			when @Major = 12 and @Build >= 4416 and @Build < 4422 then 'SP1 CU1'
			when @Major = 12 and @Build >= 4422 and @Build < 4427 then 'SP1 CU2'
			when @Major = 12 and @Build >= 4427 and @Build < 4436 then 'SP1 CU3'
			when @Major = 12 and @Build >= 4436 and @Build < 4438 then 'SP1 CU4'
			when @Major = 12 and @Build >= 4438 and @Build < 4449 then 'SP1 CU5'
			when @Major = 12 and @Build >= 4449 and @Build < 4459 then 'SP1 CU6'
			when @Major = 12 and @Build >= 4459 and @Build < 4468 then 'SP1 CU7'
			when @Major = 12 and @Build >= 4468 and @Build < 4474 then 'SP1 CU8'
			when @Major = 12 and @Build >= 4474 and @Build < 5000 then 'SP1 CU9'
			when @Major = 12 and @Build >= 5000 and @Build < 5511 then 'SP2'
			when @Major = 12 and @Build = 5511 then 'SP2 CU1'
			when @Major = 12 and @Build = 5522 then 'SP2 CU2'
			when @Major = 12 and @Build >= 5538 and @Build < 5540 then 'SP2 CU3'
			when @Major = 12 and @Build >= 5540 and @Build < 5546 then 'SP2 CU4'
			when @Major = 12 and @Build >= 5546 and @Build < 5552 then 'SP2 CU5'
			when @Major = 12 and @Build >= 5552 and @Build < 5556 then 'SP2 CU6'
			when @Major = 12 and @Build >= 5556 and @Build < 5557 then 'SP2 CU7'
			when @Major = 12 and @Build >= 5557 and @Build < 5563 then 'SP2 CU8'
			when @Major = 12 and @Build >= 5563 and @Build < 5571 then 'SP2 CU9'
			when @Major = 12 and @Build >= 5571 and @Build < 5579 then 'SP2 CU10'
			when @Major = 12 and @Build >= 5579 and @Build < 5589 then 'SP2 CU11'
			when @Major = 12 and @Build >= 5589 and @Build < 5590 then 'SP2 CU12'
			when @Major = 12 and @Build >= 5590 and @Build < 5600 then 'SP2 CU13'
			when @Major = 12 and @Build >= 5600 and @Build < 5605 then 'SP2 CU14'
			when @Major = 12 and @Build >= 5605 and @Build < 5626 then 'SP2 CU15'
			when @Major = 12 and @Build >= 5626 and @Build < 5632 then 'SP2 CU16'
			when @Major = 12 and @Build >= 5632 and @Build < 5687 then 'SP2 CU17'
			when @Major = 12 and @Build = 5687 then 'SP2 CU18'
			when @Major = 12 and @Build > 5687 then 'SP2 CU18+'
			--[SQLdm10.1] added by Srishti Purohit for SQL 2016
			when @Major = 13 and @Build >= 1601 and @Build < 2149 then 'RTM'
			when @Major = 13 and @Build >= 2149 and @Build < 2164 then 'RTM CU1'
			when @Major = 13 and @Build >= 2164 and @Build < 2186 then 'RTM CU2'
			when @Major = 13 and @Build >= 2186 and @Build < 4001 then 'RTM CU3'
			when @Major = 13 and @Build >= 4001 and @Build < 5026 then 'SP1'
			when @Major = 13 and @Build = 5026 then 'SP2'
			--[SQLdm10.3] added for SQL 2017
			when @Major = 14 and @Build >= 1000 and @Build < 3006 then 'RTM'
			when @Major = 14 and @Build >= 3006 and @Build < 3008 then 'RTM CU1'
			when @Major = 14 and @Build >= 3008 and @Build < 3015 then 'RTM CU2'
			when @Major = 14 and @Build >= 3015 and @Build < 3022 then 'RTM CU3'
			when @Major = 14 and @Build = 3022  then 'RTM CU4'
			when @Major = 14 and @Build >= 3023 and @Build < 3025 then 'RTM CU5'
			when @Major = 14 and @Build = 3025 then 'RTM CU6'
			when @Major = 14 and @Build >= 3026 and @Build < 3029 then 'RTM CU7'
			when @Major = 14 and @Build = 3029 then 'RTM CU8'
			when @Major = 14 and @Build >= 3030 and @Build < 3037 then 'RTM CU9'
			when @Major = 14 and @Build = 3037 then 'RTM CU10'
			when @Major = 14 and @Build >= 3038 and @Build < 3045 then 'RTM CU11'
			when @Major = 14 and @Build >= 3045 and @Build < 3048 then 'RTM CU12'
			when @Major = 14 and @Build >= 3048 and @Build < 3076 then 'RTM CU13'
			when @Major = 14 and @Build >= 3076 and @Build < 3162 then 'RTM CU14'
			when @Major = 14 and @Build >= 3162 and @Build < 3223 then 'RTM CU15'
			when @Major = 14 and @Build >= 3223 and @Build < 3238 then 'RTM CU16'
			when @Major = 14 and @Build >= 3238 and @Build < 3257 then 'RTM CU17'
			when @Major = 14 and @Build >= 3257 and @Build < 3281 then 'RTM CU18'
			when @Major = 14 and @Build >= 3281 then 'RTM CU19+'
			--[SQLdm10.5] added for SQL 2019
			when @Major = 15 and @Build >= 2000 and @Build < 4003 then 'RTM'
			when @Major = 15 and @Build >= 4003 and @Build < 4013 then 'RTM CU1'
			when @Major = 15 and @Build >= 4013 and @Build < 4023 then 'RTM CU2'
			when @Major = 15 and @Build >= 4023 and @Build < 4033 then 'RTM CU3'
			when @Major = 15 and @Build >= 4033 then 'RTM CU4+'
		end +
		' (' + @InputVersionString + ')'

	return @ReturnVersionString

end

