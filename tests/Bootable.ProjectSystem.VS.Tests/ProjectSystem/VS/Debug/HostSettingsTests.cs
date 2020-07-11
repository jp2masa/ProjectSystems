using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.ProjectSystem.Debug;

using Newtonsoft.Json.Linq;

using NUnit.Framework;

using Bootable.Launch.Hosts.Bochs;

namespace Bootable.ProjectSystem.VS.Debug
{
    [TestFixture]
    public class HostSettingsTests
    {
        private const string HostSettings1 =
@"{
  ""TestProfile1"": {
    ""host"": ""Bochs"",
    ""hostSettings"": {
      ""bochsDirectory"": ""some\\path\\to\\bochs\\"",
      ""ConfigurationFile"": ""some\\path\\to\\bochs.bxrc"",
      ""displayLibraryOptions.guiDebug"": true
    }
  }
}";

        [Test]
        public void TestHostSettingsDeserialization()
        {
            var launchProfileDataType = typeof(ILaunchProfile).Assembly.GetTypes().Where(
                t => t.Name == "LaunchProfileData").Single();

            var deserializeProfilesMethod = launchProfileDataType.GetMethod(
                "DeserializeProfiles", new Type[] { typeof(JObject) });

            var profilesObject = deserializeProfilesMethod.Invoke(
                null, new object[] { JObject.Parse(HostSettings1) });

            var profiles = profilesObject.GetType().GetProperty("Values").GetValue(profilesObject) as IEnumerable;

            foreach(var profile in profiles)
            {
                var otherSettings = profile.GetType().GetProperty("OtherSettings").GetValue(profile) as Dictionary<string, object>;

                Assert.That(otherSettings, Is.Not.Null);
                Assert.That(otherSettings, Contains.Key("hostSettings"));

                var bochsHostSettings = new BochsHostSettings(otherSettings["hostSettings"] as Dictionary<string, string>);

                Assert.That(bochsHostSettings.BochsDirectory, Is.EqualTo(@"some\path\to\bochs\"));
                Assert.That(bochsHostSettings.ConfigurationFile, Is.EqualTo(@"some\path\to\bochs.bxrc"));
                Assert.That(bochsHostSettings.DisplayLibraryOptions, Is.Not.Null);
                Assert.That(bochsHostSettings.DisplayLibraryOptions.GuiDebug, Is.True);
                Assert.That(bochsHostSettings.DisplayLibraryOptions.HideIps, Is.False);

                break;
            }
        }
    }
}
