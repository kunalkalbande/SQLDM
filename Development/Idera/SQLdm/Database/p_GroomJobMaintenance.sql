if (object_id('Grooming.p_GroomJobMaintenance') is not null)
begin
drop procedure Grooming.p_GroomJobMaintenance
end
go
create procedure [Grooming].[p_GroomJobMaintenance]
as
begin

set nocount on


begin try


declare @StartTime datetime
declare @TimeoutTime datetime
declare @TimeoutMinutes int
declare @run_id uniqueidentifier
declare @Sequence int
declare @RC int
declare @ErrorMessage nvarchar(2048)
declare @BlockName nvarchar(256)

select 
	@StartTime = isnull(UTCActionEndDateTime,getutcdate()),
	@run_id = isnull(RunID,newid())
from 
GroomingLog
where UTCActionEndDateTime = 
	(select 
		max (UTCActionEndDateTime) 
	from 
	GroomingLog (nolock)
	where Action = 'Started')
	
select @Sequence = max(Sequence) from GroomingLog where RunID = @run_id
	
select @TimeoutMinutes = isnull(Internal_Value,180) from RepositoryInfo where [Name] = 'GroomingMaxNumberMinutes'	

set @TimeoutTime = dateadd(mi,@TimeoutMinutes,@StartTime)

if (GetUTCDate() > @TimeoutTime)           
	raiserror (N'Timeout in %s', 11, 1,'Maintenance');

if (object_id('GroomingFragmentationMaintenance') is null)
begin
create table GroomingFragmentationMaintenance
(
rowid int identity primary key clustered,
objectid int,
tablename nvarchar(256),
indexname nvarchar(256),
indexid int,
lastdefrag datetime null,
reorgflag bit
)
end

;with DeleteList(objectid,tablename,indexname,indexid) as
(
select objectid,tablename,indexname,indexid
from GroomingFragmentationMaintenance
except
select
t.object_id,
t.name,
i.name,
i.index_id
from sys.indexes i
inner join sys.tables t
on i.object_id = t.object_id
where
i.type in (1,2)
and t.type = 'U')
delete GroomingFragmentationMaintenance
from
GroomingFragmentationMaintenance m 
inner join DeleteList d
on 
m.objectid = d.objectid
and m.tablename = d.tablename
and m.indexname = d.indexname
and m.indexid = d.indexid

set @RC = @@ROWCOUNT

exec Grooming.p_LogGroomingAction @run_id, @Sequence out,  'Grooming GroomingFragmentationMaintenance' , @RC, null


insert into GroomingFragmentationMaintenance(objectid,tablename,indexname,indexid)
select
t.object_id,
t.name,
i.name,
i.index_id
from sys.indexes i
inner join sys.tables t
on i.object_id = t.object_id
where
i.type in (1,2)
and t.type = 'U'
except
select objectid,tablename,indexname,indexid
from GroomingFragmentationMaintenance

set @RC = @@ROWCOUNT

exec Grooming.p_LogGroomingAction @run_id, @Sequence out,  'Building GroomingFragmentationMaintenance' , @RC, null

if not exists(select rowid from GroomingFragmentationMaintenance where reorgflag = 1)
begin
	update GroomingFragmentationMaintenance
	set reorgflag = 1
	where 
	rowid in(
	select top 10 f.rowid
	from 
	GroomingFragmentationMaintenance f
	inner join sys.partitions p
	on p.object_id = f.objectid
	inner join  sys.allocation_units au
	on (au.type = 2 and p.partition_id = au.container_id) OR (au.type IN (1,3) AND p.hobt_id = au.container_id) 
	cross apply Grooming.fn_GroomingFragmentation(db_id(),f.objectid,f.indexid) f2
	where
	(lastdefrag is null and f2.avg_fragmentation_in_percent > 30)
	or (datediff(d,lastdefrag,getutcdate()) > 1 and f2.avg_fragmentation_in_percent > 50)
	or (datediff(d,lastdefrag,getutcdate()) > 7 and f2.avg_fragmentation_in_percent > 30)
	group by 
	 f.rowid, f2.avg_fragmentation_in_percent
	having  
	sum(total_pages) > 100
	)
end


declare @id int, @indexname nvarchar(256)

while (1=1)
begin

	if (GetUTCDate() > @TimeoutTime)           
		raiserror (N'Timeout in %s', 11, 1,'Maintenance');
	
	begin try
		
		select top 1
			@BlockName = 'Defrag ' + tablename,
			@id = objectid, 
			@indexname = case when indexid = 1 then 'ALL' else indexname end
		from 
			GroomingFragmentationMaintenance
		where reorgflag = 1
		order by indexid asc
		
		if @id is null break;

		exec [Grooming].[p_GroomingDefrag] @id, @indexname
		
		update GroomingFragmentationMaintenance
		set lastdefrag = getutcdate(), reorgflag = 0
		where objectid = @id
		and (indexname = @indexname or @indexname = 'ALL')
		
		set @id = null
		
		exec Grooming.p_LogGroomingAction  @run_id, @Sequence out,@BlockName, 0, null

	end try
	begin catch

		set @ErrorMessage = @BlockName + ERROR_MESSAGE()
		exec Grooming.p_LogGroomingAction  @run_id, @Sequence out,@ErrorMessage, 0, null
		break;
	
	end catch		
	
end



end try --Global
begin catch
	set @ErrorMessage = ERROR_MESSAGE()
	exec Grooming.p_LogGroomingAction @run_id, @Sequence out,  @ErrorMessage, @RC, null
	
end catch

exec Grooming.p_LogGroomingAction @run_id, @Sequence out, 'Finished Maintenance', null, null

end

