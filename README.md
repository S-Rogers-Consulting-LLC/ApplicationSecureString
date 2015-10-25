# ApplicationSecureString
The 'ApplicationSecureString' is a direct replacement for the C# 'System.String'. The 'ApplicationSecureString' implements a wrapped 'SecureString' in order to eliminate or reduce the surface area of a Memory Scraping attack. ApplicationSecureString is a direct replacement for 'System.String' for all Results, Members, Properties, and as a Parameters. In addition the 'ApplicationSecureString' can be used in transport technologies like WCF, Remoting and ProtoBuf.

# Why ApplicationSecureString
Though data encryption is widely used to secure data, memory scraping finds weak areas from which it can take data. For example, some memory-scraping malware steals encrypted data from applications through which the data passed unencrypted.

# Examples from Unit Tests
```C#
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
```

```C#
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
```

```C#
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
```
