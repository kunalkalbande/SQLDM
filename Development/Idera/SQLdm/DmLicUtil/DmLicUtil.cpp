//**********************************************************************
//*
//* File: DmLicUtil.cpp
//*
//* Copyright Idera (Idera, Inc.) 2005
//*
//**********************************************************************
#include "stdafx.h"
#include "DmLicUtil.h"

//--------------------------------------------------------------------
//  defines for the encryption functions
//  Here so they can be easily changed
//--------------------------------------------------------------------
#define ENCRYPT_ALGORITHM	CALG_RC2	// Block cipher
#define ENCRYPT_BLOCK_SIZE	8			// algorithm's block size
#define ENCRYPT_BLOCK_LEN	512			// encryption block length			
#define PROVIDER_NAME 		MS_DEF_DSS_DH_PROV
#define PROVIDER_TYPE		PROV_DSS_DH
#define KEY_LENGTH			40
#define HASH_ALGORITHM		CALG_MD5

//========================================================================================
// Static helper functions.
//========================================================================================
//========================================================================================
//	_GetKey() - generate an encryption session key from a 
//		keyword string.
//
//  Parameters:
//		hCryptProv - handle to the crypto provider.
//		strKeyword - the string used to generate the encryption session
//			key.
//		errcode - pointer to an int to contain the return code.
//			0 - success
//			>0 - an error occurred.  See error codes in crypt.h.
//
//	Returns:
//		handle to the encryption session key generated from the keyword.
//
//  NOTES:  
//		hashes using MD5
//========================================================================================
static HCRYPTKEY 
   _GetKey(
	   HCRYPTPROV  hCryptProv,
	   char        *strKeyword,
	   DWORD       *errcode
   )
{
	HCRYPTKEY hKey = 0; 
	HCRYPTHASH hHash = 0; 
 
	//--------------------------------------------------------------------
	//  validate input
	if ( hCryptProv == 0 || 
		 strKeyword == NULL || *strKeyword == '\0' || 
		 errcode == NULL )
	{
		if ( errcode )
			*errcode = INVALID_INPUT_ARGUMENT;
		return hKey;
	}

	*errcode = 0;

	//--------------------------------------------------------------------
	// The session key is derived from a keyword.
	// The session key will be recreated only if the keyword used to 
	// create the key is available. 

	//--------------------------------------------------------------------
	// Create a hash object. 
   if( ! ::CryptCreateHash(
			hCryptProv, 
			HASH_ALGORITHM, 
			0, 
			0,
			&hHash) )
	{ 
		*errcode = CREATE_HASH_ERROR;
		return hKey;
	}  

	//--------------------------------------------------------------------
	// Hash the keyword. 
   if( ! ::CryptHashData(
			hHash, 
			(BYTE *)strKeyword, 
         (DWORD)::strlen(strKeyword), 
			0) )
	{
		*errcode = HASH_DATA_ERROR;
		goto cleanup;
	}

	//--------------------------------------------------------------------
	// Derive a session key from the hash object. 
   if( ! ::CryptDeriveKey(
			hCryptProv, 
			ENCRYPT_ALGORITHM, 
			hHash, 
			MAKELONG(0,KEY_LENGTH),			//   40 bit key length
			&hKey) )
	{
		*errcode = DERIVE_KEY_ERROR;
		goto cleanup;
	}

cleanup:

	//--------------------------------------------------------------------
	// Destroy the hash object and return the session key
   ::CryptDestroyHash(hHash); 
	return hKey;
}

//========================================================================================
// DllMain
//========================================================================================
BOOL APIENTRY DllMain( HANDLE hModule, 
                       DWORD  ul_reason_for_call, 
                       LPVOID lpReserved
					 )
{
    return TRUE;
}

