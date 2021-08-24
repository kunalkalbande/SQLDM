#include "stdafx.h"

const wchar_t *MS_SERVICE_NAME				= L"SQLdmManagementService$Default";
const wchar_t *CS_SERVICE_NAME				= L"SQLdmCollectionService$Default";
const wchar_t *MS_SERVICE_ACCOUNT			= L"Previous_Service_Manage";
const wchar_t *CS_SERVICE_ACCOUNT			= L"Previous_Service_Collect";
const wchar_t *SERVICE_ACCOUNTS_MATCH		= L"Previous_Service_Match";
const wchar_t *MS_SERVICE_START_TYPE		= L"PREVIOUS_SERVICE_MANAGE_START";
const wchar_t *CS_SERVICE_START_TYPE		= L"PREVIOUS_SERVICE_COLLECT_START";
const wchar_t *YES_SERVICE_ACCOUNTS_MATCH	= L"1";
const wchar_t *NO_SERVICE_ACCOUNTS_MATCH	= L"0";
const wchar_t *ERROR_SERVICE_ACCOUNTS_MATCH = L"-1";

static DWORD updateMsiProperties (
		Logger &l,
		MSIHANDLE hModule,
		const wchar_t *MSAccount,
		const wchar_t *CSAccount,
		const DWORD   MSStartType,
		const DWORD   CSStartType
	)
{
	assert(MSAccount != NULL && CSAccount != NULL);

	// Create the compare property value.
	const wchar_t *compareResult = ::_wcsicmp(MSAccount, CSAccount) == 0 ? 
								YES_SERVICE_ACCOUNTS_MATCH : NO_SERVICE_ACCOUNTS_MATCH;

	wchar_t msStartType[20];
	wchar_t csStartType[20];

	// Log what properties are being set.
	l.Log(MS_SERVICE_ACCOUNT, MSAccount);
	l.Log(CS_SERVICE_ACCOUNT, CSAccount);
	l.Log(SERVICE_ACCOUNTS_MATCH, compareResult);

	swprintf(msStartType, 20, L"%d", MSStartType);
	l.Log(MS_SERVICE_START_TYPE, MSStartType);

	swprintf(csStartType, 20, L"%d", CSStartType);
	l.Log(CS_SERVICE_START_TYPE, CSStartType);


	// Update MSI properties.
	DWORD rc = ::MsiSetProperty(hModule, MS_SERVICE_ACCOUNT, MSAccount);
	if(rc == ERROR_SUCCESS)
	{
		rc = ::MsiSetProperty(hModule, CS_SERVICE_ACCOUNT, CSAccount);
		if(rc == ERROR_SUCCESS)
		{
			rc = ::MsiSetProperty(hModule, SERVICE_ACCOUNTS_MATCH, compareResult);
			if(rc == ERROR_SUCCESS)
			{
				rc = ::MsiSetProperty(hModule, MS_SERVICE_START_TYPE, msStartType);
				if(rc == ERROR_SUCCESS)
				{
					rc = ::MsiSetProperty(hModule, CS_SERVICE_START_TYPE, csStartType);
					if(rc == ERROR_SUCCESS)
					{
					}
					else
					{
						l.Log(L"ERROR - failed to set collection service start type result property", rc);
					}
				}
				else
				{
					l.Log(L"ERROR - failed to set management service start type result property", rc);
				}
			}
			else
			{
				l.Log(L"ERROR - failed to set service account match result property", rc);
			}
		}
		else
		{
			l.Log(L"ERROR - failed to set CS service account property", rc);
		}
	}
	else
	{
		l.Log(L"ERROR - failed to set MS service account property", rc);
	}

	return rc;
}

