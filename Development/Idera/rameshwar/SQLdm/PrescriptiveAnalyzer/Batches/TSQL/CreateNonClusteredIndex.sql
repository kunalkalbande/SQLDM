
create nonclustered index {IndexName}				-- index name
                    on {TableName}					-- table name
                    {Columns}						-- (col,...n) include (col,...n)
                    with statistics_only = 0  ON [PRIMARY]
                    --with statistics_only = -1
 select indexproperty(object_id({TableNameSafe}), {IndexNameSafe}, 'IndexID')
