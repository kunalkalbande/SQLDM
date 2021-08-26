if (object_id('[p_GetBaselineMetaData]') is not null)
begin
drop procedure p_GetBaselineMetaData
end
go

create procedure [dbo].p_GetBaselineMetaData
as
begin
	declare @err int

select 
	 [ItemID]
	,[Name]
	,[Description]
	,[Category]
	,[Unit]
	,[Format]
	,[NullFormat]
	,[MetricID]
	,[StatisticTable]
	,[MetricValue]
	,[Decimals]
	,[LLimit]
	,[ULimit]
	,[Scale]
	from
		[BaselineMetaData]
	where [Name] is not null
union
select 
	CCD.Metric
	,MI.Name
	,MI.Description
	,MI.Category
	,NULL
	,'{0:G;0} - {1:G;0} {2}'
	,'0 {2}'
	,CCD.Metric
	,'@CustomCounters S left outer join [CustomCounterStatistics] CCS 
		on S.SQLServerID = CCS.SQLServerID and S.Metric = CCS.MetricID' 
--	,'[CustomCounterMap] S left outer join [CustomCounterStatistics] CCS 
--		on S.SQLServerID = CCS.SQLServerID and S.Metric = CCS.MetricID'
	,case when CCD.CalculationType = 0 then 'RawValue' else 'DeltaValue' end
	,2
	,NULL
	,NULL
	,Convert(dec(31,9),CCD.Scale)
	from 
		CustomCounterDefinition CCD, MetricInfo MI
	where  CCD.Metric = MI.Metric 
		   and CCD.Enabled = 1
	order by [StatisticTable],[ItemID]

	select @err = @@ERROR

	return @err 
end