if (object_id('p_AddTask') is not null)
begin
drop procedure [p_AddTask]
end
go

create procedure [p_AddTask]
	@ServerName nvarchar(256),
	@Subject nvarchar(256),
	@Message nvarchar(1024),
	@Comments nvarchar(1024),
	@Owner nvarchar(256),
	@CreatedOn datetime,
	@CompletedOn datetime,
	@Status tinyint,
	@Metric int,
	@Severity tinyint,
	@Value float,
	@EventID int,
	@DatabaseName nvarchar(255),
	@ReturnTaskID int output
as
begin
	DECLARE @err int
	DECLARE @created datetime

	-- if no create date is specified then set one
	SELECT @created = @CreatedOn
	if @created is null
		SELECT @created = GetUTCDate()

	INSERT	into Tasks WITH (TABLOCK) (
			[ServerName],
			[Subject],
			[Message],
			[Comments],
			[Owner],
			[CreatedOn],
			[CompletedOn],
			[Status],
			[Metric],
			[Severity],
			[Value],
			[EventID],
			[DatabaseName]
		) VALUES (
			@ServerName,
			@Subject,
			@Message,
			@Comments,
			@Owner,
			@created,
			@CompletedOn,
			@Status,
			@Metric,
			@Severity,
			@Value,
			@EventID,
			@DatabaseName)

	SELECT @err = @@error
	
	if (@err = 0)
	begin
		SELECT @ReturnTaskID = SCOPE_IDENTITY()
	end

	RETURN @err
end
