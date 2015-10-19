using System;
using System.Runtime.Serialization;
using System.Security;
using ProtoBuf;

namespace UnitTestProject {
    [Serializable]
    [DataContract]
    [ProtoContract]
    public class DataCarrier {
        public DataCarrier() {
            ParameterA = String.Empty;
            ParameterB = String.Empty;
            Result = String.Empty;
        }

        [DataMember]
        [ProtoMember(1)]
        public ApplicationSecureString ParameterA { get; set; }

        [DataMember]
        [ProtoMember(2)]
        public ApplicationSecureString ParameterB { get; set; }

        [DataMember]
        [ProtoMember(3)]
        public ApplicationSecureString Result { get; set; }
    }
}