static DWORD getServiceAccount (
		Logger &l,
		SC_HANDLE hScm,
		const wchar_t *serviceName,
		std::wstring &svcAcct,
		DWORD &startType
	)
{
	assert(hScm != NULL && serviceName != NULL);
	DWORD rc = ERROR_SUCCESS;

	// Open the Management service and get the service account.
	SC_HANDLE hMS = ::OpenService(hScm,serviceName,GENERIC_READ);
	if(!hMS)
	{
		rc = ::GetLastError();
		l.Log(L"ERROR - opening service", rc);
	}

	// Query for service account.
	if(rc == ERROR_SUCCESS)
	{
		// Allocate initial query buffer.
		DWORD svcConfigBufSize = 4096, bytesNeeded = 0;
		BYTE *svcConfig = new BYTE[svcConfigBufSize];
		if(!svcConfig)
		{
			l.Log(L"ERROR - allocating service config buffer");
			rc = ERROR_OUTOFMEMORY;
		}

		// Query for service config.
		if(rc == ERROR_SUCCESS)
		{
			if(!::QueryServiceConfig(hMS, (LPQUERY_SERVICE_CONFIG)svcConfig, svcConfigBufSize, &bytesNeeded))
			{
				rc = ::GetLastError();
				if(rc == ERROR_INSUFFICIENT_BUFFER)
				{
					// Reallocate buffer to new size.
					rc = ERROR_SUCCESS;
					delete [] svcConfig;
					svcConfigBufSize = bytesNeeded;
					svcConfig = new BYTE[svcConfigBufSize];
					if(!svcConfig)
					{
						l.Log(L"ERROR - re-allocating service config buffer");
						rc = ERROR_OUTOFMEMORY;
					}

					// Query for service config info.
					if(rc == ERROR_SUCCESS)
					{
						if(!::QueryServiceConfig(hMS, (LPQUERY_SERVICE_CONFIG)svcConfig, svcConfigBufSize, &bytesNeeded))
						{
							rc = ::GetLastError();
							l.Log(L"ERROR - querying service config (after realloc)", rc);
						}
					}
				}
			}
		}

		// Get service account from config.
		if(rc == ERROR_SUCCESS)
		{
			svcAcct = ((LPQUERY_SERVICE_CONFIG)svcConfig)->lpServiceStartName;
			startType = ((LPQUERY_SERVICE_CONFIG)svcConfig)->dwStartType;
		}

		::CloseServiceHandle(hMS);
	}

	return rc;
}



DWORD _stdcall GetSQLdmServiceAccounts (
		MSIHANDLE hModule
	)
{
	Logger l(hModule,L"GetSQLdmServiceAccounts");
	DWORD rc = ERROR_SUCCESS;

	// Get the SQLdm Management/Collection Service service accounts.
	SC_HANDLE hScm = ::OpenSCManager(0,0,GENERIC_READ);
	if(!hScm)
	{
		rc = ::GetLastError();
		l.Log(L"ERROR - opening SC Manager", rc);
	}

	// Get service accounts and log them.
	DWORD msStart = SERVICE_AUTO_START;
	DWORD csStart = SERVICE_AUTO_START;
	std::wstring msAcct, csAcct;

	if(rc == ERROR_SUCCESS)
	{
		rc = getServiceAccount(l,hScm,MS_SERVICE_NAME,msAcct,msStart);
		if(rc == ERROR_SUCCESS)
		{
			rc = getServiceAccount(l,hScm,CS_SERVICE_NAME,csAcct,csStart);
			if(rc != ERROR_SUCCESS)
			{
				l.Log(L"ERROR - reading CS svc acct", rc);
			}
		}
		else
		{
			l.Log(L"ERROR - reading MS svc acct", rc);
		}

		if(rc == ERROR_SUCCESS)
		{
			l.Log (L"MS Acct : ", msAcct.data());
			l.Log (L"CS Acct : ", csAcct.data());
		}

		::CloseServiceHandle(hScm);
	}

	// Log accounts and update msi properties.
	if(rc == ERROR_SUCCESS)
	{
		rc = updateMsiProperties(l, hModule, msAcct.data(), csAcct.data(), msStart, csStart);
	}

	return rc;
}

