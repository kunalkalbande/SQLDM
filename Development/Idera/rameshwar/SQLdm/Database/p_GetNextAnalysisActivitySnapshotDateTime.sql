if (object_id('p_GetNextAnalysisActivitySnapshotDateTime') is not null)
begin
drop procedure [p_GetNextAnalysisActivitySnapshotDateTime]
end
go
create procedure [dbo].[p_GetNextAnalysisActivitySnapshotDateTime]
	@SQLServerID int,
	@UTCSnapshotCollectionDateTime datetime
	--@RefreshType int = 0
as
begin
set transaction isolation level read uncommitted
declare @err int

select 
	min(UTCAnalysisCompleteTime)
from 
	PrescriptiveAnalysis
where 
	UTCAnalysisCompleteTime > @UTCSnapshotCollectionDateTime
	and [SQLServerID] = @SQLServerID
	AND RecommendationCount > 0
	--and isnull(RefreshType, 0) = @RefreshType

select @err = @@error
return @err
end