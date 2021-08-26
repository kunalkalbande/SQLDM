declare @id bigint, @indexid bigint, @type int, @version int, @online bit

select @online = 0
select @id = object_id(@ObjectName)
select @type = type, @indexid = index_id from sys.indexes where object_id = @id and name = @IndexName
select @version = cast(substring(cast(serverproperty('productversion') as nvarchar), 1, charindex('.', cast(serverproperty('productversion') as nvarchar)) -1) as int)

if @indexid is not null
begin
	-- Check for Enterprise Edition
	if lower(cast(serverproperty('edition') as nvarchar(50))) like '%enterprise edition%'
		or lower(cast(serverproperty('edition') as nvarchar(50))) like '%enterprise evaluation edition%'
		or lower(cast(serverproperty('edition') as nvarchar(50))) like '%developer edition%'
	begin
		-- Check for XML index
		if (@type <> 3)
		begin
			-- Check for long columns for clustered index
			if (@type = 1) 
			begin
				-- For 2012 and newer, many indexes with blob data support rebuild online 
				if @version > 10 
				begin 
					if not exists(select * from sys.columns where object_id = @id and 
									(system_type_id in 
										(select system_type_id from sys.types 
											where name in ('image', 'text', 'ntext'))))
						select @online = 1
				end
				else -- For 2008R2 and older don't do online rebuild for text, ntext, image, varchar(max), nvarchar(max), varbinary(max), xml, or large CLR type
				begin 
					if not exists(select * from sys.columns where object_id = @id and 
										(max_length < 0 or system_type_id in
										(select system_type_id from sys.types where name in ('image', 'text', 'ntext', 'xml'))
										))
										select @online = 1
				end
			end
			else
			begin
				-- For 2012 and newer, many indexes with blob data support rebuild online 
				if @version > 10 
				begin 
					-- Check for long columns for non-clustered index
					if not exists(select * from 
								sys.index_columns ic
								left join sys.columns c
								on ic.object_id = c.object_id
								left join sys.types t
								on c.system_type_id = t.system_type_id
								and ic.column_id = c.column_id
								where ic.object_id = @id 
								and ic.index_id = @indexid
								and (t.name in ('image', 'text', 'ntext')))
								select @online = 1
				end
				else -- For 2008R2 and older don't do online rebuild for text, ntext, image, varchar(max), nvarchar(max), varbinary(max), xml, or large CLR type
				begin 
					-- Check for long columns for non-clustered index
					if not exists(select * from 
								sys.index_columns ic
								left join sys.columns c
								on ic.object_id = c.object_id
								left join sys.types t
								on c.system_type_id = t.system_type_id
								and ic.column_id = c.column_id
								where ic.object_id = @id 
								and ic.index_id = @indexid
								and (t.name in ('image', 'text', 'ntext', 'xml')
								or c.max_length < 0))
								select @online = 1
				end
			end
		end
	end
end
select @online