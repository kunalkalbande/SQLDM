IF (OBJECT_ID('p_GetPredictiveForecasts') IS NOT NULL)
BEGIN
      DROP PROCEDURE [p_GetPredictiveForecasts]
END
GO

CREATE PROCEDURE [dbo].[p_GetPredictiveForecasts] 
      @SQLServerID int  
AS
BEGIN 
      
      IF @SQLServerID = -1
      BEGIN
      
            SELECT      b.InstanceName as 'ServerName',  c.Name as 'MetricName', Severity, Timeframe, Forecast, 
						case
							when Forecast = 0 then 0.05
							else Accuracy
						end as Accuracy, 
						Expiration 
            FROM  PredictiveForecasts a
                        INNER JOIN MonitoredSQLServers b ON a.SQLServerID = b.SQLServerID
                        INNER JOIN MetricInfo          c ON a.Metric = c.Metric
                        INNER JOIN MetricThresholds    d ON c.Metric = d.Metric and b.SQLServerID = d.SQLServerID
            WHERE
                        d.Enabled = 1
                        AND b.Active = 1
      
      END
      ELSE
      BEGIN
            
            SELECT      b.InstanceName as 'ServerName', c.Name as 'MetricName', Severity, Timeframe, Forecast, 
						case
							when Forecast = 0 then 0.05
							else Accuracy
						end as Accuracy, 
						Expiration 
            FROM  PredictiveForecasts a
                        INNER JOIN MonitoredSQLServers b ON a.SQLServerID = b.SQLServerID
                        INNER JOIN MetricInfo          c ON a.Metric = c.Metric
                        INNER JOIN MetricThresholds    d ON c.Metric = d.Metric and b.SQLServerID = d.SQLServerID
            WHERE a.SQLServerID = @SQLServerID        
                        AND d.Enabled = 1
                        AND b.Active = 1
            
      END   

      DECLARE @err INT
      SELECT  @err = @@error
      RETURN  @err
      
END