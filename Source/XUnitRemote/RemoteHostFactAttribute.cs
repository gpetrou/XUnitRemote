
namespace XUnitRemote
{
    using System;

    using Xunit;
    using Xunit.Sdk;

    [AttributeUsage(AttributeTargets.Method)]
    [XunitTestCaseDiscoverer("XUnitRemote.RemoteHostFactDiscoverer", "XUnitRemote")]
    public class RemoteHostFactAttribute : FactAttribute
    {
    }
}
