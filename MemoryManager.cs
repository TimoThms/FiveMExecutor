using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace FiveMExecutor
{
    public class MemoryManager
    {
        [DllImport("kernel32.dll")]
        private static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll")]
        private static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int dwSize, out IntPtr lpNumberOfBytesRead);

        [DllImport("kernel32.dll")]
        private static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int dwSize, out IntPtr lpNumberOfBytesWritten);

        private IntPtr processHandle;
        private const int PROCESS_ALL_ACCESS = 0x1F0FFF;

        public bool AttachToProcess(string processName)
        {
            Process[] processes = Process.GetProcessesByName(processName);
            if (processes.Length > 0)
            {
                processHandle = OpenProcess(PROCESS_ALL_ACCESS, false, processes[0].Id);
                return processHandle != IntPtr.Zero;
            }
            return false;
        }

        public byte[] ReadMemory(IntPtr address, int size)
        {
            byte[] buffer = new byte[size];
            IntPtr bytesRead;
            ReadProcessMemory(processHandle, address, buffer, size, out bytesRead);
            return buffer;
        }

        public bool WriteMemory(IntPtr address, byte[] data)
        {
            IntPtr bytesWritten;
            return WriteProcessMemory(processHandle, address, data, data.Length, out bytesWritten);
        }
    }
}
