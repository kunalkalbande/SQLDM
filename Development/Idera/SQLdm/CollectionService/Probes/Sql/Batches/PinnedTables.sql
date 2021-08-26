
declare @db_name varchar(255)
declare @db_name1 varchar(255)
declare @status int
declare read_db insensitive cursor for
 select
   name,
   status
 from
   master..sysdatabases (nolock)
 Where
   mode=0
   and (has_dbaccess(name) = 1)
     and DATABASEPROPERTY(name, 'IsInLoad') = 0
     and DATABASEPROPERTY(name, 'IsSuspect') = 0
     and isnull(DATABASEPROPERTY(name, 'IsInRecovery'),0) = 0
     and isnull(DATABASEPROPERTY(name, 'IsNotRecovered'),0) = 0
     and isnull(DATABASEPROPERTY(name, 'Isshutdown'),0) = 0
     and DATABASEPROPERTY(name, 'IsOffline') = 0
     and status & 32 <> 32
     and status & 64 <> 64
     and status & 128 <> 128
     and status & 256 <> 256
     and status & 512 <> 512
for read only
open read_db
fetch read_db into @db_name, @status
while @@FETCH_STATUS = 0
begin
 if @status & 4096 = 4096
 begin
   if (exists(select * from master..sysprocesses (nolock) where dbid = DB_ID(@db_name)))
     goto fetchnext
 end
  select @db_name1 = replace(@db_name,char(39),char(39)+char(39))
  select @db_name = replace(@db_name,char(93),char(93)+char(93))
  execute ('use [' + @db_name + '] select ''['' + ''' + @db_name1 + ''' + ''].[''+ USER_NAME(uid) + ''].['' + o.name + '']'', (used * 8), i.rows
           from [' + @db_name + ']..sysindexes i (nolock), [' + @db_name + ']..sysobjects o (nolock)
           Where o.status & 1048576 <> 0 and i.id = o.id  and indid < 2')

 fetchnext:
   fetch read_db into @db_name, @status
End
Close read_db
deallocate read_db
set nocount off