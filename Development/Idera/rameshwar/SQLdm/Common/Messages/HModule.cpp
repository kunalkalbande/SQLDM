#include "stdafx.h"
#include "hmodule.h"


namespace Idera { 
	namespace SQLdm { 
		namespace Common { 
			namespace Messages {

				HModule::HModule() : SafeHandle(IntPtr::Zero, true) { }
		
				HModule::HModule(IntPtr handle) : SafeHandle(IntPtr::Zero, true) 
				{ 
					SetHandle(handle);
				}

				HModule::HModule(Assembly^ assembly) : SafeHandle(IntPtr::Zero, true)
				{
					SetHandle(*(HandleFromPath(assembly->Location)));
				}

				HModule::HModule(String^ path) : SafeHandle(IntPtr::Zero, true)
				{
					SetHandle(*(HandleFromPath(path)));
				}	

			}
		}
	}
}