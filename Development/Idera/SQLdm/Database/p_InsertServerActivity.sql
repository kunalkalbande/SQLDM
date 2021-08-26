if (object_id('p_InsertServerActivity') is not null)
begin
drop procedure p_InsertServerActivity
end
go
create procedure p_InsertServerActivity
	@SQLServerID int,
	@UTCCollectionDateTime datetime,
	@StateOverview nvarchar(max) = null,
	@SystemProcesses image = null,
	@SessionList image = null,
	@LockStatistics image = null,
	@LockList image = null,
	@RefreshType int = 0
as
begin

declare @err int

if (exists(select SQLServerID from [ServerActivity] 
			where [SQLServerID] = @SQLServerID
			and [UTCCollectionDateTime] = @UTCCollectionDateTime
			--and isnull([RefreshType],0) = @RefreshType -- VH - Removing to prevent collision
		   )
	)
begin
	update [ServerActivity] set
		[StateOverview] = coalesce(@StateOverview,[StateOverview])
	   ,[SystemProcesses] = coalesce(@SystemProcesses, [SystemProcesses])
	   ,[SessionList] = coalesce(@SessionList, [SessionList])
	   ,[LockStatistics] = coalesce(@LockStatistics, [LockStatistics])
	   ,[LockList] = coalesce(@LockList, [LockList])
	   ,[RefreshType] = case when isnull([RefreshType],0) = 0 then 0 else @RefreshType end  -- VH - If we're combining up more than one type we want the scheduled refresh type to take precedence
	where [SQLServerID] = @SQLServerID 
		and [UTCCollectionDateTime] = @UTCCollectionDateTime 
		-- and isnull([RefreshType],0) = @RefreshType
end
else
begin
	insert into [ServerActivity]
		([SQLServerID]
		,[UTCCollectionDateTime]
		,[StateOverview]
		,[SystemProcesses]
		,[SessionList]
		,[LockStatistics]
		,[LockList]
		,[RefreshType])
	values
		(@SQLServerID
		,@UTCCollectionDateTime
		,@StateOverview
		,@SystemProcesses
		,@SessionList
		,@LockStatistics
		,@LockList
		,@RefreshType)
end

SELECT @err = @@error
	
RETURN @err

end
