if (object_id('[p_GetServerStandardDeviations]') is not null)
begin
drop procedure [p_GetServerStandardDeviations]
end
go
create procedure [dbo].[p_GetServerStandardDeviations]
	@SQLServerID int = null
as
begin
select
	min(UTCCollectionDateTime) as MinUTCCollectionDateTime,
	max(UTCCollectionDateTime) as MaxUTCCollectionDateTime,
	convert(dec(5,1),avg(PacketsReceived)) as [Average Packets Received],
	convert(dec(5,1),stdev(PacketsReceived)) as [Standard Deviation of Packets Received],
	count(PacketsReceived) as [Packets Received Sample Size],
	convert(dec(5,1),avg(PacketsSent)) as [Average Packets Sent],
	convert(dec(5,1),stdev(PacketsSent)) as [Standard Deviation of Packets Sent],
	count(PacketsSent) as [Packets Sent Sample Size]
from
	ServerStatistics
where
	SQLServerID = coalesce(@SQLServerID,SQLServerID)
	and UTCCollectionDateTime >= dateadd(dd,-1,getutcdate())
group by
	SQLServerID

end




