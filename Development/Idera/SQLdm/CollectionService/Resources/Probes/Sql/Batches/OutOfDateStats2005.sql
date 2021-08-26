----------------------------------------------------------------------------------------------
--	collect stats information for a database
--
--  The rowmodctr value in 2005/2008 will frequently help you determine when to update statistics 
--  because the behavior is reasonably close to the results of earlier versions.
--	
----------------------------------------------------------------------------------------------
select 
	[Database]=db_name(),
	[Schema]= (Select sch.name from sys.objects o inner join sys.schemas sch on o.schema_id = sch.schema_id where o.object_id = i.id), 
	[Table]=object_name(i.id), 
	[Name]=i.name,
	[RowCount]=irows.rows, 
	[ModCount]=i.rowmodctr,
	[StatsDate]=stats_date(i.id, i.indid),
	[HoursSinceUpdated]=datediff(hh,stats_date(i.id, i.indid),getdate())
	from sysindexes i
		left outer join sysindexes irows on i.id = irows.id and (irows.indid = 0 or irows.indid = 1)
	where indexproperty(i.id, i.name, 'IsStatistics') = 1
	and indexproperty(i.id, i.name, 'IsAutoStatistics') = 0 -- Don't include auto created stats
	and datediff(hh,stats_date(i.id, i.indid),getdate()) >= 24 -- Make sure the stats or over 24 hours old
	and objectproperty(i.id, 'IsMSShipped') = 0
	and irows.rows > 500
	and (i.rowmodctr > 1000 and i.rowmodctr > (irows.rows / 6))
