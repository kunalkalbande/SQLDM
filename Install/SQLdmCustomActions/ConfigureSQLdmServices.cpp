#include "stdafx.h"

const wchar_t *MSI_CUSTOM_ACTION_DATA_PROP = L"CustomActionData";
const wchar_t *ARGS_TOKEN				   = L"/Args";
const wchar_t *BEG_TOKEN				   = L"=\"";
const wchar_t *END_TOKEN				   = L"\"";

const wchar_t *MS_CONFIG_EXE			= L"SQLdmManagementService.exe";
const wchar_t *RS_CONFIG_EXE			= L"SQLdmRegistrationService.exe";
const wchar_t *CS_CONFIG_EXE			= L"SQLdmCollectionService.exe";
const wchar_t *AS_CONFIG_EXE			= L"SQLdmAnalyticsService.exe";
const wchar_t *MS_CONFIG_INSTALL_LOG	= L"SQLdmManagementService.InstallLog";
const wchar_t *RS_CONFIG_INSTALL_LOG	= L"SQLdmRegistrationService.InstallLog";
const wchar_t *CS_CONFIG_INSTALL_LOG	= L"SQLdmCollectionService.InstallLog";
const wchar_t *AS_CONFIG_INSTALL_LOG	= L"SQLdmAnalyticsService.InstallLog";

static DWORD getCustomActionDataProperties (
		MSIHANDLE hModule,
		std::wstring &installDir,
		std::wstring &cmdLineArgs
	)
{
	Logger l(hModule,L"getCustomActionDataProperties");
	DWORD rc = ERROR_SUCCESS;

	// Read custom action data property.
	std::wstring customActionData;
	rc = Utility::ReadMsiProperty(hModule, MSI_CUSTOM_ACTION_DATA_PROP, customActionData);
	if(rc != ERROR_SUCCESS)
	{
		l.Log(L"ERROR - failed to read custom action data property", rc);
	}

	// Parse the install dir and command line args.
	if(rc == ERROR_SUCCESS)
	{
		// Parse the dir and command line args.  NOTE: this is very position dependent
		// and the installer needs to send in data correctly.
		std::wstring dirTok, argsTok;
		size_t argsPos = std::wstring::npos, 
			   dirBegPos = std::wstring::npos, dirEndPos = std::wstring::npos,
			   argsBegPos = std::wstring::npos, argsEndPos = std::wstring::npos;

		// Split this property into dir and args tokens.
		argsPos = customActionData.find(ARGS_TOKEN);
		if(argsPos != std::wstring::npos)
		{
			// Get dir and args tokens.
			dirTok = customActionData.substr(0, argsPos);
			argsTok = customActionData.substr(argsPos, customActionData.length());

			// Find dir and args value begin and end positions.
			dirBegPos = dirTok.find(BEG_TOKEN) + 2;
			dirEndPos = dirTok.find_last_of(END_TOKEN);
			argsBegPos = argsTok.find(BEG_TOKEN) + 2;
			argsEndPos = argsTok.find_last_of(END_TOKEN);
			if(dirBegPos != std::wstring::npos && dirEndPos != std::wstring::npos
			   && argsBegPos != std::wstring::npos && argsEndPos != std::wstring::npos)
			{
				installDir = dirTok.substr(dirBegPos,dirEndPos-dirBegPos);
				cmdLineArgs = argsTok.substr(argsBegPos,argsEndPos-argsBegPos);
			}
			else
			{
				rc = ERROR_BAD_ARGUMENTS;
				l.Log(L"ERROR - the custom action data format is incorrect, unable to extract install dir and command line args.");
			}
		}
		else
		{
			rc = ERROR_BAD_ARGUMENTS;
			l.Log(L"ERROR - the custom action data format is incorrect, unable to tokenize into dir and arg tokens");
		}
	}

	return rc;
}

