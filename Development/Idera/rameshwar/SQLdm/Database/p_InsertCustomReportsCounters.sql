if (object_id('[p_InsertCustomReportsCounters]') is not null)
begin
drop procedure [p_InsertCustomReportsCounters]
end
go

create proc p_InsertCustomReportsCounters (
@ReportName nvarchar(255),
@GraphNumber int,
@CounterShortDescription nvarchar(255),
@CounterName nvarchar(255),
@Aggregation int,
@Source int
)
as
begin
declare @ID int

select @ID = ID from CustomReports where reportName = @ReportName

if (select count(*) from CustomReportsCounters where ID = @ID and GraphNumber = @GraphNumber) > 0
	begin
		select @GraphNumber
		Update CustomReportsCounters 
		set CounterShortDescription = @CounterShortDescription, 
		CounterName = @CounterName, 
		Aggregation = @Aggregation,
		Source = @Source
		where ID = @ID and GraphNumber = @GraphNumber
	end
else
	begin
		insert into CustomReportsCounters values (@ID, 
		@GraphNumber, 
		@CounterShortDescription, 
		replace(@CounterName,' ','_'), 
		@Aggregation,
		@Source)
	end
end

