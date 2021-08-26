if (object_id('p_GetPredictiveAnalyticsEnabled') is not null)
begin
drop procedure p_GetPredictiveAnalyticsEnabled 
end
go
create procedure p_GetPredictiveAnalyticsEnabled
	@PredictiveAnalyticsEnabled int output
as
begin
	if exists(select Name from [RepositoryInfo] where Name = 'PredictiveAnalyticsEnabled')
	begin
		select @PredictiveAnalyticsEnabled = Internal_Value from RepositoryInfo where Name = 'PredictiveAnalyticsEnabled'
	end
	else
	begin
		select @PredictiveAnalyticsEnabled = 0
	end
end