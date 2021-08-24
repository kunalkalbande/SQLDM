 --------------------------------------------------------------------------------
--  Batch: Replication Check
--  Tables: sysdatabases, sysobjects, sysprocesses, MSrepl_commands, 
--	MSrepl_transactions, MSsubscriptions, MSdistribution_history,
--	MSdistribution_status
--  SP: sp_replcounters, sp_helpdistributor
--------------------------------------------------------------------------------
--------------------------------------------------------------------------------
--  Batch: Replication Topology
--  Tables: sysdatabases
--------------------------------------------------------------------------------
set nocount on
declare @dbname nvarchar(255), @quotedDBName nvarchar(255), @category int, @subscribedCnt int, @publishedCnt int, @distributorCnt int
declare @distributor nvarchar(255), @DistributionDB nvarchar(255), @retcode int, 	
@checkdistributed tinyint, 	@checksubscribed tinyint, @servername nvarchar(255), @replicationState int

declare @command nvarchar(4000)

select 	@checkdistributed = 1, 	@checksubscribed = 1,	@servername = cast(serverproperty('servername')  as nvarchar(255))

select @subscribedCnt = 0, @publishedCnt = 0, @distributorCnt = 0, @replicationState = 4

declare replication_databases 
insensitive cursor for 
	select 
		 name, category 
	from 
		master..sysdatabases d(nolock) 
	where 
		--(category & 1 = 1 or category & 2 = 2 or category & 16 = 16 ) and 
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
		order by category asc
