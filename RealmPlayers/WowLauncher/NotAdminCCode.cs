/*
//#include "stdafx.h"
#include "windows.h"
#include <iostream>

#ifndef SECURITY_MANDATORY_HIGH_RID
	#define SECURITY_MANDATORY_UNTRUSTED_RID            (0x00000000L)
	#define SECURITY_MANDATORY_LOW_RID                  (0x00001000L)
	#define SECURITY_MANDATORY_MEDIUM_RID               (0x00002000L)
	#define SECURITY_MANDATORY_HIGH_RID                 (0x00003000L)
	#define SECURITY_MANDATORY_SYSTEM_RID               (0x00004000L)
	#define SECURITY_MANDATORY_PROTECTED_PROCESS_RID    (0x00005000L)
#endif
 
#ifndef TokenIntegrityLevel
	#define TokenIntegrityLevel ((TOKEN_INFORMATION_CLASS)25)
#endif
 
//#ifndef TOKEN_MANDATORY_LABEL
//	typedef struct  
//	{
//		SID_AND_ATTRIBUTES Label;
//	} TOKEN_MANDATORY_LABEL;
//#endif
 
typedef BOOL (WINAPI *defCreateProcessWithTokenW)
		(HANDLE,DWORD,LPCWSTR,LPWSTR,DWORD,LPVOID,LPCWSTR,LPSTARTUPINFOW,LPPROCESS_INFORMATION);
 

// Writes Integration Level of the process with the given ID into pu32_ProcessIL
// returns Win32 API error or 0 if succeeded
DWORD GetProcessIL(DWORD u32_PID, DWORD* pu32_ProcessIL)
{
	*pu32_ProcessIL = 0;
	
	HANDLE h_Process   = 0;
	HANDLE h_Token     = 0;
	DWORD  u32_Size    = 0;
	BYTE*  pu8_Count   = 0;
	DWORD* pu32_ProcIL = 0;
	TOKEN_MANDATORY_LABEL* pk_Label = 0;
 
	h_Process = OpenProcess(PROCESS_QUERY_INFORMATION, FALSE, u32_PID);
	if (!h_Process)
		goto _CleanUp;
 
	if (!OpenProcessToken(h_Process, TOKEN_QUERY, &h_Token))
		goto _CleanUp;
				
	if (!GetTokenInformation(h_Token, TokenIntegrityLevel, NULL, 0, &u32_Size) &&
		 GetLastError() != ERROR_INSUFFICIENT_BUFFER)
		goto _CleanUp;
						
	pk_Label = (TOKEN_MANDATORY_LABEL*) HeapAlloc(GetProcessHeap(), 0, u32_Size);
	if (!pk_Label)
		goto _CleanUp;
 
	if (!GetTokenInformation(h_Token, TokenIntegrityLevel, pk_Label, u32_Size, &u32_Size))
		goto _CleanUp;
 
	pu8_Count = GetSidSubAuthorityCount(pk_Label->Label.Sid);
	if (!pu8_Count)
		goto _CleanUp;
					
	pu32_ProcIL = GetSidSubAuthority(pk_Label->Label.Sid, *pu8_Count-1);
	if (!pu32_ProcIL)
		goto _CleanUp;
 
	*pu32_ProcessIL = *pu32_ProcIL;
	SetLastError(ERROR_SUCCESS);
 
	_CleanUp:
	DWORD u32_Error = GetLastError();
	if (pk_Label)  HeapFree(GetProcessHeap(), 0, pk_Label);
	if (h_Token)   CloseHandle(h_Token);
	if (h_Process) CloseHandle(h_Process);
	return u32_Error;
}
 
// Creates a new process u16_Path with the integration level of the Explorer process (MEDIUM IL)
// If you need this function in a service you must replace FindWindow() with another API to find Explorer process
// The parent process of the new process will be svchost.exe if this EXE was run "As Administrator"
// returns Win32 API error or 0 if succeeded
DWORD CreateProcessMediumIL(WCHAR* u16_Path, WCHAR* u16_CmdLine, WCHAR* _CurrentDirectory = NULL, bool _HideWindow = false)
{
	HANDLE h_Process = 0;
	HANDLE h_Token   = 0;
	HANDLE h_Token2  = 0;
	PROCESS_INFORMATION k_ProcInfo    = {0};
	STARTUPINFOW        k_StartupInfo = {0};
	if(_HideWindow == true)
	{
		k_StartupInfo.dwFlags = STARTF_USESHOWWINDOW;
	}
	BOOL b_UseToken = FALSE;
 
	// Detect Windows Vista, 2008, Windows 7 and higher
	if (GetProcAddress(GetModuleHandleA("Kernel32"), "GetProductInfo"))
	{
		DWORD u32_CurIL;
		DWORD u32_Err = GetProcessIL(GetCurrentProcessId(), &u32_CurIL);
		if (u32_Err)
			return u32_Err;
 
		if (u32_CurIL > SECURITY_MANDATORY_MEDIUM_RID)
			b_UseToken = TRUE;
	}
 
	// Create the process normally (before Windows Vista or if current process runs with a medium IL)
	if (!b_UseToken)
	{
		if (!CreateProcessW(u16_Path, u16_CmdLine, 0, 0, FALSE, 0, 0, _CurrentDirectory, &k_StartupInfo, &k_ProcInfo))
			return GetLastError();
 
		return ERROR_SUCCESS;
	}
 
	defCreateProcessWithTokenW f_CreateProcessWithTokenW = 
		(defCreateProcessWithTokenW) GetProcAddress(GetModuleHandleA("Advapi32"), "CreateProcessWithTokenW");
 
	if (!f_CreateProcessWithTokenW) // This will never happen on Vista!
		return ERROR_INVALID_FUNCTION; 
	
	HWND h_Progman = ::FindWindow("Progman", NULL);
	if (!h_Progman) // This can only happen if Explorer has crashed (User has no desktop and no taskbar)
		return ERROR_INVALID_WINDOW_HANDLE;
 
	DWORD u32_ExplorerPID = 0;		
	GetWindowThreadProcessId(h_Progman, &u32_ExplorerPID);
 
	// ATTENTION:
	// If UAC is turned OFF all processes run with SECURITY_MANDATORY_HIGH_RID, also Explorer!
	// But this does not matter because to start the new process without UAC no elevation is required.
	h_Process = OpenProcess(PROCESS_QUERY_INFORMATION, FALSE, u32_ExplorerPID);
	if (!h_Process)
		goto _CleanUp;
 
	if (!OpenProcessToken(h_Process, TOKEN_DUPLICATE, &h_Token))
		goto _CleanUp;
 
	if (!DuplicateTokenEx(h_Token, TOKEN_ALL_ACCESS, 0, SecurityImpersonation, TokenPrimary, &h_Token2))
		goto _CleanUp;
 
	if (!f_CreateProcessWithTokenW(h_Token2, 0, u16_Path, u16_CmdLine, 0, 0, _CurrentDirectory, &k_StartupInfo, &k_ProcInfo))
		goto _CleanUp;
 
	SetLastError(ERROR_SUCCESS);
 
	_CleanUp:
	DWORD u32_Error = GetLastError();
	if (h_Token)   CloseHandle(h_Token);
	if (h_Token2)  CloseHandle(h_Token2);
	if (h_Process) CloseHandle(h_Process);
	return u32_Error;
}
 
int main(int argc, char* argv[])
{
	DWORD u32_Err = 0;

	if(argc < 4)
	{
		printf("NotAdmin.exe exited with error: Not valid usage\r\n");
		Sleep(2000);
		return 0;
	}
	else
	{
		//"C:\\Windows\\System32\\Calc.exe"
		int argv1Size = strlen(argv[1]) + 1;
		std::wstring wc(argv1Size, L'#');
		mbstowcs(&wc[0], argv[1], argv1Size);
		
		int argv2Size = strlen(argv[2]) + 1;
		std::wstring wc2(argv2Size, L'#');
		mbstowcs(&wc2[0], argv[2], argv2Size);
		
		int argv3Size = strlen(argv[3]) + 1;
		std::wstring wc3(argv3Size, L'#');
		mbstowcs(&wc3[0], argv[3], argv3Size);

		u32_Err = CreateProcessMediumIL(&wc[0], &wc3[0], &wc2[0], argc >= 5 && strcmp(argv[4], "nowindow") == 0);
		if(u32_Err != 0)
			printf("CreateProcessMediumIL() exited with error %d\r\n", u32_Err);
		else
			printf("NotAdmin.exe successfully launched \"%s\" in working directory \"%s\" with cmdparams \"%s\"\r\n", argv[1], argv[2], argv[3]);
	}
 
	Sleep(2000);
	return 0;
}
*/