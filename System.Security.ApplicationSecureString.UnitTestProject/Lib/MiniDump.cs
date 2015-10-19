using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace UnitTestProject {
    public static class MiniDump {
        #region Public Enumeration
        public enum DumpTypes {
            MiniDumpNormal = 0,
            MiniDumpWithDataSegs = 1,
            MiniDumpWithFullMemory = 2,
            MiniDumpWithHandleData = 4,
            MiniDumpFilterMemory = 8,
            MiniDumpScanMemory = 16,
            MiniDumpWithUnloadedModules = 32,
            MiniDumpWithIndirectlyReferencedMemory = 64,
            MiniDumpFilterModulePaths = 128,
            MiniDumpWithProcessThreadData = 256,
            MiniDumpWithPrivateReadWriteMemory = 512,
            MiniDumpWithoutOptionalData = 1024,
            MiniDumpWithFullMemoryInfo = 2048,
            MiniDumpWithThreadInfo = 4096,
            MiniDumpWithCodeSegs = 8192,
        }
        #endregion

        #region Public Methods  
        public static FileInfo GetExecutingAssemblyPathWithRandomFileName() {
            return new FileInfo(Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), new FileInfo(Path.GetTempFileName()).Name.Replace(".tmp", ".dmp")));
        }

        public static void WriteDumpForProcess(Process argProcess, FileStream argFileStream, DumpTypes argDumpType) {
            if (!MiniDumpWriteDump(Process.GetCurrentProcess().Handle, (uint)argProcess.Id, argFileStream.SafeFileHandle, argDumpType, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero))
                throw new Win32Exception(Marshal.GetLastWin32Error(), "Error calling MiniDump.");
        }
        #endregion

        [DllImportAttribute("dbghelp.dll")]
        [return: MarshalAsAttribute(UnmanagedType.Bool)]
        private static extern bool MiniDumpWriteDump([In] IntPtr hProcess, uint ProcessId, SafeFileHandle hFile, DumpTypes DumpType, [In] IntPtr ExceptionParam, [In] IntPtr UserStreamParam, [In] IntPtr CallbackParam);
    }
}
