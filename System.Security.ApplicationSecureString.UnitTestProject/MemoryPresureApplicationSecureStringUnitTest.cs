using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject {
    [TestClass]
    public class MemoryPresureApplicationSecureStringUnitTest {
        [TestMethod]
        public void MemoryPresureCompareTest() {
            Parallel.ForEach(Enumerable.Range(0, 25000).ToArray(), count => {
                var randomPhrase = Generator.MakeMaxLengthRandomString(50);
                using (var testPhraseOne = (ApplicationSecureString)randomPhrase)
                using (var testPhraseTwo = (ApplicationSecureString)(randomPhrase.ToCharArray())) {
                    //Debug.WriteLine(count + ", '" + randomPhrase + "'");
                    testPhraseOne.CreateUnsecuredString().Should().NotBeEmpty();
                    testPhraseOne.CreateUnsecuredString().Should().Be(testPhraseTwo.CreateUnsecuredString());
                }
            });
        }

        [TestMethod]
        public void MemoryPresureLight0To5000CompareTest() {
            var concurrentTupleQueue = new ConcurrentQueue<Tuple<String, ApplicationSecureString, ApplicationSecureString>>();

            Parallel.ForEach(Enumerable.Range(0, 5000).ToArray(), count => {
                var randomPhrase = Generator.MakeMaxLengthRandomString(50);
                concurrentTupleQueue.Enqueue(new Tuple<String, ApplicationSecureString, ApplicationSecureString>(randomPhrase, (ApplicationSecureString)randomPhrase, (ApplicationSecureString)randomPhrase.ToCharArray()));
            });

            try {
                Parallel.ForEach(concurrentTupleQueue.ToArray(), tuple => {
                    tuple.Item1.Should().NotBeEmpty();
                    tuple.Item2.CreateUnsecuredString().Should().NotBeEmpty();
                    tuple.Item3.CreateUnsecuredString().Should().NotBeEmpty();

                    tuple.Item1.Should().Be(tuple.Item2.CreateUnsecuredString());
                    tuple.Item1.Should().Be(tuple.Item2.CreateUnsecuredString());
                });
            } finally {
                Parallel.ForEach(concurrentTupleQueue.ToArray(), tuple => {
                    tuple.Item2.Dispose();
                    tuple.Item3.Dispose();
                });
            }
        }

        [TestMethod]
        public void MemoryPresureMedium0To25000CompareTest() {
            var concurrentTupleQueue = new ConcurrentQueue<Tuple<String, ApplicationSecureString, ApplicationSecureString>>();

            Parallel.ForEach(Enumerable.Range(0, 5000).ToArray(), count => {
                var randomPhrase = Generator.MakeMaxLengthRandomString(50);
                concurrentTupleQueue.Enqueue(new Tuple<String, ApplicationSecureString, ApplicationSecureString>(randomPhrase, (ApplicationSecureString)randomPhrase, (ApplicationSecureString)randomPhrase.ToCharArray()));
            });

            try {
                Parallel.ForEach(concurrentTupleQueue.ToArray(), tuple => {
                    tuple.Item1.Should().NotBeEmpty();
                    tuple.Item2.CreateUnsecuredString().Should().NotBeEmpty();
                    tuple.Item3.CreateUnsecuredString().Should().NotBeEmpty();

                    tuple.Item1.Should().Be(tuple.Item2.CreateUnsecuredString());
                    tuple.Item1.Should().Be(tuple.Item2.CreateUnsecuredString());
                });
            } finally {
                Parallel.ForEach(concurrentTupleQueue.ToArray(), tuple => {
                    tuple.Item2.Dispose();
                    tuple.Item3.Dispose();
                });
            }
        }

        [TestMethod]
        public void MemoryPresureHeavy0To50000CompareTest() {
            var concurrentTupleQueue = new ConcurrentQueue<Tuple<String, ApplicationSecureString, ApplicationSecureString>>();

            Parallel.ForEach(Enumerable.Range(0, 50000).ToArray(), count => {
                var randomPhrase = Generator.MakeMaxLengthRandomString(50);
                concurrentTupleQueue.Enqueue(new Tuple<String, ApplicationSecureString, ApplicationSecureString>(randomPhrase, (ApplicationSecureString)randomPhrase, (ApplicationSecureString)randomPhrase.ToCharArray()));
            });

            try {
                Parallel.ForEach(concurrentTupleQueue.ToArray(), tuple => {
                    tuple.Item1.Should().NotBeEmpty();
                    tuple.Item2.CreateUnsecuredString().Should().NotBeEmpty();
                    tuple.Item3.CreateUnsecuredString().Should().NotBeEmpty();

                    tuple.Item1.Should().Be(tuple.Item2.CreateUnsecuredString());
                    tuple.Item1.Should().Be(tuple.Item2.CreateUnsecuredString());
                });
            } finally {
                Parallel.ForEach(concurrentTupleQueue.ToArray(), tuple => {
                    tuple.Item2.Dispose();
                    tuple.Item3.Dispose();
                });
            }
        }
    }
}
