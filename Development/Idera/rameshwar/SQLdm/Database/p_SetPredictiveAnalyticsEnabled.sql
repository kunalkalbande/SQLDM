if (object_id('p_SetPredictiveAnalyticsEnabled') is not null)
begin
drop procedure p_SetPredictiveAnalyticsEnabled 
end
go
create procedure p_SetPredictiveAnalyticsEnabled
	@PredictiveAnalyticsEnabled int
as
begin
	if exists(select Name from [RepositoryInfo] where Name = 'PredictiveAnalyticsEnabled')
	begin
		update RepositoryInfo set Internal_Value = @PredictiveAnalyticsEnabled where Name = 'PredictiveAnalyticsEnabled'
		
	end
	else
	begin
		insert into RepositoryInfo (Name,Internal_Value) values ( 'PredictiveAnalyticsEnabled',@PredictiveAnalyticsEnabled)
	end
end