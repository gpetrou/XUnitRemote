
namespace XUnitRemote
{
    using System.Collections.Generic;
    using System.Linq;

    using Xunit.Abstractions;
    using Xunit.Sdk;

    public class RemoteHostFactDiscoverer : IXunitTestCaseDiscoverer
    {
        private readonly FactDiscoverer _factDiscoverer;

        public IEnumerable<IXunitTestCase> Discover(ITestFrameworkDiscoveryOptions discoveryOptions, ITestMethod testMethod, IAttributeInfo factAttribute)
        {
            return _factDiscoverer.Discover(discoveryOptions, testMethod, factAttribute)
                .Select(testCase => new RemoteHostTestCase(testCase));
        }

        public RemoteHostFactDiscoverer(IMessageSink diagnosticMessageSink)
        {
            _factDiscoverer = new FactDiscoverer(diagnosticMessageSink);
        }
    }
}
