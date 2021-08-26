--------------------------------------------------------------------------------
--  Batch: Oldest Open Transaction 2000
--  Tables: master..sysdatabases, master..sysprocesses
--  Variables: [0] - Filter statements for where clause
--------------------------------------------------------------------------------

declare 
@spid int, 
@MaxBatchDate datetime, 
@CurrentBatchDate datetime,
@command nvarchar(2048),
@mode smallint, 
@dbname nvarchar(255),
@filtered bit


 
select @MaxBatchDate = '2190-01-01' 
if (select isnull(object_id('tempdb..#Open_Tran'),0)) = 0 
Create table #Open_Tran (RecID varchar(35), RecValue varchar(255)) 
else truncate table #Open_Tran 
if (select isnull(object_id('tempdb..#AllOpenTrans'),0)) = 0 
Create table #AllOpenTrans (RecID varchar(35), RecValue varchar(255), DBName varchar(35)) 
else truncate table #AllOpenTrans 

declare read_db_status insensitive cursor for 
	select 
		name, 
		mode
	from 
		master..sysdatabases d (nolock) 
	where
		lower(name) <> 'mssqlsystemresource'
		and has_dbaccess (name) = 1 
		and mode = 0 
		and isnull(databaseproperty(name, 'IsInLoad'),0) = 0 
		and isnull(databaseproperty(name, 'IsSuspect'),0) = 0 
		and isnull(databaseproperty(name, 'IsInRecovery'),0) = 0 
		and isnull(databaseproperty(name, 'IsNotRecovered'),0) = 0 
		and isnull(databaseproperty(name, 'IsOffline'),0) = 0 
		and isnull(databaseproperty(name, 'IsShutDown'),0) = 0 
		and (
			isnull(databaseproperty(name, 'IsSingleUser'),0) = 0 
			or ( 
				isnull(databaseproperty(name, 'IsSingleUser'),0) = 1 
				and not exists 
				(select * from master..sysprocesses p where dbid = d.dbid and p.spid <> @@spid)
				and not exists
				(select * from master..syslockinfo l where rsc_dbid = d.dbid and l.req_spid <> @@spid)
				)
			)
		and status & 32 <> 32 
		and status & 64 <> 64 
		and status & 128 <> 128 
		and status & 256 <> 256 
		and status & 512 <> 512 
for read only 
open read_db_status 
fetch read_db_status into @dbname, @mode 
while @@fetch_status = 0 
begin 
	
	set @command = 'DBCC OPENTRAN(' + quotename(@dbname,'[')   + ') WITH TABLERESULTS'
	
	insert into #Open_Tran 
		execute (@command)

	select 
		@CurrentBatchDate =  convert(datetime, RecValue) 
	from 
		#Open_Tran 
	where RecID = 'OLDACT_STARTTIME' 

	select @CurrentBatchDate = isnull(@CurrentBatchDate,'2190-01-01') 

	select 
		@spid = convert(int, replace(RecValue,'s',''))
	from 
		#Open_Tran 
	where RecID = 'OLDACT_SPID' 

	if (@CurrentBatchDate < @MaxBatchDate ) and 
	(	
		select 
			count(*) 
		from 
			master..sysprocesses a
		where
			spid = @spid
			{0}
	) > 0
	begin 
		select @MaxBatchDate = @CurrentBatchDate 
		truncate table #AllOpenTrans 
		insert into #AllOpenTrans 
			select 
				RecID, 
				RecValue, 
				@dbname 
			from 
				#Open_Tran 
	End 

	truncate table #Open_Tran 

	fetch read_db_status into @dbname , @mode 
end 
close read_db_status 
deallocate read_db_status 

select 
	@spid = convert(int, replace(RecValue,'s','')), 
	@dbname = DBName 
from 
	#AllOpenTrans 
where RecID = 'OLDACT_SPID' 

if @@rowcount > 0 
begin 
if (select isnull(object_id('tempdb..#inputbuffer_open_transaction'), 0)) = 0 
	begin 
		create table #inputbuffer_open_transaction (spid smallint default -1, EventType nvarchar(260), Parameters int,EventInfo nvarchar(260)) 
	end
	set @command = 'dbcc inputbuffer(' + convert(nvarchar(5),@spid) + ')'
	insert into #inputbuffer_open_transaction (EventType,Parameters,EventInfo)
		exec (@command)
	update #inputbuffer_open_transaction set spid = @spid where spid = -1

	 select top 1
		cast(a.spid as int),
		rtrim(convert(varchar(255),a.loginame)),
		rtrim(a.hostname),
		rtrim(a.status),
		rtrim(a.program_name),
		rtrim(a.cmd),
		rtrim(db_name(a.dbid)),
		a.cpu,
		a.memusage,
		a.physical_io,
		cast(a.blocked as int),
		(select count(b.spid) from master..sysprocesses b where b.blocked = a.spid),
		dateadd(mi,datediff(mi,getdate(),getutcdate()),a.login_time),
		case
		 when a.spid < 5
		   then dateadd(mi,datediff(mi,getdate(),getutcdate()),getdate())
		 else
			dateadd(mi,datediff(mi,getdate(),getutcdate()),a.last_batch)
		end ,
		open_tran,
		rtrim(net_address),
		rtrim(net_library),
		convert(bigint,waittime),
		a.ecid,
		rtrim(a.lastwaittype),
		rtrim(a.waitresource),
		EventInfo,
		isnull(datediff(s,@MaxBatchDate, getdate()),0),
		dateadd(mi,datediff(mi,getdate(),getutcdate()),@MaxBatchDate)
	 from
	   master..sysprocesses a (nolock)
	   left join #inputbuffer_open_transaction i
	   on a.spid = i.spid
	 where
		a.spid = @spid 

	drop table #inputbuffer_open_transaction
end 



