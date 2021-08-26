if (object_id('p_InsertTempdbFileData') is not null)
begin
drop procedure p_InsertTempdbFileData
end
go
create procedure p_InsertTempdbFileData
	@DatabaseID int,
	@FileName nvarchar(255),
	@FileType bit,
	@FilePath nvarchar(1000),
	@FileID int,
	@UTCCollectionDateTime datetime,
	@TimeDeltaInSeconds float,
	@DriveName nvarchar(256),
	@FileSizeInKilobytes bigint,
	@UserObjectsInKilobytes bigint,
	@InternalObjectsInKilobytes bigint,
	@VersionStoreInKilobytes bigint,
	@MixedExtentsInKilobytes bigint,
	@UnallocatedSpaceInKilobytes bigint
	
as
begin

if (@FileID is null)
begin
	execute [p_InsertDatabaseFile]
		@DatabaseID,
		@FileName,
		@FileType,
		@FilePath,
		@DriveName,
		@FileID output
end

if (@FileID is null)
	return


insert into
	[TempdbFileData]
	(
		[FileID],
		[UTCCollectionDateTime],
		[FileSizeInKilobytes],
		[UserObjectsInKilobytes],	
		[InternalObjectsInKilobytes],
		[VersionStoreInKilobytes],
		[MixedExtentsInKilobytes],
		[UnallocatedSpaceInKilobytes],
		[TimeDeltaInSeconds]
	)
values
	(
	@FileID,
	@UTCCollectionDateTime,
	@FileSizeInKilobytes,
	@UserObjectsInKilobytes,
	@InternalObjectsInKilobytes,
	@VersionStoreInKilobytes,
	@MixedExtentsInKilobytes,
	@UnallocatedSpaceInKilobytes,
	@TimeDeltaInSeconds
	)

end

