if (object_id('p_SetAggregationFlag') is not null)
begin
drop procedure p_SetAggregationFlag
end
go
CREATE PROCEDURE [dbo].p_SetAggregationFlag(
	@SQLSignatureID bigint,
	@DoNotAggregate bit = 0
)
as 
begin
	update SQLSignatures
	set DoNotAggregate = @DoNotAggregate
	where SQLSignatureID = @SQLSignatureID

end