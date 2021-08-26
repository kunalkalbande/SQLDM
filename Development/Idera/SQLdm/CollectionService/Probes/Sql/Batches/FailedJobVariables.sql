--------------------------------------------------------------------------------
--  Batch: Failed Job Variables
--
--	Variables:	[0] - Instanceid from previous refresh
--  [1] - Treat any failure as critical
--  [2] - Jobs to clear
--  [3] - Flag to prevent first-time alerts
--
-- SQLdmJjRj9pSdyOG85wJ6vVUDVK01sWrvSEy3bQPeCFgkveDvpawqXebqXRh8EE7tkcrNXepaFFn2MQ0vWDDgU4PLgOAFQaxLIgkP0sxaETBdh74x2YUc5u6AZQD1vxC4lYw1gt7jjBdUG2OSxr6Ecbhd1uGVtcDpPTbvbLck42shrtOIStIDoNbMufBVdmvERyeqM7NjEZhvSQLdm
--------------------------------------------------------------------------------
if (object_id('tempdb..FailedJobVars') is null)  
begin
	create table tempdb..FailedJobVars(name nvarchar(25), numValue int, hostname sysname)
end
else
	delete from tempdb..FailedJobVars where hostname = host_name()

if (object_id('tempdb..FailedJobSteps') is null)  
begin
	create table tempdb..FailedJobSteps(guid uniqueidentifier, hostname sysname, stepid int)
end

if (object_id('tempdb..FailedJobGuids') is not null)
begin
	insert into tempdb..FailedJobSteps (guid, hostname) (select guid, hostname from tempdb..FailedJobGuids)
	drop table tempdb..FailedJobGuids
end

delete from tempdb..FailedJobSteps where hostname = host_name()

insert into tempdb..FailedJobVars(name, numValue, hostname)
	select '@previnstance', {0}, host_name()	union select '@treatfailureascritical', {1}, host_name()	union select '@flag',{3}, host_name()

insert into tempdb..FailedJobSteps(guid,hostname, stepid)
	select '00000000-0000-0000-0000-000000000000', host_name(), 0
	{2}


