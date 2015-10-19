using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestProject {
    public static class MemoryScanner {
        #region Constants
        const int PROCESS_QUERY_INFORMATION = 0x0400;
        const int MEM_COMMIT = 0x00001000;
        const int PAGE_READWRITE = 0x04;
        const int PROCESS_WM_READ = 0x0010;
        #endregion

        #region Imports
        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(int hProcess, int lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesRead);

        [DllImport("kernel32.dll")]
        static extern void GetSystemInfo(out SYSTEM_INFO lpSystemInfo);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern int VirtualQueryEx(IntPtr hProcess, IntPtr lpAddress, out MEMORY_BASIC_INFORMATION lpBuffer, uint dwLength);
        #endregion

        #region Structures
        public struct MEMORY_BASIC_INFORMATION {
            public int BaseAddress;
            public int AllocationBase;
            public int AllocationProtect;
            public int RegionSize;
            public int State;
            public int Protect;
            public int lType;
        }

        public struct SYSTEM_INFO {
            public ushort processorArchitecture;
            private ushort reserved;
            public uint pageSize;
            public IntPtr minimumApplicationAddress;
            public IntPtr maximumApplicationAddress;
            public IntPtr activeProcessorMask;
            public uint numberOfProcessors;
            public uint processorType;
            public uint allocationGranularity;
            public ushort processorLevel;
            public ushort processorRevision;
        }
        #endregion

        public static DirectoryInfo GetBaseDirectory() {
            return new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
        }

        public static Process StartProcess(this FileInfo argFileInfo) {
            var process = new Process();
            process.StartInfo.FileName = argFileInfo.FullName;
            process.StartInfo.Arguments = "";
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.Start();
            return process;
        }


        public static IEnumerable<Byte[]> ScanMemoryForMatches(this Process argProcess, Collection<Byte[]> argTargetPatterns) {
            var pathandFileName = argProcess.StartInfo.FileName + ".Dump.txt";
            if (File.Exists(pathandFileName))
                File.Delete(pathandFileName);

            using (var streamWriter = new StreamWriter(pathandFileName)) {
                var sys_info = new SYSTEM_INFO();
                GetSystemInfo(out sys_info);

                var proc_min_address = sys_info.minimumApplicationAddress;
                var proc_max_address = sys_info.maximumApplicationAddress;

                var proc_min_address_l = (long)proc_min_address;
                var proc_max_address_l = (long)proc_max_address;

                var processHandle = OpenProcess(PROCESS_QUERY_INFORMATION | PROCESS_WM_READ, false, argProcess.Id);
                MEMORY_BASIC_INFORMATION mem_basic_info = new MEMORY_BASIC_INFORMATION();

                var bytesRead = 0;
                while (proc_min_address_l < proc_max_address_l) {
                    // 28 = sizeof(MEMORY_BASIC_INFORMATION)
                    VirtualQueryEx(processHandle, proc_min_address, out mem_basic_info, 28);
                    if (mem_basic_info.Protect == PAGE_READWRITE && mem_basic_info.State == MEM_COMMIT) {
                        var buffer = new byte[mem_basic_info.RegionSize];
                        ReadProcessMemory((int)processHandle, mem_basic_info.BaseAddress, buffer, mem_basic_info.RegionSize, ref bytesRead);

                        for (int index = 0; index < mem_basic_info.RegionSize; index++)
                            streamWriter.WriteLine("0x{0} : {1}", (mem_basic_info.BaseAddress + index).ToString("X"), (char)buffer[index]);

                        var tasks = new List<Task<Collection<Byte[]>>>();

                        tasks.Add(Task.Factory.StartNew<Collection<Byte[]>>(() => {
                            var concurrentQueue = new ConcurrentQueue<Byte[]>();
                            Parallel.ForEach(buffer.ScanMemorySegmentFor1CharWideByteMatches(argTargetPatterns).ToArray(), matchBuffer => {
                                concurrentQueue.Enqueue(matchBuffer);
                            });
                            return concurrentQueue.ToCollection();
                        }));

                        tasks.Add(Task.Factory.StartNew<Collection<Byte[]>>(() => {
                            var concurrentQueue = new ConcurrentQueue<Byte[]>();
                            Parallel.ForEach(buffer.ScanMemorySegmentFor2CharWideByteMatches(argTargetPatterns).ToArray(), matchBuffer => {
                                concurrentQueue.Enqueue(matchBuffer);
                            });
                            return concurrentQueue.ToCollection();
                        }));

                        Task.WaitAll(tasks.ToArray());

                        foreach (var task in tasks)
                            foreach (var resultBuffer in task.Result.ToArray())
                                yield return resultBuffer;
                    }

                    proc_min_address_l += mem_basic_info.RegionSize;
                    proc_min_address = new IntPtr(proc_min_address_l);
                }

                yield break;
            }
        }




        public static IEnumerable<Byte[]> ScanMemorySegmentFor2CharWideByteMatches(this Byte[] argTargetByteArray, Collection<Byte[]> argTargetPatterns) {
            var delineator = System.Environment.NewLine;
            var headPointer = -1;
            var tailPointer = 3;

            foreach (var target in argTargetPatterns) {
                var targetPattern = (new String(Encoding.UTF8.GetChars(target))).GetBytes();
                var tempHeadPointer = headPointer;
                var tempTailPointer = tailPointer;
                do {
                    tempHeadPointer = argTargetByteArray.MorrisPrattSearchFirst(targetPattern, tempTailPointer);
                    if (tempHeadPointer >= 0) {
                        tempTailPointer = tempHeadPointer + 1;
                        yield return targetPattern;
                    }
                } while (tempHeadPointer >= 0 && tempTailPointer < argTargetByteArray.Length);
            }
        }

        public static IEnumerable<Byte[]> ScanMemorySegmentFor1CharWideByteMatches(this Byte[] argTargetByteArray, Collection<Byte[]> argTargetPatterns) {
            var delineator = System.Environment.NewLine;
            var headPointer = -1;
            var tailPointer = 3;

            foreach (var targetPattern in argTargetPatterns) {
                var tempHeadPointer = headPointer;
                var tempTailPointer = tailPointer;
                do {
                    tempHeadPointer = argTargetByteArray.MorrisPrattSearchFirst(targetPattern, tempTailPointer);
                    if (tempHeadPointer >= 0) {
                        tempTailPointer = tempHeadPointer + 1;
                        yield return targetPattern;
                    }
                } while (tempHeadPointer >= 0 && tempTailPointer < argTargetByteArray.Length);
            }
        }

        public static IEnumerable<Byte[]> ReadByteArrays(this FileInfo argFieInfo) {
            if (!argFieInfo.Exists)
                throw new Exception("Missing File: " + argFieInfo.FullName);

            var delineator = System.Environment.NewLine;
            var headPointer = -1;
            var tailPointer = 3;

            var delineatorByteArray = Encoding.Default.GetBytes(delineator);
            var targetByteArray = File.ReadAllBytes(argFieInfo.Name);
            try {
                do {
                    headPointer = targetByteArray.MorrisPrattSearchFirst(delineatorByteArray, tailPointer);
                    if (headPointer >= 0) {
                        var length = headPointer - tailPointer;
                        var buffer = new Byte[length];
                        Array.Copy(targetByteArray, tailPointer, buffer, 0, length);
                        tailPointer = (headPointer + delineatorByteArray.Length);
                        yield return buffer;
                    }
                } while (headPointer >= 0 && tailPointer < targetByteArray.Length);
            } finally {
                Array.Clear(targetByteArray, 0, targetByteArray.Length);
                Array.Clear(delineatorByteArray, 0, delineatorByteArray.Length);
            }
            yield break;
        }

        public static Collection<T> ToCollection<T>(this IEnumerable<T> enumerable) {
            var collection = new Collection<T>();
            foreach (T i in enumerable)
                collection.Add(i);
            return collection;
        }
    }

    public static class Extentions {
        public static byte[] GetBytes(this string str) {
            var bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        public static string GetString(this byte[] bytes) {
            var chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }

        ///// <summary>
        ///// Reports the index of the first occurrence of the specified sub-array in this instance, using Morris-Pratt algorithm.
        ///// </summary>
        ///// <typeparam name="T">The type of two arrays.</typeparam>
        ///// <param name="t">The longer array.</param>
        ///// <param name="p">The shorter array to seek.</param>
        ///// <returns>The zero-based index position of value if <paramref name="p"/> is found, or -1 if it is not.</returns>
        ///// <exception cref="ArgumentNullException"><paramref name="t"/> or <paramref name="p"/> is null.</exception>
        //public static int MorrisPrattSearchFirst<T>(this T[] t, T[] p)
        //    where T : IEquatable<T> {
        //    return MorrisPrattSearchFirst(t, p, 0);
        //}

        /// <summary>
        /// Reports the index of the first occurrence of the specified sub-array in this instance, using Morris-Pratt algorithm.
        /// </summary>
        /// <typeparam name="T">The type of two arrays.</typeparam>
        /// <param name="t">The longer array.</param>
        /// <param name="p">The shorter array to seek.</param>
        /// <param name="startIndex">The search starting position.</param>
        /// <returns>The zero-based index position of value if <paramref name="p"/> is found, or -1 if it is not.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="t"/> or <paramref name="p"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <para><paramref name="startIndex"/> is greater than the length of <paramref name="t"/> array.</para>
        /// <para>-- Or --</para>
        /// <para><paramref name="startIndex"/> is negative.</para> 
        /// </exception>
        public static int MorrisPrattSearchFirst<T>(this T[] t, T[] p, int startIndex)
                where T : IEquatable<T> {
            return MorrisPrattSearchFirst(t, p, startIndex, t.Length - startIndex);
        }

        /// <summary>
        /// Reports the index of the first occurrence of the specified sub-array in this instance, using Morris-Pratt algorithm.
        /// </summary>
        /// <typeparam name="T">The type of two arrays.</typeparam>
        /// <param name="t">The longer array.</param>
        /// <param name="p">The shorter array to seek.</param>
        /// <param name="startIndex">The search starting position.</param>
        /// <param name="count">The number of character positions to examine.</param>
        /// <returns>The zero-based index position of value if <paramref name="p"/> is found, or -1 if it is not.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="t"/> or <paramref name="p"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <para><paramref name="startIndex"/> is greater than the length of <paramref name="t"/> array.</para>
        /// <para>-- Or --</para>
        /// <para><paramref name="startIndex"/> or <paramref name="count"/> is negative.</para>
        /// <para>-- Or --</para>
        /// <para><paramref name="count"/> is greater than the length of <paramref name="t"/> array minus <paramref name="startIndex"/>.</para>
        /// </exception>
        public static int MorrisPrattSearchFirst<T>(this T[] t, T[] p, int startIndex, int count)
            where T : IEquatable<T> {
            if (t == null)
                throw new ArgumentNullException("t");

            if (p == null)
                throw new ArgumentNullException("p");

            int tLength = t.Length, pLength = p.Length, rLength = startIndex + count;

            if (startIndex >= tLength)
                throw new ArgumentOutOfRangeException("Value is greater than the length of this array.", "startIndex");

            if (startIndex < 0)
                throw new ArgumentOutOfRangeException("Value is negative.", "startIndex");

            if (rLength > tLength)
                throw new ArgumentOutOfRangeException("Value is greater than the length of this array minus startIndex.", "count");

            if (count < 0)
                throw new ArgumentOutOfRangeException("Value is negative.", "count");

            if (p.Length > t.Length)
                return -1;

            int[] failure = new int[100];
            try {
                for (int i = 1, j = failure[0] = -1; i < pLength; i++) {
                    while (j >= 0 && !p[j + 1].Equals(p[i]))
                        j = failure[j];

                    if (p[j + 1].Equals(p[i]))
                        j++;

                    failure[i] = j;
                }

                for (int i = startIndex, j = -1; i < rLength; i++) {
                    while (j >= 0 && !p[j + 1].Equals(t[i]))
                        j = failure[j];

                    if (p[j + 1].Equals(t[i]))
                        j++;

                    if (j == p.Length - 1)
                        return i - p.Length + 1;
                }

                return -1;
            } finally {
                Array.Clear(failure, 0, failure.Length);
            }
        }

        public static int MorrisPrattSearchFirst<T>(this IEnumerable<T> t, IEnumerable<T> p, int startIndex, int count)
            where T : IEquatable<T> {
            if (t == null)
                throw new ArgumentNullException("t");

            if (p == null)
                throw new ArgumentNullException("p");

            int tLength = t.Count(), pLength = p.Count(), rLength = startIndex + count;

            if (startIndex >= tLength)
                throw new ArgumentOutOfRangeException("Value is greater than the length of this array.", "startIndex");

            if (startIndex < 0)
                throw new ArgumentOutOfRangeException("Value is negative.", "startIndex");

            if (rLength > tLength)
                throw new ArgumentOutOfRangeException("Value is greater than the length of this array minus startIndex.", "count");

            if (count < 0)
                throw new ArgumentOutOfRangeException("Value is negative.", "count");

            if (pLength > tLength)
                return -1;

            int[] failure = new int[100];
            try {
                for (int i = 1, j = failure[0] = -1; i < pLength; i++) {
                    while (j >= 0 && !p.ElementAt(j + 1).Equals(p.ElementAt(i)))
                        j = failure[j];

                    if (p.ElementAt(j + 1).Equals(p.ElementAt(i)))
                        j++;

                    failure[i] = j;
                }

                for (int i = startIndex, j = -1; i < rLength; i++) {
                    while (j >= 0 && !p.ElementAt(j + 1).Equals(t.ElementAt(i)))
                        j = failure[j];

                    if (p.ElementAt(j + 1).Equals(t.ElementAt(i)))
                        j++;

                    if (j == pLength - 1)
                        return i - pLength + 1;
                }

                return -1;
            } finally {
                Array.Clear(failure, 0, failure.Length);
            }
        }
    }

}
