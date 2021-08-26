if (object_id('[p_GetTableFragmentation]') is not null)
begin
drop procedure [p_GetTableFragmentation]
end
go
create procedure [dbo].[p_GetTableFragmentation]
	@SQLServerID int = null,
	@DatabaseName nvarchar(510) = null
as
begin

declare 
	@DatabaseID int

select 
	@DatabaseID = DatabaseID
from 
	SQLServerDatabaseNames
where 
	SQLServerID = @SQLServerID
	and DatabaseName = @DatabaseName

select 
	isnull(SchemaName,'') + '.' + isnull(TableName,''),
	Fragmentation = LogicalFragmentation,
	UTCCollectionDateTime,
	SystemTable
from 
	TableReorganization tr
	left join SQLServerTableNames tn on tr.TableID = tn.TableID
where 
	tn.DatabaseID = isnull(@DatabaseID,0)
	and UTCCollectionDateTime = 
	(
		select max(UTCCollectionDateTime) 
		from TableReorganization tr2
		where 
			tr.TableID = tr2.TableID
	)
end

