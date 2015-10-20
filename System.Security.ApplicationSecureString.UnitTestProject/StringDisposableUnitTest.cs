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
    public class StringDisposableUnitTest {
        [TestMethod]
        public void StringDisposableTest() {
            var testPhrase = Generator.MakeRandomString(34);
            var applicationSecureString = (ApplicationSecureString)testPhrase;
            using (var stringDisposable = applicationSecureString.CreateStringDisposable()) {
                stringDisposable.UnsecuredString.Should().NotBeEmpty();
                stringDisposable.UnsecuredString.Should().Be(testPhrase);
            }
        }

        [TestMethod]
        public void StringDisposable1To1000TimesTest() {
            foreach (var count in Enumerable.Range(1, 1000)) {
                var testPhrase = Generator.MakeRandomString(count);
                var applicationSecureString = (ApplicationSecureString)testPhrase;
                using (var stringDisposable = applicationSecureString.CreateStringDisposable()) {
                    stringDisposable.UnsecuredString.Should().NotBeEmpty();
                    stringDisposable.UnsecuredString.Should().Be(testPhrase);
                }
            }
        }

        [TestMethod]
        public void StringDisposableImplicitCastTest() {
            var testPhrase = Generator.MakeRandomString(12);
            var applicationSecureString = (ApplicationSecureString)testPhrase;
            using (var stringDisposable = applicationSecureString.CreateStringDisposable()) {
                testPhrase.Should().Be(stringDisposable);

                String text = stringDisposable;
                text.Should().NotBeEmpty();
                text.Should().Be(testPhrase);
            }
        }

        [TestMethod]
        public void StringDisposablEmptyStringAndUnpinnedTest() {
            var testPhrase = Generator.MakeRandomString(1225);
            var applicationSecureString = (ApplicationSecureString)testPhrase;
            var stringThatWillBeAnEmptyStringAndUnpinned = String.Empty;

            using (var stringDisposable = applicationSecureString.CreateStringDisposable()) {
                stringThatWillBeAnEmptyStringAndUnpinned = stringDisposable;
                stringThatWillBeAnEmptyStringAndUnpinned.Should().NotBeEmpty();
                stringThatWillBeAnEmptyStringAndUnpinned.Should().Be(testPhrase);
            }

            stringThatWillBeAnEmptyStringAndUnpinned.Should().BeNullOrWhiteSpace();
            stringThatWillBeAnEmptyStringAndUnpinned.Length.Should().Be(testPhrase.Length);
        }

        [TestMethod]
        public void StringDisposablEmptyStringAndUnpinnedFrom1To5000Test() {
            Parallel.ForEach(Enumerable.Range(1, 5000).ToArray(), count => {
                var testPhrase = Generator.MakeRandomString(count);
                Debug.WriteLine(count + ", '" + testPhrase + "'");
                var applicationSecureString = (ApplicationSecureString)testPhrase;
                var stringThatWillBeAnEmptyStringAndUnpinned = String.Empty;

                using (var stringDisposable = applicationSecureString.CreateStringDisposable()) {
                    stringThatWillBeAnEmptyStringAndUnpinned = stringDisposable;
                    stringThatWillBeAnEmptyStringAndUnpinned.Should().NotBeEmpty();
                    stringThatWillBeAnEmptyStringAndUnpinned.Should().Be(testPhrase);
                }

                stringThatWillBeAnEmptyStringAndUnpinned.Should().BeNullOrWhiteSpace();
                stringThatWillBeAnEmptyStringAndUnpinned.Length.Should().Be(testPhrase.Length);
            });
        }

        [TestMethod]
        public void StringDisposableBuiltUpCollectionFrom1To5000Test() {
            var concurrentQueue = new ConcurrentQueue<String>();
            Parallel.ForEach(Enumerable.Range(1, 5000).ToArray(), count => {
                var testPhrase = Generator.MakeRandomString(count);
                Debug.WriteLine(count + ", '" + testPhrase + "'");
                var applicationSecureString = (ApplicationSecureString)testPhrase;

                using (var stringDisposable = applicationSecureString.CreateStringDisposable()) {
                    var stringThatWillBeAnEmptyStringAndUnpinned = (string)stringDisposable;
                    concurrentQueue.Enqueue(stringThatWillBeAnEmptyStringAndUnpinned);
                    stringThatWillBeAnEmptyStringAndUnpinned.Should().NotBeEmpty();
                    stringThatWillBeAnEmptyStringAndUnpinned.Should().Be(testPhrase);
                }
            });
        }
    }
}
