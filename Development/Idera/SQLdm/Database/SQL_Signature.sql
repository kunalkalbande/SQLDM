if (object_id('SQL_Signature') is not null)
begin
drop function SQL_Signature
end
go
CREATE FUNCTION SQL_Signature 
	(@p1 ntext)
RETURNS nvarchar(3500)

--
-- This function is provided "AS IS" with no warranties, and confers no rights. 
-- Use of included script samples are subject to the terms specified at http://www.microsoft.com/info/cpyright.htm
-- 
-- Strips query strings in sysprocesses
AS
BEGIN 
	DECLARE @pos as INT
	DECLARE @mode as CHAR(10)
	DECLARE @maxlength as INT
	DECLARE @p2 as NCHAR(3500)
	DECLARE @currchar as CHAR(1), @nextchar as CHAR(1)
	DECLARE @p2len as INT


	SET @maxlength = len(rtrim(substring(@p1,1,3500)));
--	SET @maxlength = case when @maxlength > @parselength 
--			then @parselength else @maxlength end

	SET @pos = 1;
	SET @p2 = '';
	SET @p2len = 0;
	SET @currchar = ''
	set @nextchar = ''
	SET @mode = 'command';

	WHILE (@pos <= @maxlength) BEGIN
		SET @currchar = substring(@p1,@pos,1)
		SET @nextchar = substring(@p1,@pos+1,1)
		IF @mode = 'command' BEGIN
			SET @p2 = left(@p2,@p2len) + @currchar
			SET @p2len = @p2len + 1 
			IF @currchar in (',','(',' ','=','<','>','!') and
			   @nextchar between '0' and '9' BEGIN
				set @mode = 'number'
				SET @p2 = left(@p2,@p2len) + '#'
				SET @p2len = @p2len + 1
				END 
			IF @currchar = '''' BEGIN
				set @mode = 'literal'
				SET @p2 = left(@p2,@p2len) + '#'''
				SET @p2len = @p2len + 2 
				END
			END
		ELSE IF @mode = 'number' and @nextchar in (',',')',' ','=','<','>','!')
			SET @mode= 'command'
		ELSE IF @mode = 'literal' and @currchar = ''''
			SET @mode= 'command'

		SET @pos = @pos + 1
	END
	RETURN @p2 

END
GO

if exists (select id from syscolumns where id =  object_id('QueryMonitor') and name = 'StatementText' collate database_default and length = -1)
begin
	declare @cmd varchar(8000)
	select @cmd = replace(
				  replace(
				  replace( 
				  replace( 
				  replace( 
				  text, 
				  'nvarchar(3500)','nvarchar(max)'),
				  'NCHAR(3500)','nvarchar(max)'),
				  'SET @maxlength = len(rtrim(substring(@p1,1,3500)));','SET @maxlength = len(@p1)'),
				  '(@p1 ntext)','(@p1 nvarchar(max))'),
				  'CREATE FUNCTION SQL_Signature ','ALTER FUNCTION SQL_Signature ')
	from syscomments where id = object_id('SQL_Signature')
	execute( @cmd )
end

GO



