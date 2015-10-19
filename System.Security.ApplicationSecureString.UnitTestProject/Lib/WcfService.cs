using System;
using System.Security;
using System.ServiceModel;

namespace UnitTestProject {
    public static class WcfService {
        private const string ChannelName = @"WcfService.9887636506";
        private const string ObjectName = @"PipeEndpoint.39842297";

        static WcfService() {
            var serviceHost = new ServiceHost(new WcfServer(), new Uri("net.pipe://localhost/" + ChannelName + ""));
            serviceHost.AddServiceEndpoint(typeof(IWcfServer), new NetNamedPipeBinding(), ObjectName);
            serviceHost.Open();
        }

        public static ApplicationSecureString SimpleParametersConcat(ApplicationSecureString argApplicationSecureStringA, ApplicationSecureString argApplicationSecureStringB) {
            return WcfClient.Concat(argApplicationSecureStringA, argApplicationSecureStringB);
        }

        public static DataCarrier ComplexParametersConcat(DataCarrier argRemoteableCarrier) {
            return WcfClient.ComplexParametersConcat(argRemoteableCarrier);
        }

        private static class WcfClient {
            private static IWcfServer TheWcfServer = null;

            static WcfClient() {
                TheWcfServer = ChannelFactory<IWcfServer>.CreateChannel(new NetNamedPipeBinding(), new EndpointAddress("net.pipe://localhost/" + ChannelName + "/" + ObjectName + "/"));
            }

            public static ApplicationSecureString Concat(ApplicationSecureString argApplicationSecureStringA, ApplicationSecureString argApplicationSecureStringB) {
                var result = TheWcfServer.SimpleParametersConcat(argApplicationSecureStringA, argApplicationSecureStringB);
                return result;
            }

            public static DataCarrier ComplexParametersConcat(DataCarrier argRemoteableCarrier) {
                var result = TheWcfServer.ComplexParametersConcat(argRemoteableCarrier);
                return result;
            }
        }

        [ServiceContract]
        private interface IWcfServer {
            [OperationContract]
            ApplicationSecureString SimpleParametersConcat(ApplicationSecureString argApplicationSecureStringA, ApplicationSecureString argApplicationSecureStringB);
            [OperationContract]
            DataCarrier ComplexParametersConcat(DataCarrier argRemoteableCarrier);
        }

        [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
        private class WcfServer : IWcfServer {
            public ApplicationSecureString SimpleParametersConcat(ApplicationSecureString argApplicationSecureStringA, ApplicationSecureString argApplicationSecureStringB) {
                return argApplicationSecureStringA + argApplicationSecureStringB;
            }

            public DataCarrier ComplexParametersConcat(DataCarrier argRemoteableCarrier) {
                argRemoteableCarrier.Result = argRemoteableCarrier.ParameterA + argRemoteableCarrier.ParameterB;
                return argRemoteableCarrier;
            }
        }
    }
}
