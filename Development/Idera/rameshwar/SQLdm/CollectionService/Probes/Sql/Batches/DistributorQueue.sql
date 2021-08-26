--------------------------------------------------------------------------------
--  Batch: Distributor Queue
--  Tables: MSrepl_commands, MSsubscriptions, MSrepl_transactions, sysservers
--  Variables: 
--		[0] - Distribution Database
--		[1] - Timespan
--		[2] - Publisher name
--		[3] - Publication name
--      [4] - Subscriber Instance
--      [5] - Subscriberdb
--------------------------------------------------------------------------------
use master
-- Ensure the distribution database exists
if (select db_id(N'{0}')) > 0
begin
	use [{0}]
	declare @StartDate datetime
	
	set @StartDate = case when {1} < 2147483648 then dateadd(ss, -{1}, getdate()) else dateadd(mi, -{1} / 60, getdate()) end

	select 
	subscriber = m2.srvname + '.' + s.subscriber_db, 
	publisher = m1.srvname, 
	s.publisher_db, 
	dateadd(mi,datediff(mi,getdate(),getutcdate()),t.entry_time),
	rc.command
	from 
		MSrepl_commands rc , 
		MSsubscriptions s, 
		MSpublications p,
		MSrepl_transactions t, 
		master..sysservers m1, 
		master..sysservers m2 
	where 
		lower(m1.srvname) = lower(N'{2}')
		and t.entry_time <= @StartDate
		and s.status = 2 
		and p.publication_id = s.publication_id
		and m1.srvid = s.publisher_id 
		and m2.srvid = s.subscriber_id 
		and t.xact_seqno = rc.xact_seqno 
		and t.publisher_database_id = rc.publisher_database_id 
		and rc.publisher_database_id = s.publisher_database_id 
		and rc.xact_seqno > 
		(
			select 
				isnull(max(h.xact_seqno),0) 
			from 
				MSdistribution_history h 
			where 
				h.agent_id = s.agent_id) 
		and rc.article_id = s.article_id 
		and (
			(rc.xact_seqno >= s.subscription_seqno and (rc.type & 0x80000000) <> 0x80000000) 
			or rc.xact_seqno = s.subscription_seqno
			) 
		and s.subscriber_id >= 0 
		and p.publication = N'{3}' 
		and m2.srvname = N'{4}'
		and s.subscriber_db = N'{5}'		
		option(FORCE ORDER)
end
else
	select 'Distributor does not exist'

