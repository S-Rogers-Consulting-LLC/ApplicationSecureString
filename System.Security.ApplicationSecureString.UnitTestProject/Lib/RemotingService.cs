using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Security;

namespace UnitTestProject {
    public static class RemotingService {
        private const string ChannelName = @"IPChannelName.988763450236506";
        private const string ObjectName = @"RemoteObj.39845097";

        static RemotingService() {
            ChannelServices.RegisterChannel(new IpcChannel(ChannelName), false);
            RemotingConfiguration.RegisterWellKnownServiceType(typeof(RemoteServer), ObjectName, WellKnownObjectMode.SingleCall);
        }

        public static ApplicationSecureString SimpleParametersConcat(ApplicationSecureString argApplicationSecureStringA, ApplicationSecureString argApplicationSecureStringB) {
            return RemotingClient.SimpleParametersConcat(argApplicationSecureStringA, argApplicationSecureStringB);
        }

        public static DataCarrier ComplexParametersConcat(DataCarrier argRemoteableCarrier) {
            return RemotingClient.ComplexParametersConcat(argRemoteableCarrier);
        }

        private interface IRemoteServerInterface {
            ApplicationSecureString SimpleParametersConcat(ApplicationSecureString argApplicationSecureStringA, ApplicationSecureString argApplicationSecureStringB);
            DataCarrier ComplexParametersConcat(DataCarrier argRemoteableCarrier);
        }

        private class RemoteServer : MarshalByRefObject, IRemoteServerInterface {
            public RemoteServer() { }

            public ApplicationSecureString SimpleParametersConcat(ApplicationSecureString argApplicationSecureStringA, ApplicationSecureString argApplicationSecureStringB) {
                return argApplicationSecureStringA + argApplicationSecureStringB;
            }

            public DataCarrier ComplexParametersConcat(DataCarrier argRemoteableCarrier) {
                argRemoteableCarrier.Result = argRemoteableCarrier.ParameterA + argRemoteableCarrier.ParameterB;
                return argRemoteableCarrier;
            }
        }

        private class RemotingClient {
            private static IRemoteServerInterface TheRemoteServerInterface = null;

            static RemotingClient() {
                TheRemoteServerInterface = (IRemoteServerInterface)Activator.GetObject(typeof(IRemoteServerInterface), @"ipc://" + ChannelName + @"/" + ObjectName + @"");
            }

            public static ApplicationSecureString SimpleParametersConcat(ApplicationSecureString argApplicationSecureStringA, ApplicationSecureString argApplicationSecureStringB) {
                return TheRemoteServerInterface.SimpleParametersConcat(argApplicationSecureStringA, argApplicationSecureStringB);
            }

            public static DataCarrier ComplexParametersConcat(DataCarrier argRemoteableCarrier) {
                return TheRemoteServerInterface.ComplexParametersConcat(argRemoteableCarrier);
            }
        }
    }
}
