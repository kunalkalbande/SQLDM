if (object_id('p_GetVMConfigReport') is not null)
begin
drop procedure [p_GetVMConfigReport]
end
go


create procedure [dbo].[p_GetVMConfigReport]
				@ServerID int
as
begin
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

select top 1 
	a.InstanceName as [SqlInstanceName],
	b.VMName as [VmName], 
	b.DomainName as [VmDomainName], 
	b.NumCPUs as [vCPUs],
	b.MemSize as [VmMemSize],
	case when b.CPULimit < 0 then 'Unlimited' else cast(b.CPULimit as nvarchar(max)) end as [VmCpuLimit],
	b.CPUReserve as [VmCpuReserve],
	case when b.MemLimit < 0 then 'Unlimited' else CAST(b.MemLimit/1024 as nvarchar(max)) end as [VmMemLimit],
	b.MemReserve/1024 as [VmMemReserve],
	c.HostName as [EsxHostName],
	case c.[Status] 
		when 0 then 'Powered On'
		when 1 then 'Powered Off'
		when 2 then 'Standby'
		else 'Unknown'
	end as [EsxStatus],
	case b.[VMHeartBeat] 
		when 0 then 'Powered Off'
		when 1 then 'Powered On'
		when 2 then 'Suspended'
		else 'Unknown'
	end as [VmStatus],
	c.NumCPUCores as[ESXCPUCores],
	c.CPUMHz as [EsxCpuMhz],
	c.MemorySize/1024 as [EsxMemSize],
	c.NumNICs as [EsxNumNics],
	c.BootTime as [EsxBootTime],
	d.ServerType as [ServerType]
from 
	MonitoredSQLServers a 
	left join VMConfigData b 
		on a.SQLServerID = b.SQLServerID 
	left join ESXConfigData c 
		on a.SQLServerID = c.SQLServerID
	left join VirtualHostServers d 
		on a.VHostID = d.VHostID 
where 
	a.SQLServerID = @ServerID
order by 
	b.UTCCollectionDateTime desc
	
end