using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using Microsoft.VisualStudio.Imaging.Interop;

namespace Bootable.ProjectSystem.VS.Build
{
    public interface IBootablePublishProvider
    {
        string Name { get; }
        ImageMoniker Icon { get; }
        UserControl SettingsControl { get; }

        Task<bool> CanPublishAsync(CancellationToken cancellationToken);
        Task PublishAsync(TextWriter outputPaneWriter, CancellationToken cancellationToken);
    }
}
