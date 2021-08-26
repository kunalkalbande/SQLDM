--------------------------------------------------------------
-- SQLDM 11 - Recommended changes for Azure SQL Database v12
--------------------------------------------------------------

--RRG: Only works on User Databases for Azure SQL Database

;with xmlnamespaces 
   (default 'http://schemas.microsoft.com/sqlserver/2004/07/showplan') 
select top 100
   stmt.value('(@StatementText)[1]', 'varchar(max)') as Text, 
   stmt.value('(@StatementEstRows)[1]', 'varchar(max)') as EstRows
from sys.dm_exec_cached_plans as cp 
    cross apply sys.dm_exec_query_plan(plan_handle) as qp 
    cross apply query_plan.nodes('/ShowPlanXML/BatchSequence/Batch/Statements/StmtSimple') as batch(stmt) 