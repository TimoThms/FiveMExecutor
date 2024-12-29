using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace FiveMExecutor
{
    public class ProcessDetector
    {
        public event EventHandler<Process> FiveMProcessFound;

        public async Task StartDetection()
        {
            while (true)
            {
                Process[] processes = Process.GetProcessesByName("FiveM");
                if (processes.Length > 0)
                {
                    FiveMProcessFound?.Invoke(this, processes[0]);
                }
                await Task.Delay(1000);
            }
        }
    }
}
