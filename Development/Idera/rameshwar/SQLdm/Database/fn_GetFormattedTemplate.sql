if (object_id('fn_GetFormattedTemplate') is not null)
begin
drop function fn_GetFormattedTemplate
end
go

CREATE FUNCTION fn_GetFormattedTemplate(@template xml)
RETURNS @shredded table(Template nvarchar(1000))
AS
BEGIN

Declare @UseDefault varchar(5)
Declare @StartDateText varchar(50)
Declare @EndDateText varchar(50)
Declare @StartDate DateTime
Declare @EndDate DateTime
Declare @SelectedDays int
Declare @mon bit
Declare @tue bit
Declare @wed bit
Declare @thu bit
Declare @fri bit
Declare @sat bit
Declare @sun bit
Declare @templateString nvarchar(100)

SET @UseDefault =  @template.value('(/BaselineTemplate/UseDefault)[1]', 'varchar(5)');
SET @StartDateText =  @template.value('(/BaselineTemplate/StartDate)[1]', 'varchar(50)');
SET @EndDateText =  @template.value('(/BaselineTemplate/EndDate)[1]', 'varchar(50)');
SET @SelectedDays =  @template.value('(/BaselineTemplate/SelectedDays)[1]', 'int');

set @mon = case @SelectedDays & 4 when 4 then 1 else 0 end
set @tue = case @SelectedDays & 8 when 8 then 1 else 0 end
set @wed = case @SelectedDays & 16 when 16 then 1 else 0 end
set @thu = case @SelectedDays & 32 when 32 then 1 else 0 end
set @fri = case @SelectedDays & 64 when 64 then 1 else 0 end
set @sat = case @SelectedDays & 128 when 128 then 1 else 0 end
set @sun = case @SelectedDays & 1 when 1 then 1 else 0 end

SELECT @StartDate = CAST(LEFT(@StartDateText, 19) AS datetime)
SELECT @EndDate = CAST(LEFT(@EndDateText, 19) AS datetime)

select @templateString = case @UseDefault when 'true' then 'Last 7 Days ' else convert(nvarchar(30), @StartDate, 101) + ' to ' + convert(nvarchar(30), @EndDate, 101) + ' ' end
select @templateString = @templateString + 
		case @mon when 1 then ' Mon,' else '' end + 
		case @tue when 1 then ' Tue,' else '' end + 
		case @wed when 1 then ' Wed,' else '' end + 
		case @thu when 1 then ' Thu,' else '' end + 
		case @fri when 1 then ' Fri,' else '' end + 
		case @sat when 1 then ' Sat,' else '' end + 
		case @sun when 1 then ' Sun,' else '' end 
		
select @templateString = substring(@templateString,1,len(@templateString)-1)
select @templateString = @templateString + ' between ' + CONVERT(VARCHAR(28), @StartDate, 24) + ' and '+ CONVERT(VARCHAR(28), @EndDate, 24)

insert @shredded
select @templateString
return

END

