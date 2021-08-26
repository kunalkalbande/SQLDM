--Created by Gaurav Karwal - SQLdm 9.0
--Splits a passed string into a table
-- USAGE:SELECT * FROM fn_Split('t,e',',')
IF (object_id('fn_Split') IS NOT NULL)
BEGIN
DROP FUNCTION fn_Split
END
GO
CREATE FUNCTION fn_Split(@StringToBeSplit NVARCHAR(MAX), @CharToSplitFrom NVARCHAR(1))
RETURNS @Splitted TABLE(Value NVARCHAR(MAX),[Index] INT)
AS
BEGIN

DECLARE @Index INT;
DECLARE @CurrWord NVARCHAR(MAX),@ArgLen INT;
SELECT @Index = 1,@CurrWord = '',@ArgLen = LEN(@StringToBeSplit) ;
IF(@StringToBeSplit IS NULL OR @StringToBeSplit ='' OR @CharToSplitFrom IS NULL OR @CharToSplitFrom='') RETURN
	WHILE(@ArgLen +1 >= @Index)
	BEGIN
		DECLARE @CurrChar NVARCHAR(1);
		SELECT @CurrChar = SUBSTRING(@StringToBeSplit,@Index,@Index);
		
		IF(@CurrChar != @CharToSplitFrom)
		BEGIN
			SELECT @CurrWord = @CurrWord + @CurrChar;
			IF((@ArgLen + 1 = @Index)) INSERT INTO @Splitted SELECT @CurrWord, (SELECT COUNT(0) + 1 FROM @Splitted);
		END
		ELSE IF (@CurrChar = @CharToSplitFrom)
		BEGIN
			INSERT INTO @Splitted SELECT @CurrWord, (SELECT COUNT(0) + 1 FROM @Splitted);
			SELECT @CurrWord = '';
		END
		
		SELECT @Index = @Index + 1;
	END
	RETURN;
END