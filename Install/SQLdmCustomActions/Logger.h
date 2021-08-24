#pragma once


class Logger
{
public:
	Logger(
		MSIHANDLE hInstall,
		const wchar_t *block
	) 
	: m_hInstall(hInstall)
	{
		makePrefix(block);
		Log(L"Entered");
	}
	~Logger()
	{
		Log(L"Exiting");
	}

	void Log(const wchar_t *m1, const wchar_t *m2);
	void Log(const wchar_t *msg, DWORD rc);
	void Log(const wchar_t* msg);
	static void MyLog(MSIHANDLE msiHandle, const wchar_t *msg);

private:
	Logger();
	Logger(const Logger&);
	Logger& operator=(const Logger&);

	void makePrefix(const wchar_t *block);

private:
	MSIHANDLE m_hInstall;
	std::wstring m_BlkPrefix;
};
