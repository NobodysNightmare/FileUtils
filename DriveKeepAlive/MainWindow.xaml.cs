using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using DriveKeepAlive.Win32;

namespace DriveKeepAlive
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Stream FileStream;
        private AppSettings Config = AppSettings.Default;

        public MainWindow()
        {
            InitializeComponent();
            FileStream = File.OpenWrite(Path.Combine(Config.FileDirectory, Config.FileName));
            var t = new Thread(() =>
            {
                long writeCount = 0;
                while (true)
                {
                    if (IsUserActive())
                    {
                        WriteToBusyFile();
                        UpdateStatusText(++writeCount);
                    }
                    Thread.Sleep(Config.WriteInterval);
                }
            });
            t.IsBackground = true;
            t.Start();
        }

        private bool IsUserActive()
        {
            LastInputInfo info = new LastInputInfo();
            info.cbSize = LastInputInfo.SizeOf;
            if (!Win32Bindings.GetLastInputInfo(ref info))
            {
                throw new Exception("Could not get time of last user activity");
            }

            long idleTime = Environment.TickCount - info.dwTime;
            return idleTime <= Config.TimeToIdle;
        }

        private void WriteToBusyFile()
        {
            if (FileStream.Length >= Config.MaximumFileSize)
            {
                FileStream.SetLength(0);
            }
            FileStream.WriteByte(0);
            FileStream.Flush();
        }

        private void UpdateStatusText(long writeCount)
        {
            StatusText.Dispatcher.Invoke(new Action(() =>
            {
                StatusText.Text = string.Format("{0} times written", writeCount);
            }));
        }

        private void Window_Closed_1(object sender, EventArgs e)
        {
            FileStream.Close();
        }
    }
}
