using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace DriveKeepAlive.Win32
{
    static class Win32Bindings
    {
        [DllImport("user32.dll")]
        public static extern bool GetLastInputInfo(ref LastInputInfo plii);
    }

    [StructLayout(LayoutKind.Sequential)]
    struct LastInputInfo
    {
        public static readonly uint SizeOf = (uint)Marshal.SizeOf(typeof(LastInputInfo));

        [MarshalAs(UnmanagedType.U4)]
        public UInt32 cbSize;
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 dwTime;

        public static LastInputInfo Create()
        {
            LastInputInfo info = new LastInputInfo();
            info.cbSize = LastInputInfo.SizeOf;
            return info;
        }
    }
}
