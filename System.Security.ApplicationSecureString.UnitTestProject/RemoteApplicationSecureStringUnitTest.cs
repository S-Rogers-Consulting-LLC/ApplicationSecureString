using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject {
    [TestClass]
    public class RemoteApplicationSecureStringUnitTest {
        [TestMethod]
        public void SimpleRemoteParametersConcatTest() {
            Debug.WriteLine(@"Start ->======================================================================================");
            foreach (var userString in UserStrings.GetUserStrings(Assembly.GetExecutingAssembly()))
               Debug.WriteLine("IsInterned: " + (null != String.IsInterned(userString)).ToString() + @"', UserStrings: '" + userString + @"'");
            Debug.WriteLine(@"Start ->======================================================================================");

            var randomPhrase = Generator.MakeRandomString(51);
            using (var testPhraseOne = (ApplicationSecureString)randomPhrase)
            using (var testPhraseTwo = (ApplicationSecureString)(randomPhrase.ToCharArray()))
            using (var testPhraseThree = RemotingService.SimpleParametersConcat(testPhraseOne, testPhraseTwo)) {
                testPhraseThree.CreateUnsecuredString().Should().NotBeEmpty();
                testPhraseThree.CreateUnsecuredString().Should().Be(testPhraseOne + testPhraseTwo);
            }

            Debug.WriteLine(@"Stop ->======================================================================================");
            foreach (var userString in UserStrings.GetUserStrings(Assembly.GetExecutingAssembly()))
               Debug.WriteLine("IsInterned: " + (null != String.IsInterned(userString)).ToString() + @"', UserStrings: '" + userString + @"'");
            Debug.WriteLine(@"Stop ->======================================================================================");
        }

        [TestMethod]
        public void SimpleRemoteParametersConcatForEmptyStringsTest() {
            Debug.WriteLine(@"Start ->======================================================================================");
            foreach (var userString in UserStrings.GetUserStrings(Assembly.GetExecutingAssembly()))
               Debug.WriteLine("IsInterned: " + (null != String.IsInterned(userString)).ToString() + @"', UserStrings: '" + userString + @"'");
            Debug.WriteLine(@"Start ->======================================================================================");

            using (var testPhraseOne = (ApplicationSecureString)String.Empty)
            using (var testPhraseTwo = (ApplicationSecureString)(String.Empty.ToCharArray()))
            using (var testPhraseThree = RemotingService.SimpleParametersConcat(testPhraseOne, testPhraseTwo)) {
                testPhraseThree.CreateUnsecuredString().Should().Be(String.Empty);
            }

            Debug.WriteLine(@"Stop ->======================================================================================");
            foreach (var userString in UserStrings.GetUserStrings(Assembly.GetExecutingAssembly()))
               Debug.WriteLine("IsInterned: " + (null != String.IsInterned(userString)).ToString() + @"', UserStrings: '" + userString + @"'");
            Debug.WriteLine(@"Stop ->======================================================================================");
        }

        [TestMethod]
        public void SimpleRemoteParametersConcat0To1000LengthStringsTest() {
            Debug.WriteLine(@"Start ->======================================================================================");
            foreach (var userString in UserStrings.GetUserStrings(Assembly.GetExecutingAssembly()))
               Debug.WriteLine("IsInterned: " + (null != String.IsInterned(userString)).ToString() + @"', UserStrings: '" + userString + @"'");
            Debug.WriteLine(@"Start ->======================================================================================");

            Parallel.ForEach(Enumerable.Range(0, 1000).ToArray(), count => {
                var randomPhraseOne = Generator.MakeRandomString(count);
                var randomPhraseTwo = Generator.MakeRandomString(count + 1);
                using (var testPhraseOne = (ApplicationSecureString)randomPhraseOne)
                using (var testPhraseTwo = (ApplicationSecureString)randomPhraseTwo.ToCharArray())
                using (var testPhraseThree = RemotingService.SimpleParametersConcat(testPhraseOne, testPhraseTwo)) {
                    testPhraseThree.CreateUnsecuredString().Should().NotBeEmpty();
                    testPhraseThree.CreateUnsecuredString().Should().Be(randomPhraseOne + randomPhraseTwo);
                }
            });

            Debug.WriteLine(@"Stop ->======================================================================================");
            foreach (var userString in UserStrings.GetUserStrings(Assembly.GetExecutingAssembly()))
               Debug.WriteLine("IsInterned: " + (null != String.IsInterned(userString)).ToString() + @"', UserStrings: '" + userString + @"'");
            Debug.WriteLine(@"Stop ->======================================================================================");
        }

        [TestMethod]
        public void ComplexRemoteParametersConcatTest() {
            Debug.WriteLine(@"Start ->======================================================================================");
            foreach (var userString in UserStrings.GetUserStrings(Assembly.GetExecutingAssembly()))
               Debug.WriteLine("IsInterned: " + (null != String.IsInterned(userString)).ToString() + @"', UserStrings: '" + userString + @"'");
            Debug.WriteLine(@"Start ->======================================================================================");

            var randomPhrase = Generator.MakeRandomString(78);
            using (var testPhraseOne = (ApplicationSecureString)randomPhrase)
            using (var testPhraseTwo = (ApplicationSecureString)(randomPhrase.ToCharArray())) {
                var dataCarrier = new DataCarrier() {
                    ParameterA = testPhraseOne,
                    ParameterB = testPhraseTwo
                };

                var dataCarrierResult = RemotingService.ComplexParametersConcat(dataCarrier);
                dataCarrierResult.Result.CreateUnsecuredString().Should().NotBeEmpty();
                dataCarrierResult.Result.CreateUnsecuredString().Should().Be(dataCarrierResult.ParameterA.CreateUnsecuredString() + dataCarrierResult.ParameterB.CreateUnsecuredString());
            }

            Debug.WriteLine(@"Stop ->======================================================================================");
            foreach (var userString in UserStrings.GetUserStrings(Assembly.GetExecutingAssembly()))
               Debug.WriteLine("IsInterned: " + (null != String.IsInterned(userString)).ToString() + @"', UserStrings: '" + userString + @"'");
            Debug.WriteLine(@"Stop ->======================================================================================");
        }

        [TestMethod]
        public void ComplexRemoteParametersConcatForEmptyStringsTest() {
            Debug.WriteLine(@"Start ->======================================================================================");
            foreach (var userString in UserStrings.GetUserStrings(Assembly.GetExecutingAssembly()))
               Debug.WriteLine("IsInterned: " + (null != String.IsInterned(userString)).ToString() + @"', UserStrings: '" + userString + @"'");
            Debug.WriteLine(@"Start ->======================================================================================");

            using (var testPhraseOne = (ApplicationSecureString)String.Empty)
            using (var testPhraseTwo = (ApplicationSecureString)(String.Empty.ToCharArray())) {
                var dataCarrier = new DataCarrier() {
                    ParameterA = testPhraseOne,
                    ParameterB = testPhraseTwo
                };

                var dataCarrierResult = RemotingService.ComplexParametersConcat(dataCarrier);
                dataCarrierResult.Result.CreateUnsecuredString().Should().Be(String.Empty);
            }

            Debug.WriteLine(@"Stop ->======================================================================================");
            foreach (var userString in UserStrings.GetUserStrings(Assembly.GetExecutingAssembly()))
               Debug.WriteLine("IsInterned: " + (null != String.IsInterned(userString)).ToString() + @"', UserStrings: '" + userString + @"'");
            Debug.WriteLine(@"Stop ->======================================================================================");
        }

        [TestMethod]
        public void ComplexRemoteParametersConcatFor0To1000LengthStringsTest() {
            Debug.WriteLine(@"Start ->======================================================================================");
            foreach (var userString in UserStrings.GetUserStrings(Assembly.GetExecutingAssembly()))
               Debug.WriteLine("IsInterned: " + (null != String.IsInterned(userString)).ToString() + @"', UserStrings: '" + userString + @"'");
            Debug.WriteLine(@"Start ->======================================================================================");

            Parallel.ForEach(Enumerable.Range(0, 1000).ToArray(), count => {
                var randomPhraseOne = Generator.MakeRandomString(count + 1);
                var randomPhraseTwo = Generator.MakeRandomString(count);
                using (var testPhraseOne = (ApplicationSecureString)randomPhraseOne)
                using (var testPhraseTwo = (ApplicationSecureString)randomPhraseTwo.ToCharArray()) {
                    var dataCarrier = new DataCarrier() {
                        ParameterA = testPhraseOne,
                        ParameterB = testPhraseTwo
                    };

                    var dataCarrierResult = RemotingService.ComplexParametersConcat(dataCarrier);
                    dataCarrierResult.Result.CreateUnsecuredString().Should().NotBeEmpty();
                    dataCarrierResult.Result.CreateUnsecuredString().Should().Be(randomPhraseOne + randomPhraseTwo);
                }
            });

            Debug.WriteLine(@"Stop ->======================================================================================");
            foreach (var userString in UserStrings.GetUserStrings(Assembly.GetExecutingAssembly()))
               Debug.WriteLine("IsInterned: " + (null != String.IsInterned(userString)).ToString() + @"', UserStrings: '" + userString + @"'");
            Debug.WriteLine(@"Stop ->======================================================================================");
        }
    }
}
