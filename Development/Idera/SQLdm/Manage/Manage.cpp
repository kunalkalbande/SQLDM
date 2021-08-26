// Idera.SQLsecure.Core.Common.cpp : Defines the entry point for the DLL application.
//
#include "Manage.h"
#include "CapiProvider.h"

#ifdef _MANAGED
#pragma managed(push, off)
#endif

enum CryptMode
{
	CRYPTMODE_CBC	= 1,
	CRYPTMODE_ECB	= 2
};


BOOL APIENTRY DllMain( HMODULE hModule,
                       DWORD  ul_reason_for_call,
                       LPVOID lpReserved
					 )
{
	switch (ul_reason_for_call)
	{
	case DLL_PROCESS_ATTACH:
	case DLL_THREAD_ATTACH:
	case DLL_THREAD_DETACH:
	case DLL_PROCESS_DETACH:
		break;
	}
    return TRUE;
}

#ifdef _MANAGED
#pragma managed(pop)
#endif

__int32 Crypt(const BYTE* lpInBuffer, __int32 nInSize, BYTE* lpOutBuffer, _int32 nOutSize, unsigned char *key, __int32 nKeySize, unsigned char* IV, bool encrypt)
{
	try
	{	
		//128, 192 and 256 are the only valid keysizes
		if (nKeySize != KEYSIZE_128 && nKeySize != KEYSIZE_192 && nKeySize != KEYSIZE_256)
			return 0;
		
		AesCapiProvider* provider = NULL;

		switch (nKeySize)
		{
			case KEYSIZE_128:
				provider = new AesCapiProvider(CALG_AES_128, CRYPTMODE_CBC, KEYSIZE_128);
				break;
			case KEYSIZE_192:
				provider = new AesCapiProvider(CALG_AES_192, CRYPTMODE_CBC, KEYSIZE_192);
				break;
			case KEYSIZE_256:
				provider = new AesCapiProvider(CALG_AES_256, CRYPTMODE_CBC, KEYSIZE_256);
				break;
		};
	
		if (provider == NULL)
			return 0;

		provider->SetKey(key);
		provider->SetIv(IV);

		if (encrypt)
		{
			provider->Encrypt2(lpInBuffer, nInSize, lpOutBuffer, nOutSize);
		}
		else
		{
			provider->Decrypt2(lpInBuffer, nInSize, lpOutBuffer, nOutSize);
		}
		
		if (provider != NULL)
			delete provider;
		return nOutSize;
	}
	catch (...)
	{
		return 0;
	}
}