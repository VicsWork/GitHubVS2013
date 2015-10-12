using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Threading;
using System.Diagnostics;

using AutoIt;

namespace powercal
{
    class Coder
    {
        int _cred = 0xFF0000;
        int _cgreen = 0x008000;
        Point _point_State_color;
        TimeSpan _timeout;

        public Coder(Point point_State_color, TimeSpan timeout)
        {
            _point_State_color = point_State_color;
            if(_point_State_color.X == 0 && _point_State_color.Y == 0)
            {
                string msg = string.Format("Point to check State color not set");
                throw new Exception(msg);
            }
            _timeout = timeout;
        }

        IntPtr getWin(string win_desc)
        {
            string msg;

            if (AutoItX.WinExists(win_desc) != 1)
            {
                msg = string.Format("Unable to find window \"{0}\"", win_desc);
                throw new Exception(msg);
            }

            IntPtr hwnd = AutoItX.WinGetHandle(win_desc);
            if (AutoItX.WinExists(win_desc) != 1)
            {
                msg = string.Format("Unable to get handle for window \"{0}\"", win_desc);
                throw new Exception(msg);
            }

            return hwnd;
        }

        IntPtr activate_win(string win_desc)
        {
            IntPtr hwnd = getWin(win_desc);

            string msg;
            int n = 0;
            while (AutoItX.WinActive(hwnd) != 1)
            {
                AutoItX.WinActivate(hwnd);
                Thread.Sleep(250);
                if (n++ > 10)
                {
                    msg = string.Format("Unable to activate window \"{0}\"", win_desc);
                    throw new Exception(msg);
                }
            }

            return hwnd;
        }

        public void Code(CancellationToken cancel)
        {
            if (cancel.IsCancellationRequested)
                return;

            string win_desc = "[REGEXPTITLE:Ember Bootloader and Range Test .*]";
            IntPtr hwnd = activate_win(win_desc);
            AutoItX.MouseMove(_point_State_color.X, _point_State_color.Y, 0);
            AutoItX.Send("{SPACE}");
            Thread.Sleep(500);
            Stopwatch watch = new Stopwatch();
            watch.Start();
            int pixel_color = 0;
            string msg;
            while (true)
            {
                if (cancel.IsCancellationRequested)
                    break;

                pixel_color = AutoItX.PixelGetColor(_point_State_color.X, _point_State_color.Y);

                if (pixel_color == _cred)
                {
                    AutoItX.MouseMove(_point_State_color.X, _point_State_color.Y, 0);
                    msg = string.Format("Red pixel at location X={0}, Y={1} after {2:F2} s",
                        _point_State_color.X, _point_State_color.Y, watch.Elapsed.TotalSeconds);
                    throw new Exception(msg);
                }
                else if (pixel_color == _cgreen)
                {
                    break;
                }


                Thread.Sleep(500);
                if (watch.Elapsed > _timeout)
                {
                    AutoItX.MouseMove(_point_State_color.X, _point_State_color.Y, 0);
                    msg = string.Format("Timeout after {0:hh\\:mm\\:ss}. Unable to detect green or read pixel at location X={1}, Y={2}",
                        watch.Elapsed, _point_State_color.X, _point_State_color.Y);
                    throw new Exception(msg);
                }

                if (AutoItX.WinActive(hwnd) == 0)
                {
                    AutoItX.WinActivate(hwnd);
                    AutoItX.MouseMove(_point_State_color.X, _point_State_color.Y, 0);
                }
            }
        }
    }
}
