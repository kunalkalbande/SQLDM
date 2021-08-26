-- SQLdm 9.1 (Abhishek Joshi)

-- Filegroup Improvements
-- insert the database files information

-- exec p_AddDatabaseFileInformation @MaxSize = 45,
-- 								     @InitialSize = 64,
--								     @UsedSpace = 47,
--								     @AvailableSpace = 43,
--								     @FreeDiskSpace = 65,
--								     @DriveName = 'C'

IF (OBJECT_ID('p_AddDatabaseFileInformation') is not null)
BEGIN
	DROP PROCEDURE [p_AddDatabaseFileInformation]
END

GO

CREATE PROCEDURE [dbo].[p_AddDatabaseFileInformation]
	@MaxSize DECIMAL,
	@InitialSize DECIMAL,
	@UsedSpace DECIMAL,
	@AvailableSpace DECIMAL,
	@FreeDiskSpace DECIMAL,
	@DriveName NVARCHAR(256),
	@FileName NVARCHAR(256),
	@FilePath NVARCHAR(max),
	@FileGroupName NVARCHAR(4000),
	@FileType BIT,
	@DatabaseName NVARCHAR(256),
	@SQLServerID INT,
	@UTCCollectionDateTime DATETIME
AS
BEGIN
	
	DECLARE @DatabaseID INT;
	SELECT @DatabaseID = (SELECT DatabaseID FROM SQLServerDatabaseNames WITH(NOLOCK) WHERE DatabaseName = @DatabaseName and SQLServerID = @SQLServerID);

	IF EXISTS(SELECT FileID FROM DatabaseFiles WITH(NOLOCK) WHERE [FileName] = @FileName  and [DatabaseID] = @DatabaseID)
	BEGIN
		UPDATE DatabaseFiles 
								SET [DriveName] = @DriveName, 
									[FileGroupName] = @FileGroupName 
								WHERE [FileName] = @FileName and [DatabaseID] = @DatabaseID
	END
	ELSE
	BEGIN
		INSERT INTO DatabaseFiles(DatabaseID,[FileName],FileType,FilePath,DriveName,FileGroupName)
		SELECT @DatabaseID,@FileName,@FileType,@FilePath,@DriveName,@FileGroupName
	END

	IF NOT EXISTS(SELECT FileID FROM DatabaseFiles WITH(NOLOCK) WHERE [FileName] = @FileName and [FileGroupName] =@FileGroupName and [DatabaseID] = @DatabaseID)
	BEGIN
		INSERT INTO DatabaseFiles(DatabaseID,[FileName],[FileGroupName],[DriveName],[FilePath],FileType)
		SELECT DatabaseID,@FileName,@FileGroupName,@DriveName,@FilePath,@FileType  FROM SQLServerDatabaseNames SSDN WITH(NOLOCK) 
		WHERE SSDN.SQLServerID=@SQLServerID and SSDN.DatabaseName=@DatabaseName 
	END

	INSERT INTO [DatabaseFileStatistics]( [UTCCollectionDateTime],FileID, MaxSize, InitialSize, UsedSpace, AvailableSpace, FreeDiskSpace, DriveName)
	SELECT @UTCCollectionDateTime,DF.FileID, @MaxSize, @InitialSize, @UsedSpace, @AvailableSpace, @FreeDiskSpace, @DriveName
	FROM  DatabaseFiles DF WITH(NOLOCK)
	JOIN SQLServerDatabaseNames SSDN WITH(NOLOCK) ON DF.DatabaseID=SSDN.DatabaseID 
	WHERE SSDN.SQLServerID=@SQLServerID and SSDN.DatabaseName=@DatabaseName
	and DF.[FileName]=@FileName
END
GO

