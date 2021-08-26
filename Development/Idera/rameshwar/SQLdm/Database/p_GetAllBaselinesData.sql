IF (object_id('p_GetAllBaselinesData') IS NOT NULL)
BEGIN
DROP PROCEDURE p_GetAllBaselinesData
END
GO

CREATE PROCEDURE [dbo].p_GetAllBaselinesData
	@SQLServerId INT,
	@endTime DATETIME,
	@HistoryInSeconds INT, 
	@DayDifferenceAfterUTCConversion DECIMAL
AS
BEGIN
	DECLARE @startTime DATETIME 
	SET @startTime = DATEADD(SECOND,@HistoryInSeconds*-1,@endTime)
	--select @startTime, @endTime

	--START Day check parameters 
	--END Day check parameters

	CREATE TABLE #LocalTempTable(
	TemplateID int
	, BaselineName nvarchar(150)
	, StartSchedule DATETIME
	, EndSchedule DATETIME
	, Active bit)

	CREATE TABLE #LocalTempTableDefault(
	TemplateID int
	, BaselineName nvarchar(150)
	, Active bit)

	
CREATE TABLE #LocalTemplate(
	ID int identity (1,1)
	, TemplateID int
	, MetricID int
	, Value decimal(38,5)
	, StartSchedule DATETIME
	, EndSchedule DATETIME
	, IsDayChecked BIT
	)	
	
	INSERT INTO #LocalTempTable (TemplateID, BaselineName, StartSchedule, EndSchedule, Active)
	(
		SELECT BT.TemplateID, 
		BT.BaselineName,
		dateAdd(
		second,
		dbo.fn_GetSecondsCountUsingOffset(SUBSTRING(Template, CHARINDEX('<ScheduledStartDate>', Template) + Len('<ScheduledStartDate>')
						, CHARINDEX('</ScheduledStartDate>',Template) - ( CHARINDEX('<ScheduledStartDate>', Template) + LEN ('<ScheduledStartDate>')))),
		--dateDiff(second, getDate(), getUtcDate()),
		CAST (SUBSTRING(Template, CHARINDEX('<ScheduledStartDate>', Template) + Len('<ScheduledStartDate>')
						, (CHARINDEX('</ScheduledStartDate>',Template)-6) - ( CHARINDEX('<ScheduledStartDate>', Template) + LEN ('<ScheduledStartDate>'))) AS DATETIME)),
		dateAdd(
		second,
		dbo.fn_GetSecondsCountUsingOffset(SUBSTRING(Template, CHARINDEX('<ScheduledStartDate>', Template) + Len('<ScheduledStartDate>')
						, CHARINDEX('</ScheduledStartDate>',Template) - ( CHARINDEX('<ScheduledStartDate>', Template) + LEN ('<ScheduledStartDate>')))),
		--dateDiff(second, getDate(), getUtcDate()),
		CAST (SUBSTRING(Template, CHARINDEX('<ScheduledEndDate>', Template) + Len('<ScheduledEndDate>')
						, (CHARINDEX('</ScheduledEndDate>',Template)-6) - ( CHARINDEX('<ScheduledEndDate>', Template) + LEN ('<ScheduledEndDate>')))  AS DATETIME)),
		BT.Active
		FROM BaselineTemplates BT
		WHERE CHARINDEX('</ScheduledStartDate>',Template) > 0 AND BaselineName != 'Default' 
		AND SQLServerID = @SQLServerId AND Active = 1
		AND 1 = dbo.fn_CompareDaysSelection (
		   CAST (SUBSTRING(Template, CHARINDEX('<ScheduledSelectedDays>', Template) + Len('<ScheduledSelectedDays>')
						, CHARINDEX('</ScheduledSelectedDays>',Template) - ( CHARINDEX('<ScheduledSelectedDays>', Template) + LEN ('<ScheduledSelectedDays>')))  AS INT)
		  ,DATEPART(dw, @startTime), @DayDifferenceAfterUTCConversion)
	) 
	--select * from #LocalTempTable
	INSERT INTO #LocalTempTableDefault (TemplateID, BaselineName, Active)
	(
		SELECT BT.TemplateID, 
		BT.BaselineName,
		BT.Active
		FROM BaselineTemplates BT
		WHERE BaselineName = 'Default' AND SQLServerID = @SQLServerId AND Active = 1
		UNION
		SELECT BT.TemplateID, 
		BT.BaselineName,BT.Active
		FROM BaselineTemplates BT
		WHERE BaselineName = 'Default' AND SQLServerID = @SQLServerId AND Active = 0
	) 


	INSERT INTO #LocalTemplate(TemplateID
		, MetricID
		, Value
		, StartSchedule 
		, EndSchedule )
		(
	select TemplateID
		,MetricID
		,Value
		,CASE WHEN CONVERT(VARCHAR,StartSchedule,114) < CONVERT(VARCHAR,@startTime,114) THEN @startTime ELSE StartSchedule END AS StartSchedule,
		CASE WHEN EndSchedule > @endTime THEN @endTime ELSE EndSchedule END AS EndSchedule 
		FROM (
			select TT.TemplateID

			,BS.MetricID
			, BS.Mean + BS.StdDeviation AS Value
			,CASE WHEN (CONVERT(VARCHAR,StartSchedule,114) < CONVERT(VARCHAR,EndSchedule,114) AND CONVERT(VARCHAR,@startTime,114) >= CONVERT(VARCHAR,@endTime,114))
			THEN
				CASE WHEN(CONVERT(VARCHAR,StartSchedule,114) >= CONVERT(VARCHAR,@endTime,114))
				THEN
					dateadd(dd,datediff(dd,StartSchedule, @startTime), StartSchedule)
				ELSE
					dateadd(dd,datediff(dd,StartSchedule, @endTime), StartSchedule)
				END
			ELSE
				dateadd(dd,datediff(dd,StartSchedule, @startTime), StartSchedule)
			END
			AS StartSchedule
			, CASE WHEN (CONVERT(VARCHAR,StartSchedule,114) < CONVERT(VARCHAR,EndSchedule,114) AND CONVERT(VARCHAR,@startTime,114) >= CONVERT(VARCHAR,@endTime,114))
			THEN
				CASE WHEN(CONVERT(VARCHAR,EndSchedule,114) >= CONVERT(VARCHAR,@endTime,114))
				THEN
					dateadd(dd,datediff(dd,EndSchedule, @startTime), EndSchedule)
				ELSE
					dateadd(dd,datediff(dd,EndSchedule, @endTime), EndSchedule)
				END
			ELSE
				dateadd(dd,datediff(dd,EndSchedule, @endTime), EndSchedule)
			END AS EndSchedule
			, ROW_NUMBER() OVER(PARTITION BY TT.TemplateID,BS.MetricID ORDER BY BS.UTCCalculation DESC) RowNum
			
			FROM #LocalTempTable TT
			JOIN BaselineStatistics BS ON TT.TemplateID = BS.TemplateID 
			where Not(((CONVERT(VARCHAR,TT.StartSchedule,114) > CONVERT(VARCHAR,@endTime,114))
			or ( CONVERT(VARCHAR,TT.EndSchedule,114) < CONVERT(VARCHAR,@startTime,114))))
		) FILTER wHERE RowNum = 1
	)
	--select * from #LocalTemplate
	IF EXISTS(select TemplateID from #LocalTemplate)
	BEGIN
		;WITH CTE AS(
		SELECT rownum = ROW_NUMBER() OVER(ORDER BY p.MetricID, p.StartSchedule),
		TemplateID
		,MetricID
		,Value
		,StartSchedule
		,EndSchedule  FROM #LocalTemplate p
		)

		--select * from CTE
		INSERT INTO #LocalTemplate(TemplateID
		, MetricID
		, Value
		, StartSchedule 
		, EndSchedule )
		(
			SELECT 
			DEFAULTInsert.TemplateID,
			DEFAULTInsert.MetricID, Value,
			StartSchedule,
			EndSchedule
			FROM
			(
				SELECT 
				BS.TemplateID,
				BS.MetricID, Mean + StdDeviation AS Value,
				CTE.EndSchedule AS StartSchedule,
				nex.StartSchedule AS EndSchedule,
				UTCCalculation
				,ROW_NUMBER() OVER(PARTITION BY BS.MetricID ORDER BY Active DESC, BS.UTCCalculation ASC) RowNum
				FROM CTE
				--LEFT JOIN CTE prev ON prev.rownum = CTE.rownum - 1
				LEFT JOIN CTE nex ON nex.rownum = CTE.rownum + 1
				JOIN BaselineStatistics BS ON BS.MetricID = CTE.MetricID
				JOIN #LocalTempTableDefault LT ON LT.TemplateID = BS.TemplateID
				WHERE CTE.EndSchedule < nex.StartSchedule AND CTE.MetricID = nex.MetricID
			) DEFAULTInsert WHERE RowNum = 1
			)

			--select * from #LocalTemplate --ORDER BY StartSchedule
	END
	If EXISTS(select TemplateID from #LocalTemplate)
	BEGIN
		SELECT MetricID,Value,UTCScheduled--, TemplateID 
		FROM 
		(
			SELECT MetricID,(Mean+StdDeviation) AS Value,StartSchedule AS UTCScheduled
			--, TemplateID 
			FROM (SELECT BS.MetricID, Mean,StdDeviation,@startTime AS StartSchedule, BS.TemplateID,ROW_NUMBER() OVER(PARTITION BY BS.MetricID ORDER BY Active DESC, BS.UTCCalculation DESC) RowNum
			FROM BaselineStatistics BS
			JOIN #LocalTempTableDefault LT ON LT.TemplateID = BS.TemplateID
			--JOIN #LocalTemplate T on T.MetricID = BS.MetricID
			WHERE BS.SQLServerID = @SQLServerId AND BS.MetricID IN (-1003,-127,-126,-125,-124,-116,-112,-111,-110,-109,-71,-70,0,13,25,26,27,28,29,30,31,76,81,98) AND @startTime < (select Top(1)StartSchedule FROM #LocalTemplate WHERE MetricID = BS.MetricID ORDER BY StartSchedule)
			AND BaselineName = 'Default'
			) T WHERE T.RowNum = 1
			UNION
			--Below query will give edge case for end point according to @endStart in side range or out side range
			SELECT MetricID, Value,StartSchedule AS UTCScheduled
			--, TemplateID 
			FROM (SELECT MetricID, Value, (CASE WHEN @startTime > StartSchedule THEN @startTime ELSE StartSchedule END) AS StartSchedule, TemplateID,ROW_NUMBER() OVER(PARTITION BY TemplateID, MetricID ORDER BY LT.StartSchedule ) RowNum
			FROM #LocalTemplate LT
			WHERE MetricID IN (-1003,-127,-126,-125,-124,-116,-112,-111,-110,-109,-71,-70,0,13,25,26,27,28,29,30,31,76,81,98)
			) T WHERE T.RowNum = 1
			UNION
			SELECT MetricID, Value,StartSchedule AS UTCScheduled
			--, TemplateID 
			FROM (SELECT MetricID, Value, (CASE WHEN @endTime < EndSchedule THEN @endTime ELSE EndSchedule END) AS StartSchedule, TemplateID,ROW_NUMBER() OVER(PARTITION BY MetricID ORDER BY LT.EndSchedule DESC) RowNum
			FROM #LocalTemplate LT
			WHERE MetricID IN (-1003,-127,-126,-125,-124,-116,-112,-111,-110,-109,-71,-70,0,13,25,26,27,28,29,30,31,76,81,98)
			) T WHERE T.RowNum = 1
			UNION
			SELECT MetricID,(Mean+StdDeviation) AS Value,EndSchedule AS UTCScheduled
			--, TemplateID 
			FROM (SELECT MetricID, Mean,StdDeviation,@endTime AS EndSchedule, BS.TemplateID,ROW_NUMBER() OVER(PARTITION BY MetricID ORDER BY Active DESC, BS.UTCCalculation DESC) RowNum
			FROM BaselineStatistics BS
			JOIN #LocalTempTableDefault LT ON LT.TemplateID = BS.TemplateID WHERE BS.SQLServerID = @SQLServerId AND MetricID IN (-1003,-127,-126,-125,-124,-116,-112,-111,-110,-109,-71,-70,0,13,25,26,27,28,29,30,31,76,81,98) AND @endTime > (select Top(1)EndSchedule FROM #LocalTemplate WHERE MetricID = BS.MetricID ORDER BY EndSchedule DESC)
			AND BaselineName = 'Default'
			) T WHERE T.RowNum = 1
		) Final
		ORDER BY MetricID, UTCScheduled 
	END
	ELSE
	BEGIN
		SELECT MetricID,Value,UTCScheduled--, TemplateID 
		FROM 
		(
			SELECT MetricID,(Mean+StdDeviation) AS Value,StartSchedule AS UTCScheduled
			--, TemplateID 
			FROM (SELECT BS.MetricID, Mean,StdDeviation,@startTime AS StartSchedule, BS.TemplateID,ROW_NUMBER() OVER(PARTITION BY BS.MetricID ORDER BY Active DESC, BS.UTCCalculation DESC) RowNum
			FROM BaselineStatistics BS
			JOIN #LocalTempTableDefault LT ON LT.TemplateID = BS.TemplateID
			--JOIN #LocalTemplate T on T.MetricID = BS.MetricID
			WHERE BS.SQLServerID = @SQLServerId AND BS.MetricID IN (-1003,-127,-126,-125,-124,-116,-112,-111,-110,-109,-71,-70,0,13,25,26,27,28,29,30,31,76,81,98)
			AND BaselineName = 'Default'
			) T WHERE T.RowNum = 1
			UNION
			SELECT MetricID,(Mean+StdDeviation) AS Value,EndSchedule AS UTCScheduled
			--, TemplateID 
			FROM (SELECT MetricID, Mean,StdDeviation,@endTime AS EndSchedule, BS.TemplateID,ROW_NUMBER() OVER(PARTITION BY MetricID ORDER BY Active DESC, BS.UTCCalculation DESC) RowNum
			FROM BaselineStatistics BS
			JOIN #LocalTempTableDefault LT ON LT.TemplateID = BS.TemplateID WHERE BS.SQLServerID = @SQLServerId AND MetricID IN (-1003,-127,-126,-125,-124,-116,-112,-111,-110,-109,-71,-70,0,13,25,26,27,28,29,30,31,76,81,98)
			AND BaselineName = 'Default'
			) T WHERE T.RowNum = 1
		) Final
		ORDER BY MetricID, UTCScheduled
	END	
	
	END