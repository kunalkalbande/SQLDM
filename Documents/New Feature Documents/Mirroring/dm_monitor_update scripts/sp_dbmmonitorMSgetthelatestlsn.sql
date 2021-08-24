create procedure sys.sp_dbmmonitorMSgetthelatestlsn   
      @database_name                sysname,
      @end_of_log_lsn         numeric(25,0)=null output
as
begin
      set nocount on
      if (is_srvrolemember(N'sysadmin') &lt;&gt; 1 )
        begin
                  raiserror(21089, 16, 1)
                  return 1
            end
      if @database_name is null
      begin
            return 2
      end

      declare @str                  char(64),
                  @file             bigint,
                  @file_size        int,
                  @array                  bigint,
                  @array_size       int,
                  @slot             int,
                  @command          nvarchar(400)

      --
    -- Check if the database specified exists 
    --
    if not exists (select * from master.sys.databases where name = @database_name)
    begin
        return 2
    end

      set @command = N'dbcc dbtable(' + replace(@database_name, N'''',N'''''') + N') with tableresults, no_infomsgs'

      declare @temp table(parentObject nvarchar(255),Object nvarchar(255),field nvarchar(255),value nvarchar(255))
                                                      -- TO DO: get correct values from SteveLi
                                                      -- DONE: They are all nvarchar(255)
      insert into @temp exec( @command )                                                                                                              
      select @str = value from @temp where field=N'm_flushLSN';
      
      set @file_size = charindex(N':', @str)
      set @file = cast( LEFT( @str, @file_size - 1) as bigint)
      set @array_size = charindex(N':', @str, @file_size + 1)
      set @array = cast( substring( @str, @file_size + 1, @array_size - @file_size - 1) as bigint)
      set @slot = cast( substring( @str, @array_size + 1, charindex(N' ', @str, @array_size + 1) - @array_size ) as int)
      set @end_of_log_lsn = @file * 1000000000000000 + @array * 100000 + @slot
      -- make sure @slot &lt; MAX_SHORT (1024 * 64)
      -- check to make sure tht file and array are &lt; 4 Gig.
      if (@end_of_log_lsn &lt; 1000000000000000 or @slot &gt;= 65536 or @file &gt;= 4294967296 or @array &gt;= 4294967296)
      begin
            set @end_of_log_lsn = null
            return 3    -- return a single error code.
      end
      return 0
end

