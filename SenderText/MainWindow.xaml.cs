using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SenderText
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Process p;
        public MainWindow()
        {
            InitializeComponent();
        }

        ~MainWindow()
        {
            if(p!=null)
                if (!p.HasExited)
                    p.Kill();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (p == null)
                {
                    StartNotepadAndSetText();
                }
                else
                {
                    if (!p.HasExited)
                        p.Kill();
                    StartNotepadAndSetText();
                }

                ShowWindow(p.MainWindowHandle, 9);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }   
        }

        private void StartNotepadAndSetText()
        {
            p = new Process() { StartInfo = new ProcessStartInfo("notepad.exe") { CreateNoWindow = false, WindowStyle = ProcessWindowStyle.Minimized } };

            if (p.Start())
            {
                IntPtr childWin;
                int n=0;
                do
                {
                    childWin = FindWindowEx(p.MainWindowHandle, new IntPtr(0), "Edit", null);
                    n++;
                } while (childWin == IntPtr.Zero || n < 100);

                if (SendMessage(childWin, 0x000C, 0, this.TextBox.Text) == 0)
                    throw new Exception("Ошибка вставки текста");
            }
        }

        [DllImport("user32.dll", EntryPoint = "FindWindowEx")]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("user32.dll")]        
        public static extern int SendMessage(IntPtr hWnd, int uMsg, int wParam, string lParam);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool PostMessage(IntPtr hWnd, int Msg, int wParam, string lParam);

        [DllImport("user32.dll", SetLastError = true)]
        static extern long SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr SetFocus(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetActiveWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
    }
}
