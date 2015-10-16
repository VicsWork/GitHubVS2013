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

        string _win_desc = "[REGEXPTITLE:Ember Bootloader and Range Test .*]";

        public Coder(Point point_State_color, TimeSpan timeout)
        {
            _point_State_color = point_State_color;
            if(_point_State_color.X == 0 && _point_State_color.Y == 0)
            {
                _point_State_color = guessStatusPos();
                //string msg = string.Format("Point to check State color not set");
                //throw new Exception(msg);
            }
            _timeout = timeout;
        }

        public void Code(CancellationToken cancel)
        {
            if (cancel.IsCancellationRequested)
                return;

            IntPtr hwnd = activateMainWnd();
            moveToStatus();
            
            //AutoItX.Send("{SPACE}");
            click_Start();
            moveToStatus();
            Thread.Sleep(2000);

            int pixel_color = 0;
            string msg;
            int fail_count = 0;
            Stopwatch watch = new Stopwatch();
            watch.Start();
            while (true)
            {
                if (cancel.IsCancellationRequested)
                    break;

                pixel_color = getStatusColor();

                if (pixel_color == _cred)
                {

                    fail_count++;

                    if (fail_count > 3)
                    {
                        click_Start();
                        moveToStatus();
                        Thread.Sleep(2000);

                        msg = string.Format("Red pixel at location X={0}, Y={1} after {2:F2} s",
                            _point_State_color.X, _point_State_color.Y, watch.Elapsed.TotalSeconds);
                        throw new Exception(msg);
                    }
                    else
                    {
                        click_Retry();
                        moveToStatus();
                        Thread.Sleep(2000);
                    }
                }
                else if (pixel_color == _cgreen)
                {
                    break;
                }


                Thread.Sleep(1000);
                if (watch.Elapsed > _timeout)
                {
                    click_Start(); ;
                    moveToStatus();

                    msg = string.Format("Timeout after {0:hh\\:mm\\:ss}. Unable to detect green or read pixel at location X={1}, Y={2}",
                        watch.Elapsed, _point_State_color.X, _point_State_color.Y);
                    throw new Exception(msg);
                }

                if (AutoItX.WinActive(hwnd) == 0)
                {
                    hwnd = activateMainWnd();
                    moveToStatus();
                }
            }
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
            if (hwnd == IntPtr.Zero)
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

        IntPtr activateMainWnd()
        {
            IntPtr hwnd = activate_win(_win_desc);
            return hwnd;
        }

        IntPtr getMainWnd()
        {
            IntPtr hwnd = getWin(_win_desc);
            return hwnd;
        }


        void moveToStatus()
        {
            AutoItX.MouseMove(_point_State_color.X, _point_State_color.Y, 20);
        }

        int getStatusColor()
        {
            return AutoItX.PixelGetColor(_point_State_color.X, _point_State_color.Y);
        }

        private void click_Start()
        {
            IntPtr hwnd = activateMainWnd();
            Point p = guessStartTest_AbortTestPos();
            AutoItX.MouseClick("LEFT", p.X, p.Y, 1, 10);
        }

        private void click_Retry()
        {
            IntPtr hwnd = activateMainWnd();
            Point p = guessRetryPos();
            AutoItX.MouseClick("LEFT", p.X, p.Y, 1, 10);
        }

        void mouseMove(Point point, int speed = 0)
        {
            AutoItX.MouseMove(point.X, point.Y, speed);
        }

        IntPtr getMenu()
        {
            IntPtr hwnd = getMainWnd();
            return AutoItX.ControlGetHandle(hwnd, "[NAME:menuStrip1]");
        }

        IntPtr getGrid()
        {
            IntPtr hwnd = getMainWnd();
            return AutoItX.ControlGetHandle(hwnd, "[NAME:gridTesting]");
        }

        IntPtr getComboBox4()
        {
            IntPtr hwnd = getMainWnd();
            return AutoItX.ControlGetHandle(hwnd, "[REGEXPCLASS:.*COMBOBOX.*; INSTANCE:4]");
        }

        Rectangle getGridPos()
        {
            IntPtr hwnd = getMainWnd();
            Rectangle rec_wnd = AutoItX.WinGetPos(hwnd);

            IntPtr hmenu = getMenu();
            Rectangle rec_menu = AutoItX.ControlGetPos(hwnd, hmenu);

            IntPtr hgrid = getGrid();
            Rectangle rec_grid = AutoItX.ControlGetPos(hwnd, hgrid);

            int x = rec_wnd.X + rec_grid.X;
            int y = rec_wnd.Y + rec_grid.Y + rec_menu.Height;

            Rectangle rec = new Rectangle(x, y, rec_grid.Width, rec_grid.Height);
            return rec;

        }

        Point guessStatusPos()
        {
            Rectangle rec_grid = getGridPos();

            int x = rec_grid.X + 290;
            int y = rec_grid.Y + 32;

            return new Point(x, y);
        }

        Rectangle getCombo4Pos()
        {
            IntPtr hwnd = getMainWnd();

            IntPtr hc = getComboBox4();
            Rectangle rec_combo4 = AutoItX.ControlGetPos(hwnd, hc);

            Rectangle rec_wnd = AutoItX.WinGetPos(hwnd);
            IntPtr hmenu = getMenu();
            Rectangle rec_menu = AutoItX.ControlGetPos(hwnd, hmenu);

            int x = rec_wnd.X + rec_combo4.X;
            int y = rec_wnd.Y + rec_combo4.Y + rec_menu.Height;

            Rectangle rec = new Rectangle(x, y, rec_combo4.Width, rec_combo4.Height);
            return rec;

        }

        Point guessStartTest_AbortTestPos()
        {
            Rectangle rec_combo4 = getCombo4Pos();

            int x = rec_combo4.X + 200;
            int y = rec_combo4.Y + rec_combo4.Height / 2 + 5;

            return new Point(x, y);
        }

        Point guessRetryPos()
        {
            Point point = guessStartTest_AbortTestPos();

            return new Point(point.X + 65, point.Y);
        }

    
    }
}
