if (object_id('p_DiskDetailsReport') is not null)
begin
drop procedure p_DiskDetailsReport
end
go
CREATE PROCEDURE [dbo].[p_DiskDetailsReport]
		@ServerID int,
		@UTCStart DateTime,
		@UTCEnd DateTime,
		@UTCOffset int,
		@Interval tinyint,
		@DriveName nvarchar(256) = null,
		@Average bit
AS
BEGIN			

declare @ReplaceString NVARCHAR(MAX)
declare @ParmDefinition NVARCHAR(MAX)
declare @QueryString NVARCHAR(MAX)
if (@Average=1)
	begin		
		set @ReplaceString='avg'
	end
	else
	begin
		set @ReplaceString='max'
	end

set @QueryString= 'SELECT	
		count(*) as [Records],
		dbo.fn_RoundDateTime(@Interval, max(dateadd(mi, @UTCOffset, dd.[UTCCollectionDateTime]))) as [LastCollectioninInterval],
		dd.DriveName,
		{chartType}(dd.AverageDiskMillisecondsPerRead) as AverageDiskMillisecondsPerRead,
		{chartType}(dd.AverageDiskMillisecondsPerTransfer) as AverageDiskMillisecondsPerTransfer,
		{chartType}(dd.AverageDiskMillisecondsPerWrite) as AverageDiskMillisecondsPerWrite,
		{chartType}(dd.DiskReadsPerSecond) as DiskReadsPerSecond,
		{chartType}(dd.DiskTransfersPerSecond) as DiskTransfersPerSecond,
		{chartType}(dd.DiskWritesPerSecond) as DiskWritesPerSecond,
		ms.InstanceName
 FROM DiskDrives dd (nolock)
	JOIN MonitoredSQLServers ms (nolock) on dd.SQLServerID = ms.SQLServerID
	WHERE dbo.fn_RoundDateTime(@Interval, dd.[UTCCollectionDateTime]) between @UTCStart and @UTCEnd
		and @ServerID = dd.SQLServerID
		and dd.DriveName = coalesce(@DriveName, dd.DriveName)
		and dd.DriveName <> ''No Drives Configured''
	GROUP BY
			ms.InstanceName,
			dd.DriveName
			-- Always group by year at the least
			,datepart(yy, dateadd(mi, @UTCOffset, dd.[UTCCollectionDateTime]))
			-- Group by all intervals greater than or equal to the selected interval
			,case when @Interval <= 3 then datepart(mm,dateadd(mi, @UTCOffset, dd.[UTCCollectionDateTime])) else datepart(yy,dateadd(mi, @UTCOffset, dd.[UTCCollectionDateTime])) end
			,case when @Interval <= 2 then datepart(dd,dateadd(mi, @UTCOffset, dd.[UTCCollectionDateTime])) else datepart(yy,dateadd(mi, @UTCOffset, dd.[UTCCollectionDateTime])) end
			,case when @Interval <= 1 then datepart(hh,dateadd(mi, @UTCOffset, dd.[UTCCollectionDateTime])) else datepart(yy,dateadd(mi, @UTCOffset, dd.[UTCCollectionDateTime])) end
			,case when @Interval =  0 then datepart(mi,dateadd(mi, @UTCOffset, dd.[UTCCollectionDateTime])) else datepart(yy,dateadd(mi, @UTCOffset, dd.[UTCCollectionDateTime])) end
		ORDER BY
			dd.DriveName
			,max(dd.[UTCCollectionDateTime])'
		
		SELECT @QueryString = REPLACE(@QueryString, '{chartType}', @ReplaceString)
		
		SET @ParmDefinition = N'
		@ServerID int,
		@UTCStart DateTime,
		@UTCEnd DateTime,
		@UTCOffset int,
		@Interval tinyint,
		@DriveName nvarchar(256)';

EXECUTE sp_executesql @QueryString, 
					  @ParmDefinition, 
					  @ServerID = @ServerID, 
					  @UTCOffset = @UTCOffset, 
					  @UTCStart = @UTCStart,
					  @UTCEnd = @UTCEnd, 
					  @Interval = @Interval,
					  @DriveName = @DriveName
					  
END
 
