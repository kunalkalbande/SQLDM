--------------------------------------------------------------------------------
--  Batch: MirroringSuspendResume
--  Variables: 
--		[0] - Target Database
--		[1] - Action
--------------------------------------------------------------------------------

use master
ALTER DATABASE [{0}] SET PARTNER {1};