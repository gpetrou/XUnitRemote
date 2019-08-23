
namespace XUnitRemoteTests
{
    using System.Diagnostics;

    using Xunit;

    using XUnitRemote;

    public class RemoteHostFactAttributeTests
    {
        [RemoteHostFact]
        public void RemoteHostFactAttributeTest()
        {
            Assert.Equal("XUnitRemoteHost", Process.GetCurrentProcess().ProcessName);
        }
    }
}
