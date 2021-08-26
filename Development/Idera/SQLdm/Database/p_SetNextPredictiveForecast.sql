if (object_id('p_SetNextPredictiveForecast') is not null)
begin
drop procedure p_SetNextPredictiveForecast 
end
go
create procedure p_SetNextPredictiveForecast
	@NextForecast datetime
as
begin
	if exists(select Name from [RepositoryInfo] where Name = 'PredictiveAnalyticsForecast')
	begin
		update RepositoryInfo set Character_Value = convert(nvarchar(100), @NextForecast, 121) where Name = 'PredictiveAnalyticsForecast'		
	end
	else
	begin
		insert into RepositoryInfo (Name, Character_Value) values ( 'PredictiveAnalyticsForecast', convert(nvarchar(100), @NextForecast, 121))
	end
end