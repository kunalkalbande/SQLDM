if (object_id('Grooming.p_LogGroomingAction') is not null)
begin
drop procedure Grooming.p_LogGroomingAction
end
go
CREATE PROCEDURE [Grooming].[p_LogGroomingAction]
(
@RunID uniqueidentifier,
@Sequence int output,
@Action nvarchar(256),
@AffectedRecords int,
@InstanceName nvarchar(256) = null
)
AS
BEGIN

	set @Sequence = @Sequence + 1
	insert into [GroomingLog] (RunID,Sequence,UTCActionEndDateTime,[Action],AffectedRecords,InstanceName)
	values (@RunID, @Sequence, getutcdate(), @Action, @AffectedRecords, @InstanceName)

END



