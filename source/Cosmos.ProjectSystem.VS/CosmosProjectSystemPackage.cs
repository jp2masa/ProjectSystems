using System;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Task = System.Threading.Tasks.Task;

using Cosmos.ProjectSystem.VS.PropertyPages;

namespace Cosmos.ProjectSystem.VS
{
    [Guid(PackageGuid)]
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [ProvideObject(typeof(CosmosPropertyPage))]
    [ProvideProjectFactory(typeof(MigrateCosmosProjectFactory), null, "Cosmos Project Files (*.Cosmos);*.Cosmos", "Cosmos", "Cosmos", null)]
    internal sealed class CosmosProjectSystemPackage : AsyncPackage
    {
        public const string PackageGuid = "29194faf-90ce-454b-bc53-08b722f1dadf";

        private IVsProjectFactory _factory;

        protected async override Task InitializeAsync(
            CancellationToken cancellationToken,
            IProgress<ServiceProgressData> progress)
        {
            await base.InitializeAsync(cancellationToken, progress).ConfigureAwait(true);

            await JoinableTaskFactory.SwitchToMainThreadAsync();

            _factory = new MigrateCosmosProjectFactory();
            RegisterProjectFactory(_factory);
        }
    }
}
