if (object_id('p_GetNextPredictiveForecast') is not null)
begin
drop procedure p_GetNextPredictiveForecast 
end
go
create procedure p_GetNextPredictiveForecast
as
begin
		select Character_Value from RepositoryInfo where Name = 'PredictiveAnalyticsForecast'
end