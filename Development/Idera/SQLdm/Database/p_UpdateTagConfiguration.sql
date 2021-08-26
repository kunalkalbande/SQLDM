IF (object_id('p_UpdateTagConfiguration') IS NOT NULL)
BEGIN
DROP PROCEDURE p_UpdateTagConfiguration
END
GO

CREATE PROCEDURE [dbo].[p_UpdateTagConfiguration]
(
	@TagId INT OUTPUT,
	@NewTagName NVARCHAR(50) = NULL,
	@LinkedServers nvarchar(max) = NULL,
	@LinkedCustomCounters nvarchar(max) = NULL,
	@LinkedPermissions nvarchar(max) = NULL,
	@Synchronize bit = 1
)
AS
BEGIN
	DECLARE @err INT
	DECLARE @xmlDoc INT
	
	IF (@TagId = -1)
		EXEC p_AddTag @NewTagName, @TagId OUTPUT	
	ELSE
		IF (@NewTagName IS NOT NULL)
			EXEC p_UpdateTag @TagId, @NewTagName

	IF (@LinkedServers IS NOT NULL)
	BEGIN
		DECLARE @ServerIds TABLE(ServerId INT) 

		EXEC sp_xml_preparedocument @xmlDoc OUTPUT, @LinkedServers

		INSERT INTO @ServerIds	
			SELECT ServerId
				FROM OPENXML(@xmlDoc, '//Server', 1) WITH (ServerId INT)

		EXEC sp_xml_removedocument @xmlDoc

		IF (@Synchronize = 1) 
		BEGIN
			DELETE FROM ServerTags 
			WHERE 
				TagId = @TagId and
				SQLServerId NOT IN (SELECT ServerId FROM @ServerIds)
		END

		DELETE FROM @ServerIds 
		WHERE ServerId IN (SELECT SQLServerId FROM ServerTags WHERE TagId = @TagId)

		INSERT INTO ServerTags
		SELECT ServerId, @TagId FROM @ServerIds
	END

	IF (@LinkedCustomCounters IS NOT NULL)
	BEGIN
		DECLARE @CustomCounterIds TABLE(CustomCounterId INT) 

		EXEC sp_xml_preparedocument @xmlDoc OUTPUT, @LinkedCustomCounters

		INSERT INTO @CustomCounterIds	
			SELECT CustomCounterId
				FROM OPENXML(@xmlDoc, '//CustomCounter', 1) WITH (CustomCounterId INT)

		EXEC sp_xml_removedocument @xmlDoc

		IF (@Synchronize = 1) 
		BEGIN
			DELETE FROM CustomCounterTags 
			WHERE 
				TagId = @TagId and
				Metric NOT IN (SELECT CustomCounterId FROM @CustomCounterIds)
		END

		DELETE FROM @CustomCounterIds 
		WHERE CustomCounterId IN (SELECT Metric FROM CustomCounterTags WHERE TagId = @TagId)

		INSERT INTO CustomCounterTags
		SELECT CustomCounterId, @TagId FROM @CustomCounterIds
	END

	IF (@LinkedPermissions IS NOT NULL)
	BEGIN
		DECLARE @PermissionIds TABLE(PermissionId INT) 

		EXEC sp_xml_preparedocument @xmlDoc OUTPUT, @LinkedPermissions

		INSERT INTO @PermissionIds	
			SELECT PermissionId
				FROM OPENXML(@xmlDoc, '//Permission', 1) WITH (PermissionId INT)

		EXEC sp_xml_removedocument @xmlDoc

		IF (@Synchronize = 1) 
		BEGIN
			DELETE FROM PermissionTags 
			WHERE 
				TagId = @TagId and
				PermissionId NOT IN (SELECT PermissionId FROM @PermissionIds)
		END

		DELETE FROM @PermissionIds 
		WHERE PermissionId IN (SELECT PermissionId FROM PermissionTags WHERE TagId = @TagId)

		INSERT INTO PermissionTags
		SELECT PermissionId, @TagId FROM @PermissionIds
	END

	exec p_SyncCustomCounterThresholds

	SELECT @err = @@error
	RETURN @err
END