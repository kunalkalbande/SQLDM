if (object_id('[p_InsertBlock]') is not null)
begin
drop procedure [p_InsertBlock]
end
go

create procedure [dbo].[p_InsertBlock]
	@BlockID uniqueidentifier,
	@SQLServerID int,
	@UTCCollectionDateTime datetime,
	@XActID bigint,
	@XDLData nvarchar(max),   
	@ReturnMessage nvarchar(128) output
as
begin

insert into [Blocks]
   ([BlockID]
    ,[XActID]
	,[SQLServerID]
	,[UTCCollectionDateTime]
	,[XDLData])
values
	(@BlockID,
	@XActID,
	@SQLServerID,
	@UTCCollectionDateTime,
	@XDLData)

end

GO


