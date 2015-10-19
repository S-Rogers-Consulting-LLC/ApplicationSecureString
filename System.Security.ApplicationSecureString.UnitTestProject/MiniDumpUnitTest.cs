using System.Diagnostics;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject {
    [TestClass]
    public class MiniDumpUnitTest {
        [TestMethod]
        public void DumpHeap() {
            Debug.WriteLine(@"Start ->======================================================================================");
            var executingAssemblyPathWithRandomFileName = MiniDump.GetExecutingAssemblyPathWithRandomFileName();
            Debug.WriteLine(executingAssemblyPathWithRandomFileName.FullName);
            using (var fileStream = new FileStream(executingAssemblyPathWithRandomFileName.FullName, FileMode.OpenOrCreate)) {
                MiniDump.WriteDumpForProcess(Process.GetCurrentProcess(), fileStream, MiniDump.DumpTypes.MiniDumpWithFullMemoryInfo);
            }
            Debug.WriteLine(@"Stop  ->======================================================================================");
        }
    }
}
