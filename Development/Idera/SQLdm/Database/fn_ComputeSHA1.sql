----Function that takes signature text and returns a base binary64 string using SHA1 algorithm-----
IF OBJECT_ID (N'[dbo].[fn_ComputeSHA1]', N'FN') IS NOT NULL  
    DROP FUNCTION fn_ComputeSHA1;  
GO 
CREATE FUNCTION [dbo].[fn_ComputeSHA1]
(
    @InputString VARCHAR(MAX)
)
RETURNS VARCHAR(MAX)
AS
BEGIN
	DECLARE @y VARBINARY(MAX)
	DECLARE @Salt VARCHAR(MAX)
	DECLARE @strlen VARCHAR(15)
	SET @strlen = CAST(LEN(ISNULL(NULLIF(@InputString,''),'-')) AS VARCHAR(15))
	SET @Salt = '2Oy9DUJz4eTCmtgvjt9N4yaIJGnxgRrxqyVTsGZEfyLKuY0TpNOCvTWRvwkS4gWyAi1s34YQbefu3GA5MWSNjk6YjSriP0sbB2HY'
	SET @y =  HASHBYTES('SHA1', @InputString + @Salt + @strlen)
    RETURN (
			SELECT
				CAST(N'' AS XML).value(
					 'xs:base64Binary(sql:column("bin"))'
					 , 'VARCHAR(MAX)'
					 )
					FROM (
					SELECT CAST(@y AS VARBINARY(MAX)) AS bin
					) AS RetVal
			 )
END;