#pragma once

extern DWORD _stdcall GetClusterInfo (MSIHANDLE);
extern DWORD _stdcall GetSQLdmServiceAccounts (MSIHANDLE);
extern DWORD _stdcall ConfigureMS (MSIHANDLE);
extern DWORD _stdcall UninstallMS (MSIHANDLE);
extern DWORD _stdcall ConfigureCS (MSIHANDLE);
extern DWORD _stdcall UninstallCS (MSIHANDLE);