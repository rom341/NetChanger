using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetChanger.Data.Network.ConnectionMethods
{
    public abstract class IConnectionMethod
    {
        public abstract bool TryConnect(string NetworkName);

        protected bool ExecuteProcess(string fileName, string arguments)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = fileName,
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    Verb = "runas"
                };

                Process process = Process.Start(psi);
                if (process != null)
                {
                    process.WaitForExit();
                    return process.ExitCode == 0;
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при выполнении процесса: {ex.Message}");
                return false;
            }
        }
    }
}
