using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.ProjectSystem;
using Microsoft.VisualStudio.ProjectSystem.Build;
using Microsoft.VisualStudio.Threading;
using Task = System.Threading.Tasks.Task;

namespace Bootable.ProjectSystem.VS.Build
{
    //[Export(typeof(IPublishProvider))]
    //[AppliesTo(ProjectCapability.Bootable)]
    internal class PublishProvider : IPublishProvider
    {
        private PublishWindow _publishWindow;
        private PublishWindowViewModel _publishWindowViewModel;

        // this has to be exported to the configured project scope
        //[ImportingConstructor]
        public PublishProvider(ConfiguredProject configuredProject)
        {
            _publishWindowViewModel = new PublishWindowViewModel(configuredProject);
            _publishWindow = new PublishWindow(_publishWindowViewModel);
        }

        public Task<bool> IsPublishSupportedAsync() => TplExtensions.TrueTask;

        public Task PublishAsync(CancellationToken cancellationToken, TextWriter outputPaneWriter) =>
            _publishWindowViewModel.ActiveProvider.PublishAsync(outputPaneWriter, cancellationToken);

        public Task<bool> ShowPublishPromptAsync() => Task.FromResult(_publishWindow.ShowModal() ?? false);
    }
}
