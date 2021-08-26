--------------------------------------------------------------------------------
--  Batch: Reindex
--  Tables: sysobjects
--  Variables: 
--		[0] - Target Database
--		[1] - Table ID
--		[2] - Index Name
--------------------------------------------------------------------------------
use [{0}]

declare @qualified_table_name nvarchar(500), @command nvarchar(max), @id bigint, @type int, @indexid bigint, @cmptlevel int
set @id = {1}

select 
	@qualified_table_name = quotename(schema_name(uid)) + '.' + quotename(name) 
from
	sysobjects
where 
	id = @id

select @cmptlevel = compatibility_level from master.sys.databases where database_id = db_id()


if (len(@qualified_table_name) > 0)
begin


    SELECT @command = 'ALTER INDEX '

	if len('{2}') > 0 
		begin

			select @type = type, @indexid = index_id from sys.indexes where object_id = @id and name = '{2}'

			if @indexid is null
			begin		
				select @command = null
			end
			else
			begin
				select @command = @command + '{2} on ' + @qualified_table_name + ' REBUILD '
				-- Check for Enterprise Edition
				if lower(cast(serverproperty('edition') as nvarchar(50))) like '%enterprise edition%'
				   or lower(cast(serverproperty('edition') as nvarchar(50))) like '%developer edition%'
				begin
					-- Check for XML index
					if (@type <> 3)
						begin
							-- Check for long columns for clustered index
							if (@type = 1) 
								begin
									if not exists(select * from sys.columns where object_id = @id 
									and 
									(max_length < 0 
									or system_type_id in
									(select system_type_id from sys.types where name in ('image', 'text', 'ntext', 'xml'))
									))
									select @command = @command + 'WITH (ONLINE = ON) '
								end
							else
								begin
									-- Check for long columns for non-clustered index
									if not exists(select * from 
										sys.index_columns ic
										left join sys.columns c
										on ic.object_id = c.object_id
										left join sys.types t
										on c.system_type_id = t.system_type_id 
										where 
										ic.object_id = @id 
										and ic.index_id = @indexid
										and 
										(t.name in ('image', 'text', 'ntext', 'xml')
										or t.max_length < 0))
									select @command = @command + 'WITH (ONLINE = ON) '
								end
						end
				end
			end
		end
	else
		begin
			select @command = @command + 'ALL on ' + @qualified_table_name + ' REBUILD '
	
			-- Check for Enterprise Edition
			if lower(cast(serverproperty('edition') as nvarchar(50))) like '%enterprise edition%'
				or lower(cast(serverproperty('edition') as nvarchar(50))) like '%developer edition%'
			begin
				if not exists(select * from sys.xml_indexes where object_id = @id)
				begin
					if not exists(select * from sys.columns where object_id = @id 
						and 
						(max_length < 0 
						or system_type_id in
						(select system_type_id from sys.types where name in ('image', 'text', 'ntext', 'xml'))
						))
					select @command = @command + 'WITH (ONLINE = ON) '
				end
			end
		end

	exec (@command)

	select @qualified_table_name

	if (@cmptlevel > 80)
	begin
		declare @parmDef nvarchar(1000)
	
		if (@indexid is null)
		begin
			select @indexid = index_id from sys.indexes where index_id = 1 and object_id = @id
		end
		
		select @parmDef = '@idIN bigint, @indexidIN bigint'
		select @command = 'select sum(avg_fragmentation_in_percent * page_count) / nullif(sum(page_count),0) from master.sys.dm_db_index_physical_stats(db_id(), @idIN, @indexidIN, null, ''limited'') group by object_id'
		exec sp_executesql @command, @parmDef,@idIN = @id, @indexidIN = @indexid 
	
	end
	else
	begin
		create table #showcontig (
		   ObjectName sysname,
		   ObjectId bigint,
		   IndexName sysname,
		   IndexId bigint,
		   Lvl int,
		   CountPages int,
		   CountRows int,
		   MinRecordSize int,
		   MaxRecordSize int,
		   AvgRecordSize int,
		   ForwardedRecords int,
		   Extents int,
		   ExtentSwitches int,
		   AvgFreeBytes int,
		   AvgPageDensity int,
		   ScanDensity float,
		   BestCount int,
		   ActualCount int,
		   LogicalFragmentation float,
		   ExtentFragmentation float)

		if (len('{2}') > 0) and (@indexid is not null)

			set @command = 'dbcc showcontig (''' + replace(@qualified_table_name,char(39),char(39)+char(39)) + ''', ''{2}'') with tableresults'
		else
			set @command = 'dbcc showcontig (''' + replace(@qualified_table_name,char(39),char(39)+char(39)) + ''') with tableresults'

		insert into #showcontig
			exec(@command)
		select LogicalFragmentation from #showcontig
		drop table #showcontig
	end


end