//========================================================================================
//	EncryptString() - encrypt a string using a 
//		password-generated key, and return the encrypted version
//
//  Parameters:
//		strSource - the name of the input, an ascii plaintext string.
//		strPassword - the password string.
//		errcode - pointer to an int to hold any error code.  Contains
//			0 on successful completion, >0 if an error occurred.  See
//			error codes in crypt.h.
//
//	Returns:
//		strDestination - encrypted string.  The caller must free
//		this when finished with it.
//
//  NOTES:  
//		hashes using MD5
//		encrypts using cylink_mek (40 bit)
//========================================================================================
BYTE* __stdcall
   EncryptString(
	   char  *strSource, 
	   char  *strPassword,
	   DWORD *numBytes,		//  num bytes of encrypted data
	   DWORD *errcode) 
{ 
	//--------------------------------------------------------------------
	//   Declare and initialize local variables.

	HCRYPTPROV hCryptProv = 0; 
	HCRYPTKEY hKey = 0; 

	BYTE*	pbBuffer = NULL; 
	DWORD	dwBlockLen; 
	DWORD	dwBufferLen; 
	DWORD	dwCount; 

	BYTE*	srcBytes = (BYTE*)strSource;
	BYTE*	strDest = NULL;
	DWORD	strDestLen;
 
	BOOL	isFinal = FALSE;

	DWORD	strSourceLen;
	DWORD	strDestPos,strSourcePos;
	DWORD   encryptBlockLen;

	//--------------------------------------------------------------------
	// validate input. 

	if( ! strSource || *strSource == '\0' || 
		! strPassword || *strPassword == '\0' ||
		! errcode )
	{
		if ( errcode )
			*errcode = INVALID_INPUT_ARGUMENT;
		return NULL;
	} 

	*errcode = 0;

	//--------------------------------------------------------------------
	// Get handle to the default provider. 
   if( ::CryptAcquireContext(
		  &hCryptProv, 
		  NULL, 
		  PROVIDER_NAME, 
		  PROVIDER_TYPE,	 
		  CRYPT_VERIFYCONTEXT) == FALSE )
	{
		*errcode = ACQUIRE_PROVIDER_ERROR;
		return NULL;
	}

	hKey = _GetKey(hCryptProv,strPassword,errcode);
	if ( hKey == 0 )
	{
		goto cleanup;
	}

	//--------------------------------------------------------------------
	//  The session key is now ready. 

    
	//--------------------------------------------------------------------
	// Determine number of bytes to encrypt at a time. 
	// This must be a multiple of ENCRYPT_BLOCK_SIZE.
	// ENCRYPT_BLOCK_SIZE is set by a #define statement.

   encryptBlockLen = (DWORD)(::strlen(strSource) * sizeof(char));
	dwBlockLen = encryptBlockLen - encryptBlockLen % ENCRYPT_BLOCK_SIZE;
	//--------------------------------------------------------------------
	// Determine the block size. If a block cipher is used, 
	// it must have room for an extra block. 

	if(ENCRYPT_BLOCK_SIZE > 1) 
		dwBufferLen = dwBlockLen + ENCRYPT_BLOCK_SIZE; 
	else 
		dwBufferLen = dwBlockLen; 

	//--------------------------------------------------------------------
	// alloc mem for destination string (in bytes)

   strDestLen = strSourceLen = (DWORD)(::strlen(strSource) * sizeof(char));
	if ( strDestLen < dwBufferLen )
		strDestLen = dwBufferLen;
	else
		strDestLen = (strDestLen % dwBufferLen) * dwBufferLen + dwBufferLen;

	if( (strDest = new BYTE[strDestLen]) == NULL )
	{ 
		*errcode = MEMORY_ALLOCATION_ERROR;
		goto cleanup;
	}

	//--------------------------------------------------------------------
	// Allocate memory for the encryption buffer
	if( (pbBuffer = new BYTE[dwBufferLen]) == NULL )
	{ 
		*errcode = MEMORY_ALLOCATION_ERROR;
		goto cleanup;
	}

	//--------------------------------------------------------------------
	// In a do loop, encrypt the source string and write to the destination
	// string. 

	strSourcePos = strDestPos = 0;
	
	do 
	{ 
		if ( strSourcePos + dwBlockLen < strSourceLen )
			dwCount = dwBlockLen;
		else
		{
			dwCount = strSourceLen - strSourcePos;
			isFinal = TRUE;
		}

		//--------------------------------------------------------------------
		//	Copy up to dwCount bytes from the source string into the
		//	encryption buffer.
		memcpy((void*)pbBuffer,(const void*)&srcBytes[strSourcePos],dwCount);
 
		//--------------------------------------------------------------------
		// encrypt data. 
      if( ! ::CryptEncrypt(
				hKey,				//  hKey
				0,					//  hHash
				isFinal,			//  final
				0,					//  dwFlags
				pbBuffer,			//  pbData
				&dwCount,			//   # of bytes in pbBuffer to encrypt
				dwBufferLen))		//  length in bytes of pbBuffer
		{ 
			*errcode = ENCRYPT_ERROR;
			goto cleanup;
		} 

		//--------------------------------------------------------------------
		// copy encrypted data to the destination string. 

		memcpy((void*)&strDest[strDestPos],(const void*)pbBuffer,dwCount);
		strDestPos += dwCount;
		strSourcePos += dwCount;
	} while(! isFinal ); 

	//--------------------------------------------------------------------
	//  End the do loop when the last block of the source file has been
	//  read, encrypted, and written to the destination file.

	//--------------------------------------------------------------------

cleanup:

	// Free memory. 

	if( pbBuffer ) 
		delete [] pbBuffer; 
 
	//--------------------------------------------------------------------
	// Destroy session key. 

	if( hKey ) 
      ::CryptDestroyKey(hKey); 
  
	//--------------------------------------------------------------------
	// Release provider handle. 

	if( hCryptProv ) 
      ::CryptReleaseContext(hCryptProv, 0);

	*numBytes = strDestPos;
	return	strDest; 
} // End of encrypt

