ALTER SERVER CONFIGURATION SET BUFFER POOL EXTENSION ON
    (FILENAME = '$(BufferPoolExtFilePath)',
		SIZE = $(BufferPoolExtSize)  KB);