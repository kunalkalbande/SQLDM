if (object_id('Grooming.p_GroomingDefrag') is not null)
begin
drop procedure Grooming.p_GroomingDefrag
end
go
create procedure [Grooming].[p_GroomingDefrag]
(
@id int,
@indexname nvarchar(256)
)
as
begin

declare @qualified_table_name nvarchar(500), @command nvarchar(max), @type int, @indexid bigint, @cmptlevel int, @isonline bit

set @isonline = 0


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

	if @indexname <> 'ALL'
		begin

			select @type = type, @indexid = index_id from sys.indexes where object_id = @id and name = @indexname

			if @indexid is null
			begin		
				select @command = null
			end
			else
			begin
				select @command = @command + quotename(@indexname) + ' on ' + @qualified_table_name + ' REBUILD '
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
									set @isonline = 1
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
									set @isonline = 1
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
					set @isonline = 1
				end
			end
		end
end		

if (@isonline = 0)
begin
	if (@indexname) = 'ALL'
	begin
		set @command = 'dbcc indexdefrag (0, ' + rtrim(cast(@id as nvarchar(50))) + ') WITH NO_INFOMSGS	'
	end
	else
	begin
		set @command = 'dbcc indexdefrag (0, ' + rtrim(cast(@id as nvarchar(50))) + ', ' + quotename(@indexname,'''') + ') WITH NO_INFOMSGS	'
	end
end

exec (@command)

end