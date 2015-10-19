using System;
using System.IO;
using System.Security;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProtoBuf;
using ProtoBuf.Meta;

namespace UnitTestProject {
    [TestClass]
    public class ProtobufUnitTest {
        #region ApplicationSecureStringSurrogate Code
        static ProtobufUnitTest() {
            RuntimeTypeModel.Default
                                   .Add(typeof(ApplicationSecureString), false)
                                        .SetSurrogate(typeof(ApplicationSecureStringSurrogate));
        }

        /// <summary>
        /// ProtoBuf Surrogate permit you to wrap code you do not have the source too.
        /// </summary>
        [ProtoContract]
        public class ApplicationSecureStringSurrogate {
            [ProtoMember(1)]
            public String SecureContext { get; set; }

            public static implicit operator ApplicationSecureStringSurrogate(ApplicationSecureString argApplicationSecureString) {
                return new ApplicationSecureStringSurrogate() { SecureContext = argApplicationSecureString.SecureContext };
            }

            public static implicit operator ApplicationSecureString(ApplicationSecureStringSurrogate argApplicationSecureStringSurrogate) {
                return new ApplicationSecureString() { SecureContext = argApplicationSecureStringSurrogate.SecureContext };
            }
        }
        #endregion

        [TestMethod]
        public void SerializeAndDeserializeDataCarrierTest() {

            var dataCarrier = new DataCarrier() {
                ParameterA = "ABC",
                ParameterB = "DEF",
                Result = "GHI"
            };

            dataCarrier.ParameterA.CreateUnsecuredString().Should().Be("ABC");
            dataCarrier.ParameterB.CreateUnsecuredString().Should().Be("DEF");
            dataCarrier.Result.CreateUnsecuredString().Should().Be("GHI");

            using (var memoryStream01 = new MemoryStream()) {
                Serializer.Serialize<DataCarrier>(memoryStream01, dataCarrier);
                memoryStream01.Seek(0, SeekOrigin.Begin);
                var newDataCarrier = Serializer.Deserialize<DataCarrier>(memoryStream01);

                newDataCarrier.ParameterA.CreateUnsecuredString().Should().Be("ABC");
                newDataCarrier.ParameterB.CreateUnsecuredString().Should().Be("DEF");
                newDataCarrier.Result.CreateUnsecuredString().Should().Be("GHI");

                newDataCarrier.Result = newDataCarrier.ParameterA + newDataCarrier.ParameterB;

                using (var memoryStream02 = new MemoryStream()) {
                    Serializer.Serialize<DataCarrier>(memoryStream02, newDataCarrier);
                    memoryStream02.Seek(0, SeekOrigin.Begin);
                    var resultDataCarrier = Serializer.Deserialize<DataCarrier>(memoryStream02);

                    resultDataCarrier.ParameterA.CreateUnsecuredString().Should().Be("ABC");
                    resultDataCarrier.ParameterB.CreateUnsecuredString().Should().Be("DEF");
                    resultDataCarrier.Result.CreateUnsecuredString().Should().Be("ABCDEF");
                }
            }

        }


    }
}
