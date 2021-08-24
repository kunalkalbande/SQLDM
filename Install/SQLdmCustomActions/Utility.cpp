#include "stdafx.h"

DWORD Utility::ReadMsiProperty (
			MSIHANDLE hModule,
			const wchar_t *nameIn,
			std::wstring &valueOut
		)
{
	Logger l(hModule,L"Utility::ReadMsiProperty");
	DWORD rc = ERROR_SUCCESS;

	// Validate input.
	if(nameIn == 0 || nameIn[0] == 0)
	{
		rc = ERROR_BAD_ARGUMENTS;
		l.Log (L"ERROR - property name is invalid");
	}

	// Read the property.
	if(rc == ERROR_SUCCESS)
	{
		DWORD valueSize = 0;
		rc =  ::MsiGetProperty(hModule, nameIn, L"", &valueSize);
		if (rc == ERROR_MORE_DATA)
		{
			valueSize += 1; // value size needs to be incremented, otherwise there will be error_more_data error.
			wchar_t *value = new wchar_t[valueSize];
			if (value)
			{
				rc = MsiGetProperty(hModule, nameIn, value, &valueSize);
				if (rc == ERROR_SUCCESS)
				{
					valueOut = value;
				}
				else
				{
					std::wstring msg = L"ERROR - failed to read property ";
					msg += (nameIn != 0 && nameIn[0] != 0) ? nameIn : L"EMPTY PROPERTY";
					l.Log(msg.data(), rc);
				}

				// Cleanup.
				delete [] value;
			}
			else
			{
				l.Log(L"ERROR - allocating value buffer");
				rc = ERROR_OUTOFMEMORY;
			}
		}
	}

	return rc;
}

DWORD Utility::FileExists (
		MSIHANDLE hModule,
		const wchar_t *fileIn
	)
{
	Logger l(hModule,L"Utility::DoesFileExist");
	DWORD rc = ERROR_SUCCESS;

	// Validate input.
	if(fileIn == 0 || fileIn[0] == 0)
	{
		rc = ERROR_BAD_ARGUMENTS;
		l.Log (L"ERROR - file name is invalid");
	}

	// Check if file exists.
	if(rc == ERROR_SUCCESS)
	{
		// Search the file
		WIN32_FIND_DATA findData;
		::ZeroMemory(&findData, sizeof(findData));
		HANDLE hFind = ::FindFirstFile (fileIn, &findData);
		if ( hFind != INVALID_HANDLE_VALUE )
		{
			::FindClose( hFind );
		}
		else
		{
			rc = ::GetLastError();
			std::wstring msg = L"ERROR - file <";
			msg += fileIn;
			msg += L"> not found";
			l.Log(msg.data(), rc);
		}
	}

	return rc;
}
