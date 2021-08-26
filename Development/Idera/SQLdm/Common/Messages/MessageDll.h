// Idera.SQLdm.Common.Messages.h

#pragma once

#include <vcclr.h>
#include "HModule.h"

#define UINT unsigned int

using namespace System;
using namespace System::Security;
using namespace System::Runtime::InteropServices;
using namespace Idera::SQLdm::Common::Messages;

namespace Idera { 
	namespace SQLdm { 
		namespace Common { 
			namespace Messages
			{
				#include "messages.h"
				#include "messages.inc"

				// you can't cast a pinned pointer to another type or create an pointer to
				// a pinned pointer.  You can pass a pinned pointer to a function where we
				// just return return it to get it as a (void *).
				inline LPCVOID CvtPinnedPtr(const wchar_t *ptr) 
				{
					return ptr;
				}

				public ref class MessageDll
				{
				private:
					// SafePointer to the message dll
					HModule^ module;
				public:
					MessageDll();	
					
					String^ Format(UINT messageId, ...array<String^>^ args);
					String^ Format(Status messageId, ...array<String^>^ args) 
					{
						return Format((UINT)messageId, args);
					}

					static void WriteEvent(String ^eventSource, UINT eventType, UINT categoryId, UINT eventId, ...array<String^>^ args); 


				};
			}
		}
	}
}

