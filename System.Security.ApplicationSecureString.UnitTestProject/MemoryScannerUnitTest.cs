using System;
using System.IO;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject {
    [TestClass]
    public class MemoryScannerUnitTest {
        [TestMethod]
        public void UnsecuredMemoryScannerTest() {
            var targetPatternArrays = MemoryScanner.ReadByteArrays(new FileInfo("Program.TextFile.txt")).ToCollection();

            var directoryInfo = MemoryScanner.GetBaseDirectory();
            var fileInfo = new FileInfo(Path.Combine(directoryInfo.FullName, "UnsecuredApp.exe"));

            if (!fileInfo.Exists)
                throw new Exception("Does not Exist: " + fileInfo.FullName);

            var process = fileInfo.StartProcess();
            try {
                var result = process.ScanMemoryForMatches(targetPatternArrays).ToArray();
                result.Should().NotBeEmpty();
            } finally {
                process.Kill();
            }
        }

        [TestMethod]
        public void SecuredMemoryScannerTest() {
            var targetPatternArrays = MemoryScanner.ReadByteArrays(new FileInfo("Program.TextFile.txt")).ToCollection();

            var directoryInfo = MemoryScanner.GetBaseDirectory();
            var fileInfo = new FileInfo(Path.Combine(directoryInfo.FullName, "SecuredApp.exe"));

            if (!fileInfo.Exists)
                throw new Exception("Does not Exist: " + fileInfo.FullName);

            var process = fileInfo.StartProcess();
            try {
                var result = process.ScanMemoryForMatches(targetPatternArrays).ToArray();
                result.Should().BeEmpty();
            } finally {
                process.Kill();
            }
        }
    }
}
