if (object_id('fn_ServerVersionnVarcharToBigInt') is not null)
begin
drop function fn_ServerVersionnVarcharToBigInt
end
go
create function [dbo].[fn_ServerVersionnVarcharToBigInt](
@ServerVersion nvarchar(30)) returns decimal(30,0)
begin
declare @major int
declare @minor int
declare @revision int
declare @build int
declare @points int
declare @strmajor nchar(5)
declare @strminor nchar(5)
declare @strbuild nchar(5)
declare @strrevision nchar(5)

select @points = 0
select @major = 0
select @minor = 0
select @build = 0
select @revision = 0

declare @tmpVersion nvarchar(30)
declare @leftOfPoint nvarchar(30)

select @tmpVersion = @ServerVersion

while charindex('.', @tmpVersion) > 1
begin
	if @points > 3 continue
	--take everything left of the first point
	select @leftOfPoint = left(@tmpVersion,charindex('.', @tmpVersion)-1)

	if @points = 0 	select @major = @leftOfPoint
	if @points = 1	select @minor = @leftOfPoint
	if @points = 2	select @build = @leftOfPoint
	if @points = 3	select @revision = @leftOfPoint
	
	--keep only the right portion of the version
	select @tmpVersion = right(@tmpVersion, Len(@tmpVersion) - charindex('.', @tmpVersion))
	select @points = @points + 1
end

--if there were no points the user wanted only to search on major
if @points = 0 	select @major = @tmpVersion
if @points = 1	select @minor = @tmpVersion
if @points = 2	select @build = @tmpVersion
if @points = 3	select @revision = @tmpVersion

if @major > 99999 select @major = 99999
if @minor > 99999 or @points = 0 select @minor = 99999
if @build > 99999 or @points = 0 or @points = 1 select @build = 99999
if @revision > 99999 or @points = 0 or @points = 1 or @points = 2 select @revision = 99999

SELECT @strmajor = REPLICATE('0', 5 - len(convert(nvarchar(5),@major))) + convert(nvarchar(5),@major)
SELECT @strminor = REPLICATE('0', 5 - len(convert(nvarchar(5),@minor))) + convert(nvarchar(5),@minor)
SELECT @strbuild = REPLICATE('0', 5 - len(convert(nvarchar(5),@build))) + convert(nvarchar(5),@build)
SELECT @strrevision = REPLICATE('0', 5 - len(convert(nvarchar(5),@revision))) + convert(nvarchar(5),@revision)

--select @strmajor + @strminor + @strbuild + @strrevision

return CONVERT(decimal(30,0), @strmajor+@strminor+@strbuild+@strrevision)
end