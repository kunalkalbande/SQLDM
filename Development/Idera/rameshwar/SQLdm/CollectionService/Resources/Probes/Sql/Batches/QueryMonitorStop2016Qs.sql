-- Batch  Query Monitor Stop Query Store 2016
-- Added new batch for SQLdm 10.4 by Varun Chopra
-- QueryMonitorStop2016Qs.sql
--
-- Query Store Stop will drop the temp table QMQueryStoreState if not exists and is used for clearing the state for query store
-- Generally, QueryMonitorQsState gets refreshed during restart and older values will persist if query store is disabled currently
 IF (object_id('tempdb..QueryMonitorQsState') IS NOT NULL)  
BEGIN
	DROP TABLE tempdb..QueryMonitorQsState;
END
