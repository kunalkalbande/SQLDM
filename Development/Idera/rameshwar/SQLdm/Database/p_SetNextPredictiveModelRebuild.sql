if (object_id('p_SetNextPredictiveModelRebuild') is not null)
begin
drop procedure p_SetNextPredictiveModelRebuild 
end
go
create procedure p_SetNextPredictiveModelRebuild
	@NextRebuild datetime
as
begin
	if exists(select Name from [RepositoryInfo] where Name = 'PredictiveAnalyticsRebuild')
	begin
		update RepositoryInfo set Character_Value = convert(nvarchar(100), @NextRebuild, 121) where Name = 'PredictiveAnalyticsRebuild'		
	end
	else
	begin
		insert into RepositoryInfo (Name, Character_Value) values ( 'PredictiveAnalyticsRebuild', convert(nvarchar(100), @NextRebuild, 121) )
	end
end