//========================================================================================
//	DecryptString() - decrypt an encrypted string using a 
//		password-generated key, and return the plain-text string.
//
//  Parameters:
//		strSource - the bytes for the encrypted string.
//		strPassword - the password string.
//		errcode - pointer to an int to contain error code.  0 if no error,
//			>0, an error occurred.  See error codes in crypt.h.
//	
//	Returns:
//		a string containing the decrypted string.  The caller must free
//		the string.
//
//  NOTES:  
//		hashes using MD5
//		encrypts using cylink_mek (40 bit)
//========================================================================================
char* __stdcall
   DecryptString(
	   BYTE     *strSource, 
	   char     *strPassword,
	   DWORD    srcLen,			//  number of bytes in strSource
	   DWORD    *errcode
   ) 
{ 
	//--------------------------------------------------------------------
	//   Declare and initialize local variables.

	HCRYPTPROV hCryptProv = 0; 
	HCRYPTKEY hKey = 0; 
 
	BYTE* pbBuffer = NULL; 
	DWORD dwBlockLen; 
	DWORD dwBufferLen; 
	DWORD dwCount; 

	BYTE* strDest = NULL;
	DWORD strDestLen;
 
	BOOL  isFinal = FALSE;

	DWORD strSourceLen;
	DWORD strDestPos,strSourcePos;
	DWORD   encryptBlockLen;

	//--------------------------------------------------------------------
	// validate input
	if ( ! strSource || *strSource == '\0' ||
		 ! strPassword || *strPassword == '\0' ||
		 ! errcode )
	{
		if ( errcode )
			*errcode = INVALID_INPUT_ARGUMENT;
		return NULL;
	}

	*errcode = 0;

	//--------------------------------------------------------------------
	// Get a handle to the default provider. 
   if( ! ::CryptAcquireContext(
			&hCryptProv, 
			NULL, 
			PROVIDER_NAME, 
			PROVIDER_TYPE,	
			CRYPT_VERIFYCONTEXT) )
	{
	   *errcode = ACQUIRE_PROVIDER_ERROR;
	   return NULL;
	}

	//--------------------------------------------------------------------
	// Decrypt the file with a session key derived from a password. 

	hKey = _GetKey(hCryptProv,strPassword,errcode);
	if ( hKey == 0 )
	{ 
		goto cleanup;
	}

	//--------------------------------------------------------------------
	//   The decryption key is now available, having been created 
	//   using the password. 
 
	//--------------------------------------------------------------------
	// Determine the number of bytes to decrypt at a time. 
	// This must be a multiple of ENCRYPT_BLOCK_SIZE. 

	encryptBlockLen = srcLen;
	dwBlockLen = encryptBlockLen - encryptBlockLen % ENCRYPT_BLOCK_SIZE;
	if(ENCRYPT_BLOCK_SIZE > 1) 
		dwBufferLen = dwBlockLen + ENCRYPT_BLOCK_SIZE; 
	else 
		dwBufferLen = dwBlockLen; 

	//--------------------------------------------------------------------
	// alloc mem for destination string. 

	strDestLen = strSourceLen = srcLen;
	if ( strDestLen < dwBufferLen )
		strDestLen = dwBufferLen;
	else
		strDestLen = (strDestLen % dwBufferLen) * dwBufferLen + dwBufferLen;
   strDestLen += 1; // add 1 for terminating null.

	if( (strDest = new BYTE[strDestLen]) == NULL )
	{ 
		*errcode = MEMORY_ALLOCATION_ERROR;
		goto cleanup;
	}


	//--------------------------------------------------------------------
	// Allocate memory for the decryption buffer. 

	if( (pbBuffer = new BYTE[dwBufferLen]) == NULL )
	{ 
		*errcode = MEMORY_ALLOCATION_ERROR;
		goto cleanup;
	}

	//--------------------------------------------------------------------
	// In a do loop, decrypt the source string and write to the destination
	// string. 

	strSourcePos = strDestPos = 0;
	
	do 
	{ 
		if ( strSourcePos + dwBlockLen < strSourceLen )
			dwCount = dwBlockLen;
		else
		{
			dwCount = strSourceLen - strSourcePos;
			isFinal = TRUE;
		}

		//--------------------------------------------------------------------
		//	Copy up to dwCount bytes from the source string into the
		//	encryption buffer.
		memcpy((void*)pbBuffer,(const void*)&strSource[strSourcePos],dwCount);
 
		//--------------------------------------------------------------------
		// Decrypt data. 
      if(! ::CryptDecrypt(
				hKey,			//  key
				0,				//  hash
				isFinal,		//  final
				0,				//	flags
				pbBuffer,		//  data
				&dwCount) )		//  data length
		{ 
			*errcode = DECRYPT_ERROR;
			goto cleanup;
		} 

		//--------------------------------------------------------------------
		// copy encrypted data to the destination string. 

		memcpy((void*)&strDest[strDestPos],(const void*)pbBuffer,dwCount);
		strDestPos += dwCount;
		strSourcePos += dwCount;
	} while( !isFinal ); 
   strDest[strDestPos] = 0;

cleanup:

	//--------------------------------------------------------------------
	// Free memory. 
	if( pbBuffer ) 
		 delete [] pbBuffer; 
 
	//--------------------------------------------------------------------
	// Destroy session key. 
	if( hKey ) 
      ::CryptDestroyKey(hKey); 

	//--------------------------------------------------------------------
	// Release provider handle. 
	if( hCryptProv ) 
      ::CryptReleaseContext(hCryptProv, 0); 

	return (char*)strDest;
}

