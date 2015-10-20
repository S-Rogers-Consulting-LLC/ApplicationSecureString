using System;
using System.Diagnostics;
using System.Reflection;
using System.Security;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject {
    [TestClass]
    public class LocalApplicationSecureStringUnitTest {
        [TestMethod]
        public void ImplicitCastStringTest() {
            var testPhrase = Generator.MakeRandomString(22);
            var applicationSecureString = (ApplicationSecureString)testPhrase;
            var testChar = (Char[])applicationSecureString;
            testChar.Should().NotBeEmpty();
            var testString = (String)applicationSecureString;
            testString.Should().NotBeEmpty();
            applicationSecureString.CreateUnsecuredString().Should().Be(testPhrase);
        }

        [TestMethod]
        public void ImplicitCastCharArrayTest() {
            var testStatement = Generator.MakeRandomString(3);
            var testPhrase = testStatement.ToCharArray();
            var applicationSecureString = (ApplicationSecureString)testPhrase;
            var testChar = (Char[])applicationSecureString;
            testChar.Should().NotBeEmpty();
            var testString = (String)applicationSecureString;
            testString.Should().NotBeEmpty();
            testString.Should().Be(testStatement);
        }

        [TestMethod]
        public void CompareTest() {
            var randomPhrase = Generator.MakeRandomString(66);
            using (var testPhraseOne = (ApplicationSecureString)randomPhrase)
            using (var testPhraseTwo = (ApplicationSecureString)(randomPhrase.ToCharArray())) {
                testPhraseOne.CreateUnsecuredString().Should().NotBeEmpty();
                testPhraseOne.CreateUnsecuredString().Should().Be(testPhraseTwo.CreateUnsecuredString());
            }
        }

        [TestMethod]
        public void EqualityTest() {
            var randomPhrase = Generator.MakeRandomString(66);
            using (var testPhraseOne = (ApplicationSecureString)randomPhrase)
            using (var testPhraseTwo = (ApplicationSecureString)(randomPhrase.ToCharArray())) {
                (testPhraseOne == testPhraseTwo).Should().BeTrue();
                (testPhraseOne != testPhraseTwo).Should().BeFalse();
            }
        }

        [TestMethod]
        public void ConcatTest() {
            var randomPhraseOne = Generator.MakeRandomString(45);
            var randomPhraseTwo = Generator.MakeRandomString(222);
            using (var testPhraseOne = (ApplicationSecureString)randomPhraseOne)
            using (var testPhraseTwo = (ApplicationSecureString)(randomPhraseTwo.ToCharArray())) {
                var dataCarrier = new DataCarrier() {
                    ParameterA = testPhraseOne,
                    ParameterB = testPhraseTwo
                };

                dataCarrier.Result = dataCarrier.ParameterA + dataCarrier.ParameterB;
                dataCarrier.Result.CreateUnsecuredString().Should().NotBeEmpty();
                dataCarrier.Result.CreateUnsecuredString().Should().Be(randomPhraseOne + randomPhraseTwo);
            }
        }
    }
}
