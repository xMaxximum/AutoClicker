using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;

namespace AutoClicker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [DllImport("user32")]
        public static extern int SetCursorPos(int x, int y);

        [DllImport("user32.dll", EntryPoint = "mouse_event")]
        internal static extern void ExecuteMouseEvent(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out Point point);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetCursorPos(ref Win32Point pt);

        //Mouse actions
        private const int MOUSEEVENTF_MOVE = 0x0001;
        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;

        public static double X_POSITION;
        public static double Y_POSITION;

        public static IEnumerable<object> Enumarable { get; private set; }

        [StructLayout(LayoutKind.Sequential)]
        internal struct Win32Point
        {
            public Int32 X;
            public Int32 Y;
        };

        public MainWindow()
        {
            InitializeComponent();
        }

        private void SetClickLocation_Button_Click(object sender, RoutedEventArgs e)
        {

            var darkWindow = new Window()
            {
                Background = Brushes.Black,
                Opacity = 0.4,
                AllowsTransparency = true,
                WindowStyle = WindowStyle.None,
                WindowState = WindowState.Maximized,
                Topmost = true
            };

            darkWindow.MouseLeftButtonDown += (s, e) =>
            {
                var transform = PresentationSource.FromVisual(this).CompositionTarget.TransformToDevice;
                var mouse = transform.Transform(GetMousePosition());
                darkWindow.Close();


                X_POSITION = mouse.X;
                Y_POSITION = mouse.Y;
            };

            darkWindow.Show();
        }

        public static Point GetMousePosition()
        {
            var w32Mouse = new Win32Point();
            GetCursorPos(ref w32Mouse);

            return new Point(w32Mouse.X, w32Mouse.Y);
        }

        public static void DoMouseClick(double x, double y)
        {
            SetCursorPos((int)x, (int)y);

            ExecuteMouseEvent(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, (int)x, (int)y, 0, 0);

            foreach (var _ in Enumerable.Range(0, 1000))
            {
                ExecuteMouseEvent(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, (int)x, (int)y, 0, 0);
            }


        }

        private void MouseClick_button_Click(object sender, RoutedEventArgs e)
        {
            DoMouseClick(X_POSITION, Y_POSITION);
        }
    }
}
