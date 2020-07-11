using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using NUnit.Framework;

namespace Bootable.Launch.Hosts.Bochs
{
    [TestFixture(TestOf = typeof(BochsHostSettings))]
    public class BochsHostSettingsTests
    {
        private const string HostSettings1 =
@"{
  ""bochsDirectory"": ""some\\path\\to\\bochs\\"",
  ""ConfigurationFile"": ""some\\path\\to\\bochs.bxrc"",
  ""displayLibraryOptions.guiDebug"": true
}";

        [Test]
        public void TestHostSettingsDeserialization()
        {
            var hostSettings = JObject.Parse(HostSettings1).ToObject<Dictionary<string, string>>();
            var bochsHostSettings = new BochsHostSettings(hostSettings);

            Assert.That(bochsHostSettings.BochsDirectory, Is.EqualTo(@"some\path\to\bochs\"));
            Assert.That(bochsHostSettings.ConfigurationFile, Is.EqualTo(@"some\path\to\bochs.bxrc"));
            Assert.That(bochsHostSettings.DisplayLibraryOptions, Is.Not.Null);
            Assert.That(bochsHostSettings.DisplayLibraryOptions.GuiDebug, Is.True);
            Assert.That(bochsHostSettings.DisplayLibraryOptions.HideIps, Is.False);
        }
    }
}
