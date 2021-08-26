if (object_id('p_AddAlert') is not null)
begin
drop procedure [p_AddAlert]
end
go

create procedure [p_AddAlert] 
	@UTCOccurrenceDateTime datetime,
	@ServerName nvarchar(256),
	@DatabaseName nvarchar(255),
	@TableName nvarchar(255),
	@Metric int,
	@Severity tinyint,
	@StateEvent tinyint,
	@Value float,
	@Heading nvarchar(256),
	@Message nvarchar(2048),
	@QualifierHash nvarchar(28),
	@LinkedData uniqueidentifier
as
begin
	DECLARE @err int
	DECLARE @alertid bigint
	DECLARE @active bit

	if (@Severity < 2)
		SELECT @active = 0
	else
		SELECT @active = 1

	if (@Metric = 56) 
		SELECT @active = 0

	-- dont actually insert the alert for metrics 
	--	34 - Long Running Agent Job
	--	35 - Bombed Agent Job
	if (@active = 0) and (@Metric in (34, 35)) 
	begin
--  old stuff to clear job alerts rather than adding an OK alert - no longer needed not that all alerts are added every refresh
--		UPDATE [Alerts] SET
--			[Active] = 0
--			WHERE
--				([Active] = 1 and [Metric] = @Metric) and
--				(@ServerName is null or [ServerName] = @ServerName) and
--				(@QualifierHash is null or [QualifierHash] = @QualifierHash)
--
--		SELECT @err = @@error
--
--		if (@err = 0)
--			COMMIT
--		else
--			ROLLBACK

		RETURN 0
	end

	BEGIN TRANSACTION

	INSERT	into Alerts -- WITH (TABLOCK)
	(
			[UTCOccurrenceDateTime],
			[ServerName],
			[DatabaseName],
			[TableName],
			[Active],
			[Metric],
			[Severity],
			[StateEvent],
			[Value],
			[Heading],
			[Message],
			[QualifierHash],
			[LinkedData]
		) VALUES (
			@UTCOccurrenceDateTime,
			@ServerName,
			@DatabaseName,
			@TableName,
			@active,
			@Metric,
			@Severity,
			@StateEvent,
			@Value,
			@Heading,
			@Message,
			@QualifierHash,
			@LinkedData)

	SELECT @err = @@error

--  This be old stuff to reset the active flag for all alerts matching the alert just added - it sucks the life out of an sql server
--	if (@err = 0)
--	begin
--		SELECT @alertid = SCOPE_IDENTITY()
--		print @alertid
--		
--		if ((@Metric = 48) or (@Metric = 12 AND @Value = 4)) 
--		begin
--			-- SQL Service Status is Undetermined
--			UPDATE [Alerts] SET
--				[Active] = 0
--			WHERE
--				([Active] = 1 and [AlertID] < @alertid) and
--				(@ServerName is null or [ServerName] = @ServerName)
--		end
--		else
--		begin
--			UPDATE [Alerts] SET
--				[Active] = 0
--			WHERE
--				([Active] = 1 and [Metric] = @Metric and [AlertID] < @alertid) and
--				(@ServerName is null or [ServerName] = @ServerName) and
--				(@DatabaseName is null or [DatabaseName] = @DatabaseName) and
--				(@TableName is null or [TableName] = @TableName) and 
--				(@QualifierHash is null or [QualifierHash] = @QualifierHash)
--		end
--		SELECT @err = @@error
--	end
	
	if (@err = 0)
		COMMIT
	else
		ROLLBACK

	RETURN @err
end
