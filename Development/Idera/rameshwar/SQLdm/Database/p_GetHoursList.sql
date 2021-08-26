if (object_id('p_GetHoursList') is not null)
begin
drop procedure [p_GetHoursList]
end

GO
CREATE PROCEDURE [p_GetHoursList]	
AS
BEGIN
	if (OBJECT_ID('#tempHoursTable') IS NOT NULL)
	begin
		drop table #tempHoursTable
	end
	
	CREATE TABLE #tempHoursTable (
		hoursKey int NOT NULL PRIMARY KEY,
		hoursValue VARCHAR(5) NOT NULL 
	);

	DECLARE @intFlag INT
	DECLARE @twoDigitsHour VARCHAR(2)

	SET @intFlag = 0
	SET @twoDigitsHour = '00'

	WHILE (@intFlag < 24)
	BEGIN

		IF(@intFlag < 10)
		BEGIN
			SET @twoDigitsHour = '0' + CAST(@intFlag AS varchar(1))
		END
		ELSE
		BEGIN
			SET @twoDigitsHour = CAST(@intFlag AS varchar(2))
		END

		INSERT INTO #tempHoursTable(hoursKey, hoursValue)
		SELECT @twoDigitsHour, @twoDigitsHour + ':00'

		SET @intFlag = @intFlag + 1
	END

	select * from #tempHoursTable	

	drop table #tempHoursTable
END
