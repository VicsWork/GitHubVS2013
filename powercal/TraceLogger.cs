using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace PowerCalibration
{
    class TraceLogger
    {
        /// <summary>
        /// Writes to the trace
        /// </summary>
        /// <param name="txt"></param>
        public static string Log(string txt)
        {
            string line = string.Format("{0:G}: {1}", DateTime.Now, txt);
            Trace.WriteLine(line);
            Trace.Flush();

            return line;
        }


    }
}
