if (object_id('p_InsertDeadlock') is not null)
begin
drop procedure p_InsertDeadlock
end
go
create procedure p_InsertDeadlock
	@SQLServerID int,
	@UTCCollectionDateTime datetime,
	@DeadlockID uniqueidentifier,         
	@XDLData nvarchar(max),   
	@ReturnMessage nvarchar(128) output
as
begin

insert into [Deadlocks]
   ([DeadlockID]
	,[SQLServerID]
	,[UTCCollectionDateTime]
	,[XDLData])
values
	(@DeadlockID,
	@SQLServerID,
	@UTCCollectionDateTime,
	@XDLData)

end
go
