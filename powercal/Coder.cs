using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Threading;
using System.Diagnostics;

using AutoIt;

namespace PowerCalibration
{
    class Coder
    {
        int _cred = 0xFF0000;
        int _cgreen = 0x008000;
        TimeSpan _timeout;

        string _win_desc = "[REGEXPTITLE:Ember Bootloader and Range Test .*]";

        float _dpiX, _dpiY;

        Ember _ember;
        public Ember Ember { get { return _ember; } set { _ember = value; } }

        public delegate void StatusHandler(object sender, string status_txt);
        public event StatusHandler Status_Event;

        public Coder(TimeSpan timeout)
        {
            _timeout = timeout;

            using (Graphics graphics = Graphics.FromHwnd(IntPtr.Zero))
            {
                _dpiX = graphics.DpiX;
                _dpiY = graphics.DpiY;
            }

        }

        public void Code(CancellationToken cancel)
        {
            if (cancel.IsCancellationRequested)
                return;

            if (Properties.Settings.Default.Disable_ReadProtection_BeforeCoding)
            {
                try
                {
                    // Disable read protection
                    disable_read_protection();
                }
                catch (Exception ex)
                {
                    string error = ex.Message;
                    //throw;
                    fire_status("Unable to disable read protection: " + ex.Message);
                }
            }


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
                        Point p = moveToStatus();
                        Thread.Sleep(2000);

                        msg = string.Format("Red pixel at location X={0}, Y={1} after {2:F2} s",
                            p.X, p.Y, watch.Elapsed.TotalSeconds);
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
                    Point p = moveToStatus();

                    msg = string.Format("Timeout after {0:hh\\:mm\\:ss}. Unable to detect green or read pixel at location X={1}, Y={2}",
                        watch.Elapsed, p.X, p.Y);
                    throw new Exception(msg);
                }

                if (AutoItX.WinActive(hwnd) == 0)
                {
                    hwnd = activateMainWnd();
                    Point p = moveToStatus();
                }
            }

            // minimize if coding passed
            if (pixel_color == _cgreen && Properties.Settings.Default.CodeMinimizedOnPASS)
            {
                AutoItX.WinSetState(hwnd, AutoItX.SW_SHOWMINIMIZED);
            }

        }

        void disable_read_protection()
        {
            // Disable read protection
            fire_status("Disable read protection");
            bool disable_rd_prot_success = false;
            string disable_output = "";
            for (int i = 0; i < 5; i++)
            {
                try
                {
                    disable_output = Ember.EnableRdProt(false);
                }
                catch (Exception ex)
                {
                    disable_output = ex.Message;
                }
                if (
                    disable_output.Contains("Disable Read Protection") ||
                    disable_output.Contains("Read Protection is already disabled"))
                {
                    disable_rd_prot_success = true;
                    break;
                }
            }

            if (!disable_rd_prot_success)
            {
                //fire_status("Unable to disable read protection: " + disable_output);
                throw new Exception("Unable to disable read protection: " + disable_output);
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


        Point moveToStatus()
        {
            Point point = guessStatusPos();
            AutoItX.MouseMove(point.X, point.Y, 20);
            return point;
        }

        int getStatusColor()
        {
            Point p = guessStatusPos();
            return AutoItX.PixelGetColor(p.X, p.Y);
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

            int x = 290;
            int y = 32;
            if (_dpiX >= 120)
            {
                x = 295;
                y = 40;
            }

            x = rec_grid.X + x;
            y = rec_grid.Y + y;

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

            int x = 200;
            int y = 5;
            if (_dpiX >= 120)
            {
                x = 250;
                y = 10;
            }

            x = rec_combo4.X + x;
            y = rec_combo4.Y + rec_combo4.Height / 2 + y;

            return new Point(x, y);
        }

        Point guessRetryPos()
        {
            Point point = guessStartTest_AbortTestPos();

            int x = 65;
            if (_dpiX >= 120)
            {
                x = 75;
            }

            return new Point(point.X + x, point.Y);
        }

        void fire_status(string msg)
        {
            if (Status_Event != null)
            {
                Status_Event(this, msg);
            }
        }

    }
}
