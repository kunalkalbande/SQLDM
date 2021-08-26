
IF (OBJECT_ID('p_GroomExpiredForecasts') IS NOT NULL)
BEGIN
	DROP PROCEDURE [p_GroomExpiredForecasts]
END
GO

CREATE PROCEDURE [dbo].[p_GroomExpiredForecasts]	
AS
BEGIN

delete from PredictiveForecasts where Expiration < GETUTCDATE()

END