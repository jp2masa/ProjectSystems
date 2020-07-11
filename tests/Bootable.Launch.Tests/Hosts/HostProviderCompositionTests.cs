using System.Linq;

using NUnit.Framework;

namespace Bootable.Launch.Hosts
{
    [TestFixture]
    public class HostProviderCompositionTests
    {
        [Test]
        public void TestMethod1()
        {
            var hostProviderCompositionHost = new CompositionHost();
            var hostProviders = hostProviderCompositionHost.HostProviders;

            Assert.That(hostProviders.Any(p => p.Name == "Bochs"));
        }
    }
}
