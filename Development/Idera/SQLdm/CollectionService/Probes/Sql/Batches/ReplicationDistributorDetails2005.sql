--------------------------------------------------------------------------------
--  Batch: Replication Distribution Details 2005
--  Tables: sys.servers, MSdistribution_status, MSpublications,MSrepl_transactions, 
--          MSrepl_commands, MSsubscriptions, MSdistribution_history
--  Variables: 
--		[0] - Distribution Database name
--		[1] - Publication Server name
--		[2] - Publication Name
--      [3] - Subscriber db
--      [4] - Subscriber Instance
--------------------------------------------------------------------------------
set nocount on

use [{0}]

IF object_id('tempdb..#tempLatency') IS NOT NULL
BEGIN
   DROP TABLE #tempLatency
END

select 'Number of Articles' = count(*),s.publisher_database_id,s.publisher_id, 
'Publisher' = pub.name + '.' + s.publisher_db, 
'Distributor' = (select name from sys.servers where server_id = 0),
'Subscriber' = sub.name + '.' + s.subscriber_db,
'subscription type' = case s.subscription_type when 0 then 'push' when 1 then 'pull' when 2 then 'push' end,
'sync type' = case s.sync_type when 1 then 'automatic' when 2 then 'no synchronisation' end,
s.publication_id, publ.publication,
 s.subscriber_id,s.agent_id, 
'subscribed' = sum(cast(ds.DelivCmdsInDistDB as bigint)), 
'non-subscribed' = sum(cast(ds.UndelivCmdsInDistDB as bigint)),
'entry_time' = max(dblatency.entry_time),
'SampleTime' = GetUTCDate() into #tempLatency
 from MSsubscriptions s 
 left join sys.servers pub on s.publisher_id = pub.server_id
 left join sys.servers sub on s.subscriber_id = sub.server_id
 left outer join MSdistribution_status ds on s.agent_id = ds.agent_id and ds.article_id = s.article_id
 inner join MSpublications publ on publ.publication_id = s.publication_id
left outer join (
select maxSeqPerAgent.agent_id, t.publisher_database_id,s.subscriber_id,  max(t.entry_time) as entry_time
from MSrepl_transactions t inner join MSrepl_commands c on t.xact_seqno = c.xact_seqno
	inner join MSsubscriptions s on s.publisher_database_id = c.publisher_database_id and s.article_id = c.article_id
	inner join(select isnull(max(xact_seqno), 0) as seq_no, agent_id from MSdistribution_history group by agent_id) as maxSeqPerAgent on maxSeqPerAgent.agent_id = s.agent_id
where s.status >= 0 and s.subscriber_id > 0
and c.xact_seqno > maxSeqPerAgent.seq_no
	and ((c.xact_seqno >= s.subscription_seqno
			and (c.type & 0x80000000) <> 0x80000000) --snapshot
		or c.xact_seqno = s.subscription_seqno)
group by t.publisher_database_id, s.subscriber_id, maxSeqPerAgent.agent_id
) as dblatency on dblatency.publisher_database_id = s.publisher_database_id 
and dblatency.subscriber_id = s.subscriber_id  
and dblatency.agent_id = s.agent_id
 where s.status >= 0
 and ((s.subscriber_id >= 0 and exists(select agent_id from MSsubscriptions s1 where s1.publisher_db = s.publisher_db and s1.subscriber_id >= 0) and s.subscriber_db = N'{3}' and sub.name = N'{4}')
 or (s.subscriber_id = -1  and not exists(select agent_id from MSsubscriptions s1 where s1.publisher_db = s.publisher_db and s1.subscriber_id >= 0)))
and lower(pub.name) = lower(N'{1}') and publ.publication = N'{2}'
group by s.publisher_database_id,s.publisher_id,s.publisher_db,s.publication_id,publ.publication,s.subscriber_id,s.subscriber_db,s.agent_id,pub.name,sub.name,
s.subscription_type, s.sync_type

select [Number of Articles],
publisher_database_id,
publisher_id,
Publisher,
Distributor,
Subscriber,
[subscription type],
[sync type],
publication_id,
publication,
subscriber_id,
agent_id,
subscribed,
[non-subscribed],
'subscription latency' = isnull(datediff(second,entry_time,getdate()),0), 
SampleTime
from #tempLatency

drop table #tempLatency