//========================================================================================
// FreeCryptBuffer
//========================================================================================
void __stdcall
   FreeCryptBuffer(
      void        *buf           // Buffer to be deallocated.
   )
{
   if(buf) { delete [] buf; }
}

//========================================================================================
// SetObjectAccess
//========================================================================================
DWORD __stdcall
   SetObjectAccess (
		LPCTSTR  accountIn,        // account in domain\user format
      DWORD    objTypeIn,        // Object type 
                                 // 1 - file/dir (it doesn't distinguish between file and dir).
                                 // 4 - registry.
		LPCTSTR  pathIn,           // Object path.
      DWORD    accessMaskIn      // Access rights.
   )
{
   DWORD rc = ERROR_SUCCESS;

   //----------------------------------------------------------------------------------------
   // Validate input.
   if(pathIn == 0 || ::_tcslen(pathIn) == 0           // non-empty path.
      || accountIn == 0 || ::_tcslen(accountIn) == 0) // non-empty account.
   {
      rc = ERROR_INVALID_PARAMETER;
   }

   //----------------------------------------------------------------------------------------
   // Lookup the account name to get its SID.   Also verify if the account is a user.
   BYTE           sid[100];    
   DWORD          dwSid = sizeof(sid);
   TCHAR          domain[32];  
   DWORD          dwDomain = DIM(domain);
   SID_NAME_USE   sidNameUse;
   if(rc == ERROR_SUCCESS)
   {
      if (::LookupAccountName 
            (NULL, accountIn, sid, &dwSid,
               domain, &dwDomain, &sidNameUse))   
      {
         if(sidNameUse != SidTypeUser)
         {
            rc = ERROR_NO_SUCH_USER; 
         }
      }
      else
      {
         rc = ::GetLastError();
      }
   }

   //----------------------------------------------------------------------------------------
   // Setup ACL.
   EXPLICIT_ACCESS      ea;
   PACL                 pACL = 0;
   if(rc == ERROR_SUCCESS)
   {
      // Create explicit access for the input SID to have full control.
      ::memset((void*)&ea,0,sizeof(ea));
      ea.grfAccessMode        = SET_ACCESS;
      ea.grfAccessPermissions = accessMaskIn;
      ea.grfInheritance       = SUB_CONTAINERS_AND_OBJECTS_INHERIT;
      ea.Trustee.TrusteeForm  = TRUSTEE_IS_SID;
      ea.Trustee.TrusteeType  = TRUSTEE_IS_UNKNOWN;
      ea.Trustee.ptstrName    = (LPTSTR)sid;

      // Create ACL.
      rc = ::SetEntriesInAcl(1,&ea,0,&pACL);
   }

   //----------------------------------------------------------------------------------------
   // Set the DACL on the object and prevent inheritance.
   if(rc == ERROR_SUCCESS)
   {
      rc = ::SetNamedSecurityInfo((LPSTR)pathIn,(SE_OBJECT_TYPE)objTypeIn,
               DACL_SECURITY_INFORMATION,0,0,pACL,0);
   }

   // Free resources.
   if(pACL) { ::LocalFree(pACL); }

   return rc;
}

