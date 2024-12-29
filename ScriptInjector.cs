using System;
using System.Runtime.InteropServices;
using System.Text;

namespace FiveMExecutor
{
    public class ScriptInjector
    {
        [DllImport("kernel32.dll")]
        private static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint flAllocationType, uint flProtect);

        [DllImport("kernel32.dll")]
        private static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);

        private MemoryManager memoryManager;

        public ScriptInjector(MemoryManager manager)
        {
            memoryManager = manager;
        }

        public bool InjectScript(string script)
        {
            try
            {
                byte[] scriptBytes = Encoding.UTF8.GetBytes(script);
                IntPtr allocatedMemory = VirtualAllocEx(memoryManager.ProcessHandle, IntPtr.Zero, (uint)scriptBytes.Length, 0x1000, 0x40);
                
                if (allocatedMemory != IntPtr.Zero)
                {
                    if (memoryManager.WriteMemory(allocatedMemory, scriptBytes))
                    {
                        IntPtr threadHandle = CreateRemoteThread(memoryManager.ProcessHandle, IntPtr.Zero, 0, allocatedMemory, IntPtr.Zero, 0, IntPtr.Zero);
                        return threadHandle != IntPtr.Zero;
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
