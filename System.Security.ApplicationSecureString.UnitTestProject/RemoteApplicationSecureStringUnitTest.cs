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
            var randomPhrase = Generator.MakeRandomString(51);
            using (var testPhraseOne = (ApplicationSecureString)randomPhrase)
            using (var testPhraseTwo = (ApplicationSecureString)(randomPhrase.ToCharArray()))
            using (var testPhraseThree = RemotingService.SimpleParametersConcat(testPhraseOne, testPhraseTwo)) {
                testPhraseThree.CreateUnsecuredString().Should().NotBeEmpty();
                testPhraseThree.CreateUnsecuredString().Should().Be(testPhraseOne + testPhraseTwo);
            }
        }

        [TestMethod]
        public void SimpleRemoteParametersConcatForEmptyStringsTest() {
            using (var testPhraseOne = (ApplicationSecureString)String.Empty)
            using (var testPhraseTwo = (ApplicationSecureString)(String.Empty.ToCharArray()))
            using (var testPhraseThree = RemotingService.SimpleParametersConcat(testPhraseOne, testPhraseTwo)) {
                testPhraseThree.CreateUnsecuredString().Should().Be(String.Empty);
            }
        }

        [TestMethod]
        public void SimpleRemoteParametersConcat0To1000LengthStringsTest() {
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
        }

        [TestMethod]
        public void ComplexRemoteParametersConcatTest() {
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
        }

        [TestMethod]
        public void ComplexRemoteParametersConcatForEmptyStringsTest() {
            using (var testPhraseOne = (ApplicationSecureString)String.Empty)
            using (var testPhraseTwo = (ApplicationSecureString)(String.Empty.ToCharArray())) {
                var dataCarrier = new DataCarrier() {
                    ParameterA = testPhraseOne,
                    ParameterB = testPhraseTwo
                };

                var dataCarrierResult = RemotingService.ComplexParametersConcat(dataCarrier);
                dataCarrierResult.Result.CreateUnsecuredString().Should().Be(String.Empty);
            }
        }

        [TestMethod]
        public void ComplexRemoteParametersConcatFor0To1000LengthStringsTest() {
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
        }
    }
}