for read only 
open replication_databases 
fetch replication_databases into @dbname, @category 
while @@fetch_status = 0 
begin 
	select @quotedDBName = quotename(@dbname)
	if ((@category & 1 = 1) or (@category & 4 = 4)) -- publishing snapshot or transactional or merge
	begin
		select @publishedCnt = @publishedCnt + 1
		
		select @command = ''
		select 'published_database', @dbname
		
		if (@category & 1 = 1)
		begin
		select @command = N'use ' + @quotedDBName + ' select ''articles'' = COUNT(ssubs.artid),''publisher'' = serverproperty(''servername''),
		N''' + replace(@dbname,'''','''''') + ''' as publisheddb, p.name as publication, 
		ss.srvname, dest_db, ssubs.status, ssubs.sync_type, ssubs.subscription_type 
		from syssubscriptions  ssubs 
		inner join sysarticles a on a.artid = ssubs.artid
		inner join syspublications p on p.pubid = a.pubid
		inner join master..sysservers ss on ss.srvid = ssubs.srvid where ssubs.srvid >= 0 
		group by ss.srvname, dest_db, p.name, ssubs.status, ssubs.sync_type, ssubs.subscription_type'
		end

		if (@category & 4 = 4)
		begin
		exec(N' if (select count(*) from ' + @quotedDBName + '..sysobjects where name in (''sysmergepublications'',''sysmergesubscriptions'')) <> 2 select ''no merge found''')
			if @@ROWCOUNT = 0
			begin
				if(len(@command) > 0) 
				begin
					select @command = @command + N' union'
				end else
					select @command = @command + N' use ' + @quotedDBName

				select @command = @command + N' select ''articles'' = count(a.name), p.publisher, ''publisheddb'' = p.publisher_db,
				''publication'' = p.name,
				''srvname'' = s.subscriber_server,
				''dest_db'' = s.db_name, s.status, s.sync_type, s.subscription_type
				from sysmergearticles a 
				inner join sysmergepublications p on a.pubid = p.pubid
				inner join sysmergesubscriptions s on s.pubid = p.pubid
				where publisher <> s.subscriber_server and p.publisher_db <> s.db_name
				group by p.publisher, p.publisher_db, s.subscriber_server, s.db_name, p.name, s.status, s.sync_type, s.subscription_type'
			end
		end		
		exec(@command) 
	end
	if ((@category & 2 = 2) or (@category & 8 = 8)) -- subscribing to snapshot, transactional or merge
	begin
		select @subscribedCnt = @subscribedCnt + 1
		select 'subscribed_database', @quotedDBName 
		exec(N'exec ' + @quotedDBName + '..sp_MSenumsubscriptions ''both''') 
	end
	if (@category & 16 = 16) -- distributor
	begin
		select @distributorCnt = @distributorCnt + 1
		exec(N'if (select count(*) from ' +  @quotedDBName + '..sysobjects where name in (''MSrepl_transactions'',''MSsubscriptions'',''MSdistribution_history'', ''MSrepl_commands'')) <> 4 select ''missing tables'' ') 
		if @@rowcount = 0 
		begin 
			exec(N'select ''subscription latency'', isnull(max(datediff(second, entry_time, getdate())),0) '
				+ 'from (select t.entry_time, agent_id, t.xact_seqno '
				+ 'from ' +  @quotedDBName + '..MSrepl_commands rc '
				+ 'inner join ' +  @quotedDBName + '..MSrepl_transactions t on t.xact_seqno = rc.xact_seqno '
				+ 'inner join ' +  @quotedDBName + '..MSsubscriptions s on s.publisher_database_id = t.publisher_database_id and s.publisher_database_id = rc.publisher_database_id '
				+ 'where s.subscriber_id >= 0 and s.status = 2 and rc.article_id = s.article_id '
				+ 'and ((rc.xact_seqno >= s.subscription_seqno and (rc.type & 0x80000000) <> 0x80000000) or rc.xact_seqno = s.subscription_seqno)) as unsubscribed '
				+ 'inner join '
				+ '(select isnull(max(xact_seqno), 0) as seq_no, agent_id from ' +  @quotedDBName + '..MSdistribution_history '
				+ 'group by agent_id) as maxSeqPerAgent on unsubscribed.agent_id = maxSeqPerAgent.agent_id where unsubscribed.xact_seqno > maxSeqPerAgent.seq_no')

			select 'distribution_database', @dbname 
		
			select @command = N'select  count(s.article_id) as articles, ps.srvname as PublisherInstance,p.publisher_db,'
				+ ' ss.srvname as SubscriberInstance,s.subscriber_db,publication,p.description as publicationDescription,'
				+ ' sum(cast(DelivCmdsInDistDB as bigint)) as ''subscribed'', sum(cast(UndelivCmdsInDistDB as bigint)) as ''non-subscribed'', '
				+ ' p.publication_type as ''replication_type'' '
				+ ' from ' + @quotedDBName + '..MSpublications p left join ' + @quotedDBName + '..MSsubscriptions s on p.publication_id = s.publication_id '
				+ ' left join ' + @quotedDBName + '..MSdistribution_status ds on ds.agent_id = s.agent_id and ds.article_id = s.article_id '
				+ ' left join master..sysservers ps on ps.srvid = p.publisher_id '
				+ ' left join master..sysservers ss on ss.srvid = s.subscriber_id '
				+ ' where s.subscriber_id >= 0 '
				+ ' group by ps.srvname, p.publisher_db, ss.srvname, s.subscriber_db, publication,p.description, p.publication_type '
			select @command = @command + N' union select  count(s.article_id) as articles, ps.srvname as PublisherInstance,p.publisher_db, '
				+ ' isnull(ss.srvname,'''') as SubscriberInstance, subscriber_db = case s.subscriber_db when N''virtual'' then '''' else s.subscriber_db end,'
				+ ' publication,p.description as publicationDescription, '
				+ ' sum(cast(DelivCmdsInDistDB as bigint)) as ''subscribed'', '
				+ ' sum(cast(UndelivCmdsInDistDB as bigint)) as ''non-subscribed'', '
				+ ' p.publication_type as ''replication_type'''
				+ ' from ' + @quotedDBName + '..MSpublications p '
				+ ' left join ' + @quotedDBName + '..MSsubscriptions s on p.publication_id = s.publication_id  '
				+ ' left join ' + @quotedDBName + '..MSdistribution_status ds on ds.agent_id = s.agent_id '
				+ ' and ds.article_id = s.article_id  '
				+ ' left join master..sysservers ps on ps.srvid = p.publisher_id '
				+ ' left join master..sysservers ss on ss.srvid = s.subscriber_id  '
				+ ' where not exists(select * from ' + @quotedDBName + '..MSpublications pp1 inner join ' + @quotedDBName + '..MSsubscriptions ss1 on pp1.publication_id = ss1.publication_id and pp1.publication = p.publication and  ss1.subscriber_id >= 0)'
				+ ' and p.publication_type <> 2  and s.subscriber_id = -1 group by ps.srvname, p.publisher_db, ss.srvname, s.subscriber_db, publication,p.description, p.publication_type '

			exec(N'if (select count(*) from ' + @quotedDBName + '..sysobjects where name in (''MSpublications'',''MSmerge_subscriptions'')) <> 2 select ''no merge found''')
			if @@ROWCOUNT = 0
			begin
				select @command = @command 
				+ N' union select count(*) as articles, p.srvname as PublisherInstance, ms.publisher_db, '
				+ 's.srvname as SubscriberInstance, ms.subscriber_db, sp.publication, '
				+ 'sp.description, null as ''subscribed'', '
				+ 'null as ''non-subscribed'', sp.publication_type as ''replication_type'' '
				+ 'from ' + @quotedDBName + '..MSmerge_subscriptions ms '
				+ 'inner join ' + @quotedDBName + '..MSpublications sp on sp.publication_id = ms.publication_id '
				+ 'left join master..sysservers p on p.srvid = ms.publisher_id '
				+ 'left join master..sysservers s on s.srvid = ms.subscriber_id '
				+ 'group by p.srvname, ms.publisher_db, s.srvname, ms.subscriber_db, sp.publication, '
				+ 'sp.description, sp.publication_type '
			end

			select @command = @command + N' option (maxdop 1)'
			exec(@command)
 		end 		
	end

	fetch replication_databases into @dbname, @category
end 
Close replication_databases 
deallocate replication_databases 

--if there are published databases on this server then look for counters
if @publishedCnt > 0
begin
	select 'replcounters'
	exec master..sp_replcounters
	select 'sp_helpdistributor'
	exec sp_helpdistributor 
end

-- This returns the replication states that are required by the RplicationState enum
-- I am not sure at this point if all of these are required but better to populate it for now
if @distributorCnt > 0 and @publishedCnt > 0
	select @replicationState = 0
if @publishedCnt = 0 and @subscribedCnt = 0 and @distributorCnt = 0	
	select @replicationState = 1
if (@publishedCnt > 0 or @subscribedCnt > 0) and @distributorCnt = 0
	select @replicationState = 2
if @distributorCnt > 0 and @publishedCnt = 0
	select @replicationState = 3
select 'replicationstate', @replicationState