static DWORD runConfig (
		MSIHANDLE      hModule,
		const wchar_t *dirIn,
		const wchar_t *exeIn,
		const wchar_t *argsIn,
		const wchar_t *logIn
	)
{
	Logger l(hModule,L"runConfig");
	DWORD rc = ERROR_SUCCESS;

	// Validate input.
	if(dirIn == 0 || dirIn[0] == 0 || exeIn == 0 || exeIn[0] == 0 || argsIn == 0 || argsIn[0] == 0 || logIn == 0 || logIn[0] == 0)
	{
		rc = ERROR_BAD_ARGUMENTS;
		l.Log (L"ERROR - invalid input");
	}

	// Construct the path and verify that exe exists.
	std::wstring exePath, logPath;
	if(rc == ERROR_SUCCESS)
	{
		// Construct paths.
		exePath = logPath = dirIn;
		exePath += exeIn;
		logPath += logIn;

		// Check if file exists.
		rc = Utility::FileExists(hModule,exePath.data());
	}

	// Call the exe and get the return code.
	if(rc == ERROR_SUCCESS)
	{
		// Make a copy of the args input var.
		size_t argsCopyLen = exePath.length() + 3 + ::wcslen(argsIn) + 1;
		wchar_t *argsCopy = new wchar_t[argsCopyLen];
		if(argsCopy)
		{
			argsCopy[0] = 0;
			::wcscat_s(argsCopy,argsCopyLen,L"\"");
			::wcscat_s(argsCopy,argsCopyLen,exePath.data());
			::wcscat_s(argsCopy,argsCopyLen,L"\" ");
			::wcscat_s(argsCopy,argsCopyLen,argsIn);
		}
		else
		{
			rc = ERROR_OUTOFMEMORY;
			l.Log (L"ERROR - failed to allocate memory for args copy");
		}

		// Call the exe and get the exit code.
		if(rc == ERROR_SUCCESS)
		{
			// Call the exe.
			STARTUPINFO si;
			PROCESS_INFORMATION pi;
			ZeroMemory( &si, sizeof(si) );
			ZeroMemory( &pi, sizeof(pi) );
			si.cb = sizeof(si);
			if(!::CreateProcess(exePath.data(), argsCopy, NULL, NULL, 0, CREATE_NO_WINDOW, 0, 0, &si, &pi)) 
			{
				rc = ::GetLastError();
				l.Log(L"ERROR - Failed to create process", rc);
			}

			if(rc == ERROR_SUCCESS)
			{
				// Wait for 5 minutes for the process to exit.
				DWORD waitCode = ::WaitForSingleObject(pi.hProcess, 5*60*1000);
				switch(waitCode)
				{
					case WAIT_OBJECT_0: 
						if(::GetExitCodeProcess(pi.hProcess,&rc))
						{
							l.Log(L"INFO - process finished with exit code", rc);
						}
						else
						{
							rc = ::GetLastError();
							l.Log(L"ERROR - failed to get the process exit code", rc);
						}
						break;
					case WAIT_TIMEOUT:
						rc = ERROR_INVALID_FUNCTION;
						l.Log(L"ERROR - the process did not complete in 5 minutes.");
						break;
					case WAIT_FAILED:
						rc = ::GetLastError();
						l.Log(L"ERROR - wait for process to complete failed", rc);
						break;
					default:
						rc = ERROR_INVALID_FUNCTION;
						l.Log(L"ERROR - unknown wait return code", waitCode);
						break;
				}

				// Close process and thread handles. 
				::CloseHandle(pi.hProcess);
				::CloseHandle(pi.hThread);
			}

			// Deallocate args copy buffer.
			delete [] argsCopy;

			// Delete the log file, if it exists.
			if(rc == ERROR_SUCCESS)
			{
				DWORD tempRc = Utility::FileExists(hModule,logPath.data());
				if(tempRc == ERROR_SUCCESS)
				{
					if(!::DeleteFile(logPath.data()))
					{
						std::wstring msg (L"ERROR - install log file <");
						msg += logPath;
						msg += L"> delete failed";
						l.Log (msg.data(),::GetLastError());
					}
				}
				else
				{
					std::wstring msg (L"WARNING - install log file <");
					msg += logPath;
					msg += L"> does not exist";
					l.Log (msg.data(), tempRc);
				}
			}
		}
	}

	return rc;
}

DWORD _stdcall ConfigureManagementService (
		MSIHANDLE hModule
	)
{
	Logger l(hModule,L"ConfigureManagementService");
	DWORD rc = ERROR_SUCCESS;

	// Retrieve the custom action data properties.
	std::wstring installDir, cmdLineArgs;
	rc = getCustomActionDataProperties(hModule,installDir,cmdLineArgs);
	if(rc != ERROR_SUCCESS)
	{
		l.Log(L"ERROR - failed to read custom action data properties",rc);
	}

	// Run MS config.
	if(rc == ERROR_SUCCESS)
	{
		rc = runConfig(hModule, installDir.data(), MS_CONFIG_EXE, cmdLineArgs.data(), MS_CONFIG_INSTALL_LOG);
	}

	return rc;
}

