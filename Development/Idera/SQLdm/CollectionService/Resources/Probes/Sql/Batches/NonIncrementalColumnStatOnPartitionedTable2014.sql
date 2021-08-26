----------------------------------------------------------------------------------------------
-- //SQLDm 10.0 - Srishti Purohit - New Recommendations - SDR-I23 - Adding New Batch
--
--	
----------------------------------------------------------------------------------------------
select serverproperty('Edition');

With Parts
As (Select object_id
	From sys.partitions
	Where index_id In (0, 1)
	Group By object_id
	Having count(partition_number) > 1)
Select DB_NAME() As DatabaseName, OBJECT_NAME(S.object_id) As TableName,
	S.name As StatsName
From sys.stats S
Inner Join Parts P On S.objecT_id = P.object_id
Where user_created = 1
And is_incremental = 0
And Not Exists (Select 1 From sys.indexes
		Where object_id = S.object_id
		And name = S.name);

-- For Optimize and Undo script support
With Parts
As (Select object_id
	From sys.partitions
	Where index_id In (0, 1)
	Group By object_id
	Having count(partition_number) > 1)
Select OBJECT_NAME(S.object_id) As TableName,
	S.name As StatsName,
	DropCreateSQL = N'If Exists (Select 1 From sys.stats' + char(10) +
		char(9) + N'Where object_id = ' + Cast(S.object_id as nvarchar(35)) + char(10) +
		char(9) + N'And name = N''' + S.name + N''')' + char(10) +
		N'Drop Statistics ' +
		quotename(OBJECT_SCHEMA_NAME(S.object_id)) + N'.' +
		quotename(OBJECT_NAME(S.object_id)) + N'.' +
		quotename(S.name) + N';' +
		char(10) + N'Go' + char(10) + char(10) +
		N'Create Statistics ' + quotename(S.name) + N' on ' +
		quotename(OBJECT_SCHEMA_NAME(S.object_id)) + N'.' +
		quotename(OBJECT_NAME(S.object_id)) + N'(' + dt.StatsColumns + N')' +
		Case S.has_filter When 1 Then char(10) + N'Where ' + S.filter_definition
			Else N''
			End +
		char(10) + N'With FullScan, Incremental = On;',
	RollBackSQL = N'If Exists (Select 1 From sys.stats' + char(10) +
		char(9) + N'Where object_id = ' + Cast(S.object_id as nvarchar(35)) + char(10) +
		char(9) + N'And name = N''' + S.name + N''')' + char(10) +
		N'Drop Statistics ' +
		quotename(OBJECT_SCHEMA_NAME(S.object_id)) + N'.' +
		quotename(OBJECT_NAME(S.object_id)) + N'.' +
		quotename(S.name) + N';' +
		char(10) + N'Go' + char(10) + char(10) +
		N'Create Statistics ' + quotename(S.name) + N' on ' +
		quotename(OBJECT_SCHEMA_NAME(S.object_id)) + N'.' +
		quotename(OBJECT_NAME(S.object_id)) + N'(' + dt.StatsColumns + N')' +
		Case S.has_filter When 1 Then char(10) + N'Where ' + S.filter_definition
			Else N''
			End +
		char(10) + N'With FullScan, Incremental = Off' +
		Case S.no_recompute When 1 Then N', NoRecompute;'
			Else N';'
			End
From sys.stats S 
Inner Join Parts P On S.objecT_id = P.object_id
--Inner Join sys.stats_columns SC On SC.object_id = S.object_id And SC.stats_id = SC.stats_id
Inner Join (Select Distinct SC.object_id, SC.stats_id,
		StatsColumns = Replace((Select C.name As 'data()'
			From sys.columns C
			Inner Join sys.stats_columns SC2 On C.object_id = SC2.object_id
				And C.column_id = SC2.column_id
				And SC2.stats_id = SC.stats_id
			Where C.object_id = SC.object_id
			For XML Path('')), ' ', ',')
	From sys.stats_columns SC --On C.object_id = IC.object_id And C.column_id = IC.column_id
	Where OBJECTPROPERTYEX(SC.object_id, 'IsMSShipped') = 0) As dt
	On dt.stats_id = S.stats_id
	And dt.object_id = S.object_id
Where user_created = 1
And is_incremental = 0
And Not Exists (Select 1 From sys.indexes
				Where object_id = S.object_id
				And name = S.name);