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

            var path = Path.Combine(Config.FileDirectory, Config.FileName);
            PathLabel.Text = path;
            FileStream = File.OpenWrite(path);

            var t = new Thread(KeepAliveCycle);
            t.IsBackground = true;
            t.Start();
        }

        private void KeepAliveCycle()
        {
            long writeCount = 0;
            long idleCount = 0;
            while (true)
            {
                if (IsUserActive())
                {
                    WriteToKeepAliveFile();
                    UpdateStatusText(++writeCount, idleCount);
                }
                else
                {
                    UpdateStatusText(writeCount, ++idleCount);
                }
                Thread.Sleep(Config.WriteInterval);
            }
        }

        private bool IsUserActive()
        {
            LastInputInfo info = LastInputInfo.Create();
            if (!Win32Bindings.GetLastInputInfo(ref info))
            {
                throw new Exception("Could not get time of last user activity");
            }

            long idleTime = Environment.TickCount - info.dwTime;
            return idleTime <= Config.TimeToIdle;
        }

        private void WriteToKeepAliveFile()
        {
            if (FileStream.Length >= Config.MaximumFileSize)
            {
                FileStream.SetLength(0);
            }
            FileStream.WriteByte(0);
            FileStream.Flush();
        }

        private void UpdateStatusText(long writeCount, long idleCount)
        {
            WriteCountLabel.Dispatcher.Invoke(new Action(() =>
            {
                WriteCountLabel.Text = string.Format("write cycles: {0}", writeCount);
            }));

            IdleCountLabel.Dispatcher.Invoke(new Action(() =>
            {
                IdleCountLabel.Text = string.Format("idle cycles: {0}", idleCount);
            }));
        }

        private void Window_Closed_1(object sender, EventArgs e)
        {
            FileStream.Close();
        }
    }
}
