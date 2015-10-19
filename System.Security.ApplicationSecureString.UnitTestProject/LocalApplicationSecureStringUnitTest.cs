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
            Debug.WriteLine(@"Start ->======================================================================================");
            foreach (var userString in UserStrings.GetUserStrings(Assembly.GetExecutingAssembly()))
               Debug.WriteLine("IsInterned: " + (null != String.IsInterned(userString)).ToString() + @"', UserStrings: '" + userString + @"'");
            Debug.WriteLine(@"Start ->======================================================================================");

            var testPhrase = Generator.MakeRandomString(22);
            var applicationSecureString = (ApplicationSecureString)testPhrase;
            var testChar = (Char[])applicationSecureString;
            testChar.Should().NotBeEmpty();
            var testString = (String)applicationSecureString;
            testString.Should().NotBeEmpty();
            applicationSecureString.CreateUnsecuredString().Should().Be(testPhrase);

            Debug.WriteLine(@"Stop ->======================================================================================");
            foreach (var userString in UserStrings.GetUserStrings(Assembly.GetExecutingAssembly()))
               Debug.WriteLine("IsInterned: " + (null != String.IsInterned(userString)).ToString() + @"', UserStrings: '" + userString + @"'");
            Debug.WriteLine(@"Stop ->======================================================================================");
        }

        [TestMethod]
        public void ImplicitCastCharArrayTest() {
            Debug.WriteLine(@"Start ->======================================================================================");
            foreach (var userString in UserStrings.GetUserStrings(Assembly.GetExecutingAssembly()))
               Debug.WriteLine("IsInterned: " + (null != String.IsInterned(userString)).ToString() + @"', UserStrings: '" + userString + @"'");
            Debug.WriteLine(@"Start ->======================================================================================");

            var testStatement = Generator.MakeRandomString(3);
            var testPhrase = testStatement.ToCharArray();
            var applicationSecureString = (ApplicationSecureString)testPhrase;
            var testChar = (Char[])applicationSecureString;
            testChar.Should().NotBeEmpty();
            var testString = (String)applicationSecureString;
            testString.Should().NotBeEmpty();
            testString.Should().Be(testStatement);

            Debug.WriteLine(@"Stop ->======================================================================================");
            foreach (var userString in UserStrings.GetUserStrings(Assembly.GetExecutingAssembly()))
               Debug.WriteLine("IsInterned: " + (null != String.IsInterned(userString)).ToString() + @"', UserStrings: '" + userString + @"'");
            Debug.WriteLine(@"Stop ->======================================================================================");
        }

        [TestMethod]
        public void CompareTest() {
            Debug.WriteLine(@"Start ->======================================================================================");
            foreach (var userString in UserStrings.GetUserStrings(Assembly.GetExecutingAssembly()))
               Debug.WriteLine("IsInterned: " + (null != String.IsInterned(userString)).ToString() + @"', UserStrings: '" + userString + @"'");
            Debug.WriteLine(@"Start ->======================================================================================");

            var randomPhrase = Generator.MakeRandomString(66);
            using (var testPhraseOne = (ApplicationSecureString)randomPhrase)
            using (var testPhraseTwo = (ApplicationSecureString)(randomPhrase.ToCharArray())) {
                testPhraseOne.CreateUnsecuredString().Should().NotBeEmpty();
                testPhraseOne.CreateUnsecuredString().Should().Be(testPhraseTwo.CreateUnsecuredString());
            }

            Debug.WriteLine(@"Stop ->======================================================================================");
            foreach (var userString in UserStrings.GetUserStrings(Assembly.GetExecutingAssembly()))
               Debug.WriteLine("IsInterned: " + (null != String.IsInterned(userString)).ToString() + @"', UserStrings: '" + userString + @"'");
            Debug.WriteLine(@"Stop ->======================================================================================");
        }

        [TestMethod]
        public void EqualityTest() {
            Debug.WriteLine(@"Start ->======================================================================================");
            foreach (var userString in UserStrings.GetUserStrings(Assembly.GetExecutingAssembly()))
               Debug.WriteLine("IsInterned: " + (null != String.IsInterned(userString)).ToString() + @"', UserStrings: '" + userString + @"'");
            Debug.WriteLine(@"Start ->======================================================================================");

            var randomPhrase = Generator.MakeRandomString(66);
            using (var testPhraseOne = (ApplicationSecureString)randomPhrase)
            using (var testPhraseTwo = (ApplicationSecureString)(randomPhrase.ToCharArray())) {
                (testPhraseOne == testPhraseTwo).Should().BeTrue();
                (testPhraseOne != testPhraseTwo).Should().BeFalse();
            }

            Debug.WriteLine(@"Stop ->======================================================================================");
            foreach (var userString in UserStrings.GetUserStrings(Assembly.GetExecutingAssembly()))
               Debug.WriteLine("IsInterned: " + (null != String.IsInterned(userString)).ToString() + @"', UserStrings: '" + userString + @"'");
            Debug.WriteLine(@"Stop ->======================================================================================");
        }

        [TestMethod]
        public void ConcatTest() {
            Debug.WriteLine(@"Start ->======================================================================================");
            foreach (var userString in UserStrings.GetUserStrings(Assembly.GetExecutingAssembly()))
               Debug.WriteLine("IsInterned: " + (null != String.IsInterned(userString)).ToString() + @"', UserStrings: '" + userString + @"'");
            Debug.WriteLine(@"Start ->======================================================================================");

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

            Debug.WriteLine(@"Stop ->======================================================================================");
            foreach (var userString in UserStrings.GetUserStrings(Assembly.GetExecutingAssembly()))
               Debug.WriteLine("IsInterned: " + (null != String.IsInterned(userString)).ToString() + @"', UserStrings: '" + userString + @"'");
            Debug.WriteLine(@"Stop ->======================================================================================");
        }
    }
}
