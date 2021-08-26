if (object_id('p_GetNextPredictiveModelRebuild') is not null)
begin
drop procedure p_GetNextPredictiveModelRebuild 
end
go
create procedure p_GetNextPredictiveModelRebuild
as
begin
		select Character_Value from RepositoryInfo where Name = 'PredictiveAnalyticsRebuild'
end