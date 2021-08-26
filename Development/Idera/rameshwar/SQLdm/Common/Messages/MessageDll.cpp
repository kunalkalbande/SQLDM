// This is the main DLL file.

#include "stdafx.h"
#include <sddl.h>
#include "MessageDll.h"


extern "C" WINADVAPI BOOL WINAPI ConvertStringSidToSidW(IN LPCWSTR StringSid, OUT PSID *Sid);

namespace Idera    { 
namespace SQLdm    { 
namespace Common   { 
namespace Messages {

	using namespace System::Reflection;
	using namespace System::ComponentModel;
	using namespace System::Threading;
	using namespace System::Security::Principal;

	MessageDll::MessageDll() 
	{
		module = gcnew HModule(Assembly::GetExecutingAssembly());
	}

	String^ MessageDll::Format(UINT messageId, ...array<String^>^ args)
	{
		String^ resultString;

		LPTSTR lpBuffer = NULL;

		LPCWSTR *argArray = NULL;
		int nArgs = 0;

		if (args != nullptr && args->Length > 0) 
		{
			nArgs = args->Length;
			argArray = new LPCWSTR[nArgs];
		}

		// get closer to the actual handle to the message dll
		IntPtr^ hModuleRef = module->DangerousGetHandle();
        try
        {
			// we always want to get from the module or the system and we want the function to alloc the buffer
			UINT flags = FORMAT_MESSAGE_FROM_SYSTEM | FORMAT_MESSAGE_FROM_HMODULE | FORMAT_MESSAGE_ALLOCATE_BUFFER;
			// if no substitutions then ignore inserts else we need to pass the values in an array
			if (nArgs == 0)
                flags |= FORMAT_MESSAGE_IGNORE_INSERTS;
            else
                flags |= FORMAT_MESSAGE_ARGUMENT_ARRAY;

			// marshall the args to an array of pointers
			for (int i = 0; i < nArgs; i++) 
			{
				pin_ptr<const wchar_t> sptr = PtrToStringChars(args[i]);
				// clr wont let you create an array of pinned pointers - fake it out
				argArray[i] = (LPCWSTR)CvtPinnedPtr(sptr);
			}

            UINT dwChars = FormatMessageW(
                flags,          
				hModuleRef->ToPointer(),        // message dll
                messageId,						// message resource id
                0,								// default language
                (LPTSTR)&lpBuffer,				// returned pointer to the formatted message
                128,							// minimum alloc size for dynamic buffer allocation
                (va_list*)argArray);			// optional substitution strings

            if (dwChars == 0)
            {
				int le = Marshal::GetLastWin32Error();
                throw gcnew Win32Exception(le);
            }
			IntPtr bufPtr = IntPtr(lpBuffer);

            // convert the returned message to a string
			resultString = Marshal::PtrToStringUni(bufPtr);
        }
        finally
        {
			// free the array of message subst values
			if (argArray != NULL) 
			{
				delete [] argArray;
			}
            // free the allocated message buffer
			if (lpBuffer != NULL) 
			{
				LocalFree(lpBuffer);
			}
        }

        return resultString;
	}

	void MessageDll::WriteEvent(String ^eventSource, UINT eventType, UINT categoryId, UINT eventId, ...array<String^>^ args) 
	{
		pin_ptr<const wchar_t> pEventSource = PtrToStringChars(eventSource);
		HANDLE hEventLog = RegisterEventSource(NULL, pEventSource);
		if (hEventLog == NULL)
			throw gcnew ArgumentException("Event Source is invalid", "eventSource");

		PSID pSID = NULL;
		try 
		{	// get a pointer to the sid for the current threads identity
			IIdentity ^identity = Thread::CurrentPrincipal->Identity;
			if (identity != nullptr && identity->GetType() == WindowsIdentity::typeid) 
			{
				SecurityIdentifier^ sid = safe_cast<WindowsIdentity^>(identity)->User;
				if (sid != nullptr) 
				{
					String^ sidString = sid->Value;
					pin_ptr<const wchar_t> pSidString = PtrToStringChars(sidString);
					ConvertStringSidToSidW(pSidString, &pSID);
				}
			}
		} 
		catch (Exception ^e) 
		{
			// no biggie - the event record just won't get written with a user	
		}


		LPCWSTR *argArray = NULL;
		int nArgs = 0;

		if (args != nullptr && args->Length > 0) 
		{
			nArgs = args->Length;
			argArray = new LPCWSTR[nArgs];
		}

        try
        {
			// marshall the args to an array of pointers
			for (int i = 0; i < nArgs; i++) 
			{
				pin_ptr<const wchar_t> sptr = PtrToStringChars(args[i]);
				// clr wont let you create an array of pinned pointers - fake it out
				argArray[i] = (LPCWSTR)CvtPinnedPtr(sptr);
			}

            BOOL bResult = ReportEventW(
				hEventLog,
                eventType,
				categoryId,
				eventId,
				pSID,
				nArgs,
				0,
				argArray,
				NULL);	

            if (!bResult)
            {
				int le = Marshal::GetLastWin32Error();
                throw gcnew Win32Exception(le);
            }
        }
        finally
        {
			if (pSID != NULL)
			{
				LocalFree(pSID);
			}

			// free the array of message subst values
			if (argArray != NULL) 
			{
				delete [] argArray;
			}

			DeregisterEventSource(hEventLog);
        }
					
	}


}
}
}
}