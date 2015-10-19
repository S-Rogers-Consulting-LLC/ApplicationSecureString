using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject {
    [TestClass]
    public class UserStringsUnitTest {
        [TestMethod]
        public void ListAllUserStrings() {
            Debug.WriteLine(@"Start ->======================================================================================");
            foreach (var assembly in UserStrings.GetAssemblies()) {
                Debug.WriteLine("AssemblyName: " + assembly.GetName());
                foreach(var assemblyString in UserStrings.GetUserNonInternedStrings(assembly))
                    Debug.WriteLine("  NonInternedString: " + assemblyString);
                foreach (var assemblyString in UserStrings.GetUserInternedStrings(assembly))
                    Debug.WriteLine("  InternedString: " + assemblyString);
                Debug.WriteLine(@"Break ->======================================================================================");
            }
            Debug.WriteLine(@"Stop  ->======================================================================================");
        }
    }
}