DWORD _ConfigureServiceStartup(Logger &l, MSIHANDLE hModule, const wchar_t *serviceName, DWORD dwStartType)
{
	DWORD rc = ERROR_SUCCESS;

	// sanity check
	if (dwStartType < SERVICE_AUTO_START || dwStartType > SERVICE_DISABLED)
		dwStartType = SERVICE_AUTO_START;

	// Get the SQLdm Management/Collection Service service accounts.
	SC_HANDLE hScm = ::OpenSCManager(0,0,GENERIC_READ | GENERIC_WRITE);
	if(!hScm)
	{
		rc = ::GetLastError();
		l.Log(L"ERROR - opening SC Manager", rc);
		return rc;
	}

	// Open the service.
	SC_HANDLE hSvc = ::OpenService(hScm,serviceName,GENERIC_READ | GENERIC_WRITE | GENERIC_EXECUTE);
	if (hSvc)
	{
		BOOL success = ChangeServiceConfig(hSvc, 
										 SERVICE_NO_CHANGE,  
										 dwStartType,
										 SERVICE_NO_CHANGE,
										 NULL,
										 NULL,
										 NULL,
										 NULL,
										 NULL,
										 NULL,
										 NULL);
		if (success)
		{
			if (dwStartType == SERVICE_AUTO_START)
			{
				if (!StartService(hSvc, 0, NULL))
				{
					rc = GetLastError();
					if (rc == ERROR_SERVICE_ALREADY_RUNNING)
						l.Log(L"ERROR starting service - It's already running");
					else
						l.Log(L"ERROR - starting service", rc);

					// don't return start error since it will rollback the install
					rc = ERROR_SUCCESS;
				}
			}
		}
		else
		{
			rc = GetLastError();
			l.Log(L"ERROR - updating service start type", rc);
		} 
	} else
	{
			rc = GetLastError();
			l.Log(L"ERROR - opening service for update", rc);
	}

	::CloseServiceHandle(hScm);
	
	return rc;
}

DWORD _stdcall ConfigureCollectionServiceStartup(MSIHANDLE hModule){
	Logger l(hModule,L"ConfigureCollectionServiceStartup");

	std::wstring start_type;
	DWORD dwStartType = SERVICE_AUTO_START;

	DWORD rc = Utility::ReadMsiProperty(hModule, CS_SERVICE_START_TYPE, start_type);
	if(rc == ERROR_SUCCESS)
	{
		l.Log(L"Previous_Service_Collect_Start length", start_type.length());
		if (start_type.length() == 1)
		{
			dwStartType = _wtoi(start_type.data());
			l.Log(L"Got new cs service start type", dwStartType);
		}
	} else
		l.Log(L"Error calling MsiGetProperty 'Previous_Service_Collect_Start'", rc);

	return _ConfigureServiceStartup(l, hModule, CS_SERVICE_NAME, dwStartType); 
}

DWORD _stdcall ConfigureManagementServiceStartup(MSIHANDLE hModule)
{
	Logger l(hModule,L"ConfigureManagementServiceStartup");

	std::wstring start_type;
	DWORD dwStartType = SERVICE_AUTO_START;

	DWORD rc = Utility::ReadMsiProperty(hModule, MS_SERVICE_START_TYPE, start_type);
	if(rc == ERROR_SUCCESS)
	{
		l.Log(L"Previous_Service_Manage_Start length", start_type.length());
		if (start_type.length() == 1)
		{
			dwStartType = _wtoi(start_type.data());
			l.Log(L"Got new ms service start type", dwStartType);
		}
	}
	else
		l.Log(L"Error calling MsiGetProperty 'Previous_Service_Manage_Start'", rc);

	return _ConfigureServiceStartup(l, hModule, MS_SERVICE_NAME, dwStartType); 
}
