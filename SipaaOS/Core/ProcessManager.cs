using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SipaaOS.Core
{
    public class ProcessManager
    {
        public static List<Process> Processes { get; set; }

        public static void Init()
        {
            Processes = new List<Process>();
        }

        public static void Yield()
        {
            try
            {
                foreach (Process process in Processes)
                {
                    process.Update();
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.SKBugCheck(ex);
            }
        }

        public static bool StartProcess(Process process)
        {
            Processes.Add(process);

            var r = process.Start();

            return r;
        }

        public static bool StopProcess(Process process)
        {
            Processes.Remove(process);

            var r = process.Stop();

            return r;
        }

        public static Process GetProcessByName(string name)
        {
            foreach (Process process in Processes)
            {
                if (process.Name == name)
                {
                    return process;
                }
            }

            return null;
        }
    }
}
