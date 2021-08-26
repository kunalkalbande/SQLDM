--Function created for baseline Template to get difference seconds from UTC off set time and template off set
--SQLDM 10.1(srishti purohit)

if (object_id('fn_GetSecondsCountUsingOffset') is not null)
begin
drop function fn_GetSecondsCountUsingOffset
end
go
create function fn_GetSecondsCountUsingOffset (@OffsetString nvarchar(1024))  
RETURNS int  
WITH EXECUTE AS CALLER  
AS  
BEGIN  
	DECLARE @SecondsCount int;  
	DECLARE @timeZone nvarchar(10)
	SET @timeZone = RIGHT(@OffsetString,6)
	SET @SecondsCount = 0
	IF (SUBSTRING(@timeZone,1,1) = '-' OR SUBSTRING(@timeZone,1,1) = '+')	
		SET @SecondsCount = (CASE WHEN SUBSTRING(@timeZone,1,1) = '+' THEN -1 ELSE 1 END) * 
							(CONVERT(INT,SUBSTRING(@timeZone,2,2))*3600 
							+ CONVERT(INT,SUBSTRING(@timeZone,5,2)) * 60)		
	
	RETURN(@SecondsCount)
END;  
