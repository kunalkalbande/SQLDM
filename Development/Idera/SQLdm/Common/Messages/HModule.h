#pragma once

#include <windows.h>
#include <vcclr.h>

namespace Idera { 
	namespace SQLdm { 
		namespace Common { 
			namespace Messages
			{
				using namespace System;
				using namespace System::Reflection;
				using namespace System::Runtime::InteropServices;

				public ref class HModule : SafeHandle
				{
				private:
					IntPtr^ HandleFromPath(String^ path) 
					{						
						pin_ptr<const wchar_t> pPath = PtrToStringChars(path);
						LPVOID addr = LoadLibraryEx(pPath, NULL, LOAD_LIBRARY_AS_DATAFILE);
						return gcnew IntPtr(addr);
					}
				public:
					HModule();
					
					HModule(IntPtr handle); 

					HModule(Assembly^ assembly);

					HModule(String^ path);

					virtual bool ReleaseHandle() override 
					{
						return FreeLibrary((HMODULE)handle.ToPointer()) ? true : false;
					}

					virtual property bool IsInvalid
					{
						bool get() override { return handle == IntPtr::Zero; }
					}
				};
			}
		}
	}
}