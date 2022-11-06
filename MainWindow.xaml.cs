using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Forms;
using System.Threading;
using System.Drawing.Imaging;
using System.Drawing;
using Loamen.KeyMouseHook;
using Gma.System.MouseKeyHook;
using System.Runtime.InteropServices;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;

namespace PianoTilesPlayer
{
    sealed class Win32
    {
        [DllImport("user32.dll")]
        static extern IntPtr GetDC(IntPtr hwnd);

        [DllImport("user32.dll")]
        static extern Int32 ReleaseDC(IntPtr hwnd, IntPtr hdc);

        [DllImport("gdi32.dll")]
        static extern uint GetPixel(IntPtr hdc, int nXPos, int nYPos);

        static public System.Drawing.Color GetPixelColor(int x, int y)
        {
            IntPtr hdc = GetDC(IntPtr.Zero);
            uint pixel = GetPixel(hdc, x, y);
            ReleaseDC(IntPtr.Zero, hdc);
            Color color = Color.FromArgb((int)(pixel & 0x000000FF),
                         (int)(pixel & 0x0000FF00) >> 8,
                         (int)(pixel & 0x00FF0000) >> 16);
            return color;
        }
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Subscribe(Hook.GlobalEvents());
        }

        private IKeyboardMouseEvents m_Events;
        private void Subscribe(IKeyboardMouseEvents events)
        {
            m_Events = events;
            m_Events.KeyPress += hookKeyPress;
        }

        Point pos1 = new System.Drawing.Point();
        Point pos2 = new System.Drawing.Point();
        Point pos3 = new System.Drawing.Point();
        Point pos4 = new System.Drawing.Point();
        bool calibrating = false;
        bool play = false;
        bool cd1 = false;
        bool cd2 = false;
        bool cd3 = false;
        bool cd4 = false;

        private void hookKeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 'q') //calibrate
            {
                x1CurrPos.Text = System.Windows.Forms.Cursor.Position.X.ToString();
                y1CurrPos.Text = System.Windows.Forms.Cursor.Position.Y.ToString();
                pos1.X = System.Windows.Forms.Cursor.Position.X;
                pos1.Y = System.Windows.Forms.Cursor.Position.Y;
            }
            if (e.KeyChar == 'w') //calibrate
            {
                x2CurrPos.Text = System.Windows.Forms.Cursor.Position.X.ToString();
                y2CurrPos.Text = System.Windows.Forms.Cursor.Position.Y.ToString();
                pos2.X = System.Windows.Forms.Cursor.Position.X;
                pos2.Y = System.Windows.Forms.Cursor.Position.Y;
            }
            if (e.KeyChar == 'e') //calibrate
            {
                x3CurrPos.Text = System.Windows.Forms.Cursor.Position.X.ToString();
                y3CurrPos.Text = System.Windows.Forms.Cursor.Position.Y.ToString();
                pos3.X = System.Windows.Forms.Cursor.Position.X;
                pos3.Y = System.Windows.Forms.Cursor.Position.Y;
            }
            if (e.KeyChar == 'r') //calibrate
            {
                x4CurrPos.Text = System.Windows.Forms.Cursor.Position.X.ToString();
                y4CurrPos.Text = System.Windows.Forms.Cursor.Position.Y.ToString();
                pos4.X = System.Windows.Forms.Cursor.Position.X;
                pos4.Y = System.Windows.Forms.Cursor.Position.Y;
            }
            if (e.KeyChar == 'f') //stop
            {
                if (calibrating)
                    m_Events.MouseMove -= hookMouseMove;
                else
                    m_Events.MouseMove += hookMouseMove;
                calibrating = !calibrating;
            }
            if (e.KeyChar == 'p') //start
            {
                if (!play)
                {
                    if (calibrating)
                        m_Events.MouseMove -= hookMouseMove;
                    Status.Text = "Running";
                    play = true;

                    new Thread(() =>
                    {
                        int width = 1920;
                        int height = 1080;
                        var bitmap = new Bitmap(width, height);
                        var graphics = Graphics.FromImage(bitmap);
                        var size = new Size(width, height);
                        Color pix;
                        Point ePoint = Point.Empty;
                        
                        while (play)
                        {
                            graphics.CopyFromScreen(ePoint, ePoint, size);
                            pix = bitmap.GetPixel(pos1.X, pos1.Y);
                            if (pix.B < 80)
                            {
                                if (!cd1)
                                {
                                    LeftMouseClick(pos1.X, pos1.Y);
                                    cd1 = true;
                                }
                            }
                            else
                            {
                                cd1 = false;
                            }
                            pix = bitmap.GetPixel(pos2.X, pos2.Y);
                            if (pix.B < 80)
                            {
                                if (!cd2)
                                {
                                    LeftMouseClick(pos2.X, pos2.Y);
                                    cd2 = true;
                                }
                            }
                            else
                            {
                                cd2 = false;
                            }
                            pix = bitmap.GetPixel(pos3.X, pos3.Y);
                            if (pix.B < 80)
                            {
                                if (!cd3)
                                {
                                    LeftMouseClick(pos3.X, pos3.Y);
                                    cd3 = true;
                                }
                            }
                            else
                            {
                                cd3 = false;
                            }
                            pix = bitmap.GetPixel(pos4.X, pos4.Y);
                            if (pix.B < 80)
                            {
                                if (!cd4)
                                {
                                    LeftMouseClick(pos4.X, pos4.Y);
                                    cd4 = true;
                                }
                            }
                            else
                            {
                                cd4 = false;
                            }
                        }
                    }).Start();
                }
                else
                {
                    cd1 = false;
                    cd2 = false;
                    cd3 = false;
                    cd4 = false;
                    play = false;
                    Status.Text = "Ready";
                }
            }
        }

        private void hookMouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            xCurrPos.Text = System.Windows.Forms.Cursor.Position.X.ToString();
            yCurrPos.Text = System.Windows.Forms.Cursor.Position.Y.ToString();
            var color = Win32.GetPixelColor(System.Windows.Forms.Cursor.Position.X, System.Windows.Forms.Cursor.Position.Y);
            RGB.Text = color.R.ToString() + "," + color.G.ToString() + "," + color.B.ToString();
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern bool SetCursorPos(int x, int y);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        static extern uint GetPixel(IntPtr hdc, int nXPos, int nYPos);

        public const int MOUSEEVENTF_LEFTDOWN = 0x02;
        public const int MOUSEEVENTF_LEFTUP = 0x04;

        //This simulates a left mouse click
        public static void LeftMouseClick(int xpos, int ypos)
        {
            mouse_event(MOUSEEVENTF_LEFTUP, xpos, ypos, 0, 0);
            SetCursorPos(xpos, ypos);
            mouse_event(MOUSEEVENTF_LEFTDOWN, xpos, ypos, 0, 0);
        }
    }
}