DWORD _stdcall ConfigureRestService (
		MSIHANDLE hModule
	)
{
	Logger l(hModule,L"ConfigureRestService");
	DWORD rc = ERROR_SUCCESS;

	// Retrieve the custom action data properties.
	std::wstring installDir, cmdLineArgs;
	rc = getCustomActionDataProperties(hModule,installDir,cmdLineArgs);
	if(rc != ERROR_SUCCESS)
	{
		l.Log(L"ERROR - failed to read custom action data properties",rc);
	}

	// Run Rest Services config.
	if(rc == ERROR_SUCCESS)
	{
		rc = runConfig(hModule, installDir.data(), RS_CONFIG_EXE, cmdLineArgs.data(), RS_CONFIG_INSTALL_LOG);
	}

	if(rc != ERROR_SUCCESS)
	{
		rc = ERROR_SUCCESS;
		l.Log(L"ERROR - failed to configure and start the Rest Services ",rc);
	}

	return rc;
}

DWORD _stdcall UninstallManagementService (
		MSIHANDLE hModule
	)
{
	Logger l(hModule,L"UninstallManagementService");
	DWORD rc = ERROR_SUCCESS;

	// Retrieve the following properties needed for command line args
	// InstanceName - INST_NAME_MS
	// Force

	return rc;
}

DWORD _stdcall UninstallRestService (
		MSIHANDLE hModule
	)
{
	Logger l(hModule,L"UninstallRestService");
	DWORD rc = ERROR_SUCCESS;

	// Retrieve the following properties needed for command line args
	// InstanceName - INST_NAME_MS
	// Force

	return rc;
}

DWORD _stdcall ConfigureCollectionService (
		MSIHANDLE hModule
	)
{
	Logger l(hModule,L"ConfigureCollectionService");
	DWORD rc = ERROR_SUCCESS;

	// Retrieve the custom action data properties.
	std::wstring installDir, cmdLineArgs;
	rc = getCustomActionDataProperties(hModule,installDir,cmdLineArgs);
	if(rc != ERROR_SUCCESS)
	{
		l.Log(L"ERROR - failed to read custom action data properties",rc);
	}

	// Run CS config.
	if(rc == ERROR_SUCCESS)
	{
		rc = runConfig(hModule, installDir.data(), CS_CONFIG_EXE, cmdLineArgs.data(), CS_CONFIG_INSTALL_LOG);
	}
	
	return rc;
}

DWORD _stdcall UninstallCollectionService (
		MSIHANDLE hModule
	)
{
	Logger l(hModule,L"UninstallCollectionService");
	DWORD rc = ERROR_SUCCESS;

	// Retrieve the following properties needed for command line args
	// InstanceName - INST_NAME_CS
	// Force 
	return rc;
}

DWORD _stdcall ConfigureAnalyticsService (
		MSIHANDLE hModule
	)
{
	Logger l(hModule,L"ConfigureAnalyticsService");
	DWORD rc = ERROR_SUCCESS;

	// Retrieve the custom action data properties.
	std::wstring installDir, cmdLineArgs;
	rc = getCustomActionDataProperties(hModule,installDir,cmdLineArgs);
	if(rc != ERROR_SUCCESS)
	{
		l.Log(L"ERROR - failed to read custom action data properties",rc);
	}

	// Run CS config.
	if(rc == ERROR_SUCCESS)
	{
		rc = runConfig(hModule, installDir.data(), AS_CONFIG_EXE, cmdLineArgs.data(), AS_CONFIG_INSTALL_LOG);
	}
	
	return rc;
}

DWORD _stdcall UninstallAnalyticsService (
		MSIHANDLE hModule
	)
{
	Logger l(hModule,L"UninstallAnalyticsService");
	DWORD rc = ERROR_SUCCESS;

	// Retrieve the following properties needed for command line args
	// InstanceName - INST_NAME_CS
	// Force 
	return rc;
}

