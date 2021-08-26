if (object_id('p_GetLicenseKeysAndListener') is not null)
begin
drop procedure p_GetLicenseKeysAndListener
end
go

CREATE PROCEDURE [dbo].[p_GetLicenseKeysAndListener](
    @LicenseID UNIQUEIDENTIFIER,
    @ReturnServerCount int output,
    @ReturnInstanceName nvarchar(128) output
)
AS

BEGIN
       DECLARE @err INT
       DECLARE @monitoredServerCount INT
       DECLARE @instanceName nvarchar(128)
       DECLARE @SQLString nvarchar(max);
       DECLARE @ParmDefinition nvarchar(500);

       declare @repo sysname;
       set @repo = db_name()

       -- set return values to current number of registered servers and SQL instance
       SELECT @monitoredServerCount = COUNT([SQLServerID]) FROM MonitoredSQLServers (NOLOCK) WHERE Active = 1  

       DECLARE @sqlServerVersion nvarchar(128)
       select @sqlServerVersion = @@MICROSOFTVERSION / 0x01000000
       SET @ParmDefinition = N'@repoName sysname, @instanceNameOut nvarchar(255) OUTPUT';

       if @sqlServerVersion >= 11
       begin
              -- Used to avoid compatibility problems with lower versions of SQL Server.
              select @SQLString = 'select @instanceNameOut = l.dns_name + '','' + convert(nvarchar(10),l.port)'
              + ' from sys.dm_hadr_database_replica_states s'
              + ' inner join sys.availability_group_listeners l on l.group_id = s.group_id'
              + ' and db_name(database_id) = @repoName'
              + ' group by l.dns_name + '','' + convert(nvarchar(10),l.port)'
              EXECUTE sp_executesql @SQLString, @ParmDefinition, @repoName = @repo, @instanceNameOut = @instanceName  OUTPUT;
       end
       
       -- Verify if the listener exist or not.
	   declare @oldListener nvarchar(1024)
	   select @oldListener = (select top (1) Character_Value from RepositoryInfo where Name = 'AGListener');

	   if @instanceName is not null and not exists(select 1 from RepositoryInfo where Name = 'AGListener')
		insert into RepositoryInfo (Name, Character_Value) values('AGListener',@instanceName)
	   else
	       if @instanceName is not null and @instanceName <> @oldListener
		   begin
			   --Update the RepositoryInfo table with the new AG listener
			   update RepositoryInfo set Character_Value = @instanceName where Name = 'AGListener'
		   end
	   
       if @instanceName is null
	   begin
               delete from RepositoryInfo where Name = 'AGListener'
               select @instanceName = CONVERT(nvarchar(128), serverproperty('servername'))
	   end


       IF (@LicenseID IS NULL) 
       BEGIN
              SELECT [LicenseID],[LicenseKey],[DateAddedUtc] FROM [LicenseKeys] (NOLOCK)
              ORDER BY [DateAddedUtc]
       END
       ELSE
       BEGIN
              SELECT [LicenseID],[LicenseKey],[DateAddedUtc] FROM [LicenseKeys] (NOLOCK)
				WHERE ([LicenseID] = @LicenseID)
              ORDER BY [DateAddedUtc]
       END

       SELECT @err = @@error
       IF (@err = 0)
       BEGIN
              SELECT @ReturnServerCount = @monitoredServerCount 
              SELECT @ReturnInstanceName = @instanceName
       END

       RETURN @err

END
