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


            Application.Run(new Form_Main());
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {

            Form_Exception dlg = new Form_Exception(title:"Unhadled UI Exception",
                message: (e.ExceptionObject as Exception).Message, detail: e.ExceptionObject.ToString());
            dlg.ShowDialog();

        }

        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            Form_Exception dlg = new Form_Exception(title:"Unhadled Thread Exception",
                message: e.Exception.Message, detail: e.Exception.StackTrace.ToString());
            dlg.ShowDialog();

        }
    }
}
