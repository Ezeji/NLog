// 
// Copyright (c) 2004-2006 Jaroslaw Kowalski <jaak@jkowalski.net>
// 
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without 
// modification, are permitted provided that the following conditions 
// are met:
// 
// * Redistributions of source code must retain the above copyright notice, 
//   this list of conditions and the following disclaimer. 
// 
// * Redistributions in binary form must reproduce the above copyright notice,
//   this list of conditions and the following disclaimer in the documentation
//   and/or other materials provided with the distribution. 
// 
// * Neither the name of Jaroslaw Kowalski nor the names of its 
//   contributors may be used to endorse or promote products derived from this
//   software without specific prior written permission. 
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE 
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE 
// ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE 
// LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR 
// CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
// SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
// INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN 
// CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) 
// ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF 
// THE POSSIBILITY OF SUCH DAMAGE.
// 

using System;
using System.Collections;
using System.Reflection;
using System.Globalization;
using System.Xml;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;

using NLog.Internal;
using NLog.Config;

namespace NLog.Internal
{
    /// <summary>
    /// Returns details about current process and thread in a portable manner.
    /// </summary>
    public sealed class ThreadIDHelper
    {
        private static int _currentProcessID;
        private static string _currentProcessName;
        private static string _currentProcessBaseName;
        private static string _currentProcessDirectoryName;

        static ThreadIDHelper()
        {
            _currentProcessID = GetCurrentProcessId();
            StringBuilder sb = new StringBuilder(512);
            GetModuleFileName(IntPtr.Zero, sb, sb.Capacity);
            _currentProcessName = sb.ToString();
            _currentProcessBaseName = Path.GetFileNameWithoutExtension(_currentProcessName);
            _currentProcessDirectoryName = Path.GetDirectoryName(_currentProcessName);
        }
            
#if !NETCF
        [DllImport("kernel32.dll")]
        private extern static int GetCurrentThreadId();

        [DllImport("kernel32.dll")]
        private extern static int GetCurrentProcessId();

        [DllImport("kernel32.dll", SetLastError=true, PreserveSig=true)]
        private static extern uint GetModuleFileName([In] IntPtr hModule, [Out] StringBuilder lpFilename, [In] [MarshalAs(UnmanagedType.U4)] int nSize);
#else
        [DllImport("coredll.dll")]
        private extern static int GetCurrentThreadId();

        [DllImport("coredll.dll")]
        private extern static int GetCurrentProcessId();

        [DllImport("coredll.dll", SetLastError=true)]
        private static extern uint GetModuleFileName([In] IntPtr hModule, [Out] StringBuilder lpFilename, [In] int nSize);
#endif
        /// <summary>
        /// Returns current unmanaged thread ID.
        /// </summary>
        public static int CurrentThreadID
        {
            get { return GetCurrentThreadId(); }
        }

        /// <summary>
        /// Returns current process ID.
        /// </summary>
        public static int CurrentProcessID
        {
            get { return _currentProcessID; }
        }

        /// <summary>
        /// Returns current process name.
        /// </summary>
        public static string CurrentProcessName
        {
            get { return _currentProcessName; }
        }

        /// <summary>
        /// Returns current process name (excluding filename extension, if any).
        /// </summary>
        public static string CurrentProcessBaseName
        {
            get { return _currentProcessBaseName; }
        }
        
        /// <summary>
        /// Returns the base directory where process EXE file resides.
        /// </summary>
        public static string CurrentProcessDirectory
        {
            get { return _currentProcessDirectoryName; }
        }
    }
}
