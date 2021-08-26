if (object_id('p_UpdateTask') is not null)
begin
drop procedure [p_UpdateTask]
end
go

create procedure [p_UpdateTask]
	@TaskID int,
	@Status tinyint,
	@Owner nvarchar(256),
	@Comments nvarchar(1024)
as
begin
	DECLARE @err int
	DECLARE @completed datetime

	-- if no create date is specified then set one
	SELECT @completed = null;
	if @Status = 16
		SELECT @completed = GetUTCDate()

	UPDATE Tasks SET
			[Comments] = @Comments,
			[Owner] = @Owner,
			[Status] = @Status,
			[CompletedOn] = @completed
		WHERE [TaskID] = @TaskID

	SELECT @err = @@error
	
	RETURN @err
end
