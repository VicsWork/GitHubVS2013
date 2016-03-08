using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PowerCalibration
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

            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            Form_Main form_main = new Form_Main();
            if (!form_main.IsDisposed)
            {
                Application.Run(form_main);
            }
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {

            Form_Exception dlg = new Form_Exception(title:"Unhandled UI Exception",
                message: (e.ExceptionObject as Exception).Message, detail: e.ExceptionObject.ToString());
            dlg.ShowDialog();

        }

        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            Form_Exception dlg = new Form_Exception(title:"Unhandled Thread Exception",
                message: e.Exception.Message, detail: e.Exception.StackTrace.ToString());
            dlg.ShowDialog();

        }
    }
}
