#include "stdafx.h"

void Logger::Log(const wchar_t *m1, const wchar_t *m2)
{
	if(!m_hInstall || !m1 || !m2) { return; }

	std::wstring temp(m1);
	temp += L" : ";
	temp += m2;
	Log(temp.data());
}

void Logger::Log(const wchar_t *msg, DWORD rc)
{
	if(!m_hInstall || !msg) { return; }

	wchar_t buf[100];
	_ltow_s(rc,buf,10);

	std::wstring temp(msg);
	temp += L", rc = ";
	temp += buf;
	Log(temp.data());
}

void Logger::Log(const wchar_t* msg)
{
	if(!m_hInstall || !msg) { return; }

	std::wstring temp = m_BlkPrefix;
	temp += msg;

	PMSIHANDLE newHandle = ::MsiCreateRecord(2);
	::MsiRecordSetString(newHandle, 0, temp.data());
	::MsiProcessMessage(m_hInstall, INSTALLMESSAGE(INSTALLMESSAGE_INFO), newHandle);
}

//void Logger::MyLog(MSIHANDLE msiHandle, const wchar_t *msg)
//{
//	PMSIHANDLE newHandle = ::MsiCreateRecord(2);
//	TCHAR szTemp[MAX_PATH * 2];
//	swprintf(szTemp, L"***%s", msg); // *** prefix is added for clarity in log file to easily identify our entries
//	MsiRecordSetString(newHandle, 0, szTemp);
//	MsiProcessMessage(msiHandle, INSTALLMESSAGE(INSTALLMESSAGE_INFO), newHandle);
//}

void Logger::makePrefix(const wchar_t *block)
{
	m_BlkPrefix = L"****** ";
	if(block)
	{
		m_BlkPrefix += block;
		m_BlkPrefix += L" - ";
	}
}
