if (object_id('p_GetPredictiveModelsCount') is not null)
begin
drop procedure p_GetPredictiveModelsCount 
end
go
create procedure p_GetPredictiveModelsCount
as
begin
		select COUNT(SQLServerID) from PredictiveModels
end