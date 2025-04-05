using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.ComponentModel;

namespace StarZInjector.Classes
{
    public static class LoadLibraryAInjector
    {
        private static readonly SecurityIdentifier AppPackagesSid = new("S-1-15-2-1");
        private static readonly string logFileName = "Injector.txt";

        public static bool Inject(string path, string name)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
                    throw new ArgumentException("Invalid DLL path.");

                string fullPath = Path.GetFullPath(path); // Convert to full path for safety
                LogsManager.Log("Injection process started.", logFileName);
                ApplyAppPackages(fullPath);

                Process process = ProcessManager.GetTargetProcessFromExeName(name) ?? throw new Exception("Target process not found.");
                IntPtr processHandle = NativeMethods.OpenProcess(
                    NativeMethods.PROCESS_ALL_ACCESS |
                    NativeMethods.PROCESS_CREATE_THREAD |
                    NativeMethods.PROCESS_QUERY_INFORMATION |
                    NativeMethods.PROCESS_VM_OPERATION |
                    NativeMethods.PROCESS_VM_WRITE |
                    NativeMethods.PROCESS_VM_READ,
                    false,
                    process.Id);

                if (processHandle == IntPtr.Zero)
                    throw new Win32Exception(Marshal.GetLastWin32Error(), "Failed to open process handle.");

                IntPtr allocMemAddress = IntPtr.Zero;
                IntPtr threadHandle = IntPtr.Zero;

                try
                {
                    IntPtr loadLibraryAddress = NativeMethods.GetProcAddress(
                        NativeMethods.GetModuleHandle("kernel32.dll"), "LoadLibraryA");

                    if (loadLibraryAddress == IntPtr.Zero)
                        throw new Win32Exception(Marshal.GetLastWin32Error(), "Failed to get LoadLibraryA address.");

                    byte[] buffer = Encoding.ASCII.GetBytes(fullPath + "\0");
                    uint size = (uint)buffer.Length;

                    allocMemAddress = NativeMethods.VirtualAllocEx(
                        processHandle,
                        IntPtr.Zero,
                        size,
                        NativeMethods.MEM_COMMIT | NativeMethods.MEM_RESERVE,
                        NativeMethods.PAGE_READWRITE);

                    if (allocMemAddress == IntPtr.Zero)
                        throw new Win32Exception(Marshal.GetLastWin32Error(), "Failed to allocate memory in target process.");

                    if (!NativeMethods.WriteProcessMemory(processHandle, allocMemAddress, buffer, size, out _))
                        throw new Win32Exception(Marshal.GetLastWin32Error(), "Failed to write to allocated memory.");

                    threadHandle = NativeMethods.CreateRemoteThread(
                        processHandle,
                        IntPtr.Zero,
                        0,
                        loadLibraryAddress,
                        allocMemAddress,
                        0,
                        IntPtr.Zero);

                    if (threadHandle == IntPtr.Zero)
                        throw new Win32Exception(Marshal.GetLastWin32Error(), "Failed to create remote thread.");

                    uint waitResult = NativeMethods.WaitForSingleObject(threadHandle, NativeMethods.INFINITE);
                    if (waitResult != NativeMethods.WAIT_OBJECT_0)
                        throw new Exception($"WaitForSingleObject failed or timed out. Result: {waitResult}");

                    if (!NativeMethods.GetExitCodeThread(threadHandle, out uint remoteModuleHandle))
                        throw new Win32Exception(Marshal.GetLastWin32Error(), "Failed to get thread exit code.");

                    if (remoteModuleHandle == 0)
                        throw new Exception("LoadLibraryA failed in remote process (exit code = 0).");

                    LogsManager.Log("Injection completed successfully.", logFileName);
                    return true;
                }
                finally
                {
                    if (threadHandle != IntPtr.Zero)
                        NativeMethods.CloseHandle(threadHandle);

                    if (allocMemAddress != IntPtr.Zero)
                        NativeMethods.VirtualFreeEx(processHandle, allocMemAddress, 0, NativeMethods.MEM_RELEASE);

                    if (processHandle != IntPtr.Zero)
                        NativeMethods.CloseHandle(processHandle);
                }
            }
            catch (Exception ex)
            {
                LogsManager.Log($"ERROR: {ex.Message}", logFileName);
                return false;
            }
        }

        private static void ApplyAppPackages(string path)
        {
            try
            {
                FileSecurity fileSecurity = new FileInfo(path).GetAccessControl();
                fileSecurity.AddAccessRule(new FileSystemAccessRule(AppPackagesSid, FileSystemRights.FullControl, AccessControlType.Allow));
                new FileInfo(path).SetAccessControl(fileSecurity);
                LogsManager.Log("AppPackages permissions applied.", logFileName);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to apply AppPackages permissions: {ex.Message}");
            }
        }
    }
}