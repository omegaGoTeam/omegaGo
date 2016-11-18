#pragma once

namespace Oakfoam
{
	public ref class OakfoamWrapper sealed
	{
	public:		
		/// <summary>
		/// Initializes the Oakfoam Go engine and readies it. After calling this method, the user
		/// will be able to pass GTP commands to the engine.
		/// </summary>
		static void InitializeEngine() {

		}		
		/// <summary>
		/// Passes a Go Text Protocol command to the engine and blocks until the engine turns an answer.
		/// Only the answer will be returned, all debugging information printed before the answer will be
		/// discarded. 
		/// TODO we should also return debugging information
		/// </summary>
		/// <param name="command">The command.</param>
		/// <returns></returns>
		static Platform::String^ ExecuteGtpCommand(Platform::String^ command) {
			wchar_t msg[] = L"This is not yet supported.";
			return ref new Platform::String(msg);
		}
		OakfoamWrapper() {

		}
	};
}
