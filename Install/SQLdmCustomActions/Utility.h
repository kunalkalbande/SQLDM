#pragma once

namespace Utility
{
	DWORD ReadMsiProperty (
			MSIHANDLE hModule,
			const wchar_t *propertyNameIn,
			std::wstring &valueOut
		);
	DWORD FileExists (
			MSIHANDLE hModule,
			const wchar_t *fileIn
		);
}