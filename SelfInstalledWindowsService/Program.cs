using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration.Install;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace SelfInstalledWindowsService
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {

            string[] args = Environment.GetCommandLineArgs();

            WriteLogs("Argument Length: " + args.Length);


            if (args.Length == 1)
            {
                WriteLogs( "First Argument:  " + args[0]);
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                new SelfInstallService()
                };
                ServiceBase.Run(ServicesToRun);
            }
            else if (args.Length == 2)
            {
                WriteLogs("Second Argument:  " + args[1]);
                switch (args[1])
                {
                    case "-i":
                        InstallService();
                        StartService();
                        break;
                    case "-u":
                        StopService();
                        UninstallService();
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

        }


        private static bool IsInstalled()
        {
            using (ServiceController controller =
                new ServiceController("SelfInstalledService"))
            {
                try
                {
                    ServiceControllerStatus status = controller.Status;
                }
                catch
                {
                    return false;
                }
                return true;
            }
        }

        private static bool IsRunning()
        {
            using (ServiceController controller =
                new ServiceController("SelfInstalledService"))
            {
                if (!IsInstalled()) return false;
                return (controller.Status == ServiceControllerStatus.Running);
            }
        }

        private static AssemblyInstaller GetInstaller()
        {
            AssemblyInstaller installer = new AssemblyInstaller(
                typeof(SelfInstallService).Assembly, null);
            installer.UseNewContext = true;
            return installer;
        }

        private static void InstallService()
        {
            if (IsInstalled()) return;

            try
            {
                using (AssemblyInstaller installer = GetInstaller())
                {
                    IDictionary state = new Hashtable();
                    try
                    {
                        installer.Install(state);
                        installer.Commit(state);
                    }
                    catch
                    {
                        try
                        {
                            installer.Rollback(state);
                        }
                        catch { }
                        throw;
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        private static void UninstallService()
        {
            if (!IsInstalled()) return;
            try
            {
                using (AssemblyInstaller installer = GetInstaller())
                {
                    IDictionary state = new Hashtable();
                    try
                    {
                        installer.Uninstall(state);
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        private static void StartService()
        {
            if (!IsInstalled()) return;

            using (ServiceController controller =
                new ServiceController("SelfInstalledService"))
            {
                try
                {
                    if (controller.Status != ServiceControllerStatus.Running)
                    {
                        controller.Start();
                        controller.WaitForStatus(ServiceControllerStatus.Running,
                            TimeSpan.FromSeconds(10));
                    }
                }
                catch
                {
                    throw;
                }
            }
        }

        private static void StopService()
        {
            if (!IsInstalled()) return;
            using (ServiceController controller =
                new ServiceController("SelfInstalledService"))
            {
                try
                {
                    if (controller.Status != ServiceControllerStatus.Stopped)
                    {
                        controller.Stop();
                        controller.WaitForStatus(ServiceControllerStatus.Stopped,
                             TimeSpan.FromSeconds(10));
                    }
                }
                catch
                {
                    throw;
                }
            }
        }

        private static void WriteLogs(string logmessage)
        {
            string directoryPath = AppDomain.CurrentDomain.BaseDirectory;
            string fileName = "SelfInstalledServiceLogs.txt";
            string filePath = Path.Combine(directoryPath, fileName);

            using (StreamWriter sw = new StreamWriter(File.Open(filePath, System.IO.FileMode.Append)))
            {
                sw.WriteLineAsync(DateTime.Now + " : " + logmessage);
            }
        }
    }
}
