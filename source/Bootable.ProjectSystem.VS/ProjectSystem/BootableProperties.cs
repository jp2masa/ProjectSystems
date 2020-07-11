using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualStudio.ProjectSystem;

namespace Bootable.ProjectSystem
{
    [Export(typeof(IBootableProperties))]
    [AppliesTo(ProjectCapability.Bootable)]
    internal class BootableProperties : IBootableProperties
    {
        private UnconfiguredProject _unconfiguredProject;
        private ProjectProperties _projectProperties;

        private string _projectDirectory;

        [ImportingConstructor]
        public BootableProperties(UnconfiguredProject unconfiguredProject, ProjectProperties projectProperties)
        {
            _unconfiguredProject = unconfiguredProject;
            _projectProperties = projectProperties;

            _projectDirectory = Path.GetDirectoryName(_unconfiguredProject.FullPath);
        }

        public async Task<string> GetBinFileFullPathAsync()
        {
            var bootableProperties = await _projectProperties.GetBootableConfigurationPropertiesAsync().ConfigureAwait(false);
            var binPath = await bootableProperties.BinFile.GetEvaluatedValueAtEndAsync().ConfigureAwait(false);

            return String.IsNullOrWhiteSpace(binPath) ? _projectDirectory : _unconfiguredProject.MakeRooted(binPath);
        }

        public async Task<string> GetIsoFileFullPathAsync()
        {
            var bootableProperties = await _projectProperties.GetBootableConfigurationPropertiesAsync().ConfigureAwait(false);
            var isoPath = await bootableProperties.IsoFile.GetEvaluatedValueAtEndAsync().ConfigureAwait(false);

            return String.IsNullOrWhiteSpace(isoPath) ? _projectDirectory : _unconfiguredProject.MakeRooted(isoPath);
        }
    }
}
