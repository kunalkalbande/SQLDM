if (object_id('p_GetCounterCategories') is not null)
begin
drop procedure p_GetCounterCategories
end
go

CREATE PROCEDURE [dbo].p_GetCounterCategories
AS
begin
	declare @e int

	SELECT DISTINCT [Category] FROM MetricInfo ORDER BY [Category]

	SET @e = @@ERROR

	RETURN @e
END	

