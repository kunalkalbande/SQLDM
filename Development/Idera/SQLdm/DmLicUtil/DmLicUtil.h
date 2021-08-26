//**********************************************************************
//*
//* File: DmLicUtil.h
//*
//* Copyright Idera (Idera, Inc.) 2005
//*
//**********************************************************************
#pragma once

// Error codes.
#define INVALID_INPUT_ARGUMENT		1	//  bad function call arg
#define MEMORY_ALLOCATION_ERROR		2	//  memory allocation failed
#define ACQUIRE_PROVIDER_ERROR		3	//  CryptAcquireContext failed
#define ENCRYPT_ERROR				   4	//  CryptEncrypt failed
#define DECRYPT_ERROR				   5	//  CryptDecrypt failed
#define CREATE_HASH_ERROR			   6	//  CryptCreateHash failed
#define HASH_DATA_ERROR				   7	//  CryptHashData failed
#define DERIVE_KEY_ERROR			   8	//  CryptDeriveKey failed

extern "C" {
   //  string encryption/decryption functions
   //  these functions return the encrypted/decrypted string
   BYTE* __stdcall
      EncryptString(
         char        *szSource,		//  input plain text string
         char        *szPassword,	//  key word for encryption
	      DWORD	      *numBytes,		//  size of returned encrypted string in bytes
	      DWORD       *errcode			//  error return code
      );

   char* __stdcall
      DecryptString(
         BYTE        *szSource,		//  input encrypted string as bytes
         char        *szPassword,	//  key word for encryption
	      DWORD       srcLen,			//  length of source in bytes
	      DWORD       *errcode			//  error return code
      );

   void __stdcall
      FreeCryptBuffer(
         void        *buf           // Buffer to be deallocated.
      );

   // Set permissions (directories, registries, files, etc.)
   DWORD __stdcall
      SetObjectAccess (
			LPCTSTR  accountIn,        // account in domain\user format
         DWORD    objTypeIn,        // Object type 
                                    // 1 - file/dir (it doesn't distinguish between file and dir).
                                    // 4 - registry.
		   LPCTSTR  pathIn,           // Object path.
         DWORD    accessMaskIn      // Access rights.
      );

}