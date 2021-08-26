
--SQLDM-28629 Creating Clustered(UTCCollectionDate) and Non-clustered(UTCCollectionDateTime) Index on table AlwaysOnStatistics.
CREATE INDEX CPF_IX_AlwaysOnStatistics_UTCCollectionDate ON [dbo].[AlwaysOnStatistics]([UTCCollectionDateTime])
CREATE NONCLUSTERED INDEX CPF_X_AlwaysOnStatistics_UTCCollectionDateTime ON [dbo].[AlwaysOnStatistics]([UTCCollectionDateTime])
