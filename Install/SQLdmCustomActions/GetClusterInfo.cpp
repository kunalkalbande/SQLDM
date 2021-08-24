#include "stdafx.h"
// #include <clusapi.h>

typedef BOOL  (WINAPI *LPFN_ISWOW64PROCESS) (HANDLE, PBOOL);

#define CLUSTER_INSTALLED   0x00000001
#define CLUSTER_CONFIGURED  0x00000002
#define CLUSTER_RUNNING     0x00000010

typedef enum NODE_CLUSTER_STATE {
    ClusterStateNotInstalled                = 0x00000000,
    ClusterStateNotConfigured               = CLUSTER_INSTALLED,
    ClusterStateNotRunning                  = CLUSTER_INSTALLED | CLUSTER_CONFIGURED,
    ClusterStateRunning                     = CLUSTER_INSTALLED | CLUSTER_CONFIGURED | CLUSTER_RUNNING
} NODE_CLUSTER_STATE;

typedef DWORD (WINAPI * PCLUSAPI_GET_NODE_CLUSTER_STATE)(__in_opt LPCWSTR lpszNodeName, __out LPDWORD pdwClusterState);

const wchar_t *NODE_CLUSTER_STATUS = L"Node_Cluster_Status";

static DWORD UpdateClusterStatusProperties(Logger &l, MSIHANDLE hModule, const DWORD status)
{
	wchar_t szStatus[20];

	swprintf(szStatus, 20, L"%d", status);
	l.Log(NODE_CLUSTER_STATUS, status);

	DWORD rc = ::MsiSetProperty(hModule, NODE_CLUSTER_STATUS, szStatus);
	l.Log(L"MsiSetProperty", rc);
	return rc;
}

DWORD GetClusterInfo_Wow64(MSIHANDLE hModule, Logger &l)
{
	HKEY hConfigKey = NULL;
	DWORD state = 0;
	DWORD stateLen = sizeof(state);

	l.Log(L"GetClusterInfo_Wow64 entered");

	DWORD rc = RegOpenKeyEx(HKEY_LOCAL_MACHINE,
						L"SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Cluster Server",
						0,
						KEY_QUERY_VALUE | KEY_WOW64_64KEY,
						&hConfigKey);

	if (rc == ERROR_SUCCESS)
	{
		rc = RegQueryValueEx(hConfigKey, L"ClusterInstallationState", NULL, NULL, (LPBYTE)&state, &stateLen);
		l.Log(L"RegQueryValueEx for ClusterInstallationState", rc);
		if (rc == ERROR_SUCCESS && stateLen == sizeof(state))
		{
			if (state >= ClusterStateNotRunning)
			{
				SC_HANDLE hScm = ::OpenSCManager(0,0,GENERIC_READ);
				if(!hScm)
				{
					rc = ::GetLastError();
					l.Log(L"ERROR - opening SC Manager", rc);
				} else
				{
					SC_HANDLE hService = ::OpenService(hService, L"ClusSvc", GENERIC_READ);
					if(!hService)
					{
						rc = ::GetLastError();
						l.Log(L"ERROR - opening service", rc);
						UpdateClusterStatusProperties(l, hModule, ClusterStateNotInstalled);
					} 
					else
					{
						// Allocate initial query buffer.
						DWORD statBufSize = 4096, bytesNeeded = 0;
						BYTE *statConfig = new BYTE[statBufSize];
						if(!statConfig)
						{
							l.Log(L"ERROR - allocating service status buffer");
							rc = ERROR_OUTOFMEMORY;
						}
						else
						if (!::QueryServiceStatusEx(hService, SC_STATUS_PROCESS_INFO, statConfig, statBufSize, &bytesNeeded))
						{
							rc = ::GetLastError();
							if(rc == ERROR_INSUFFICIENT_BUFFER)
							{
								// Reallocate buffer to new size.
								rc = ERROR_SUCCESS;
								delete [] statConfig;
								statBufSize = bytesNeeded;
								statConfig = new BYTE[statBufSize];
								if(!statConfig)
								{
									l.Log(L"ERROR - re-allocating service status buffer");
									rc = ERROR_OUTOFMEMORY;
								}

								if(rc == ERROR_SUCCESS)
								{
									if(!::QueryServiceStatusEx(hService, SC_STATUS_PROCESS_INFO, statConfig, statBufSize, &bytesNeeded))
									{
										rc = ::GetLastError();
										l.Log(L"ERROR - querying service status", rc);
									}
								}
							} 
							else
								l.Log(L"ERROR - querying service status", rc);
						}

						if (rc == ERROR_SUCCESS)
							UpdateClusterStatusProperties(l, hModule, ((SERVICE_STATUS_PROCESS *)statConfig)->dwCurrentState);
						else
							UpdateClusterStatusProperties(l, hModule, ClusterStateNotInstalled);

						if (statConfig)
							delete [] statConfig;
					}
				}
			}
			else
			{
				l.Log(L"ClusterInstallationState (rc=state)", rc);
				UpdateClusterStatusProperties(l, hModule, ClusterStateNotInstalled);
			}
		}
		else
		{
			l.Log(L"ERROR - open getting ClusterInstallationState value from 64 bit registry", rc);
			UpdateClusterStatusProperties(l, hModule, ClusterStateNotInstalled);
		}

		RegCloseKey(hConfigKey);
	}
	else
	{
		l.Log(L"ERROR - open registry key HKLM\\SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Cluster Server", rc);
		UpdateClusterStatusProperties(l, hModule, ClusterStateNotInstalled);
	}

	l.Log(L"GetClusterInfo_Wow64 exited");

	return rc;
}

DWORD _stdcall GetClusterInfo(MSIHANDLE hModule)
{
	static HMODULE hKernel  = LoadLibrary(L"kernel32.dll");
	static HMODULE hCluster = LoadLibrary(L"clusapi.dll");

	static LPFN_ISWOW64PROCESS fnIsWow64Process = (hKernel != NULL) 
			? (LPFN_ISWOW64PROCESS)GetProcAddress(hKernel, "IsWow64Process") 
			: NULL;

	static PCLUSAPI_GET_NODE_CLUSTER_STATE fnGetNodeClusterState = (hModule != NULL) 
			? (PCLUSAPI_GET_NODE_CLUSTER_STATE)GetProcAddress(hCluster, "GetNodeClusterState") 
			: NULL;

	Logger l(hModule,L"GetClusterInfo");

	BOOL  bIsWow64 = FALSE;
	DWORD state = ClusterStateNotInstalled;
	
    if (NULL != fnIsWow64Process)
    {
        if (fnIsWow64Process(GetCurrentProcess(),&bIsWow64))
        {
			if (bIsWow64)
				return GetClusterInfo_Wow64(hModule, l);
			else
				l.Log(L"IsWow64 is false");
		} else
			l.Log(L"IsWow64Process returned false");
    } else
		l.Log(L"IsWow64Process not supported");
	
	if (NULL != fnGetNodeClusterState)
	{
		fnGetNodeClusterState(NULL, &state);	
	}
	
	return UpdateClusterStatusProperties(l, hModule, state);
}