using Microsoft.VisualStudio.PlatformUI;

namespace Bootable.ProjectSystem.VS.Build
{
    /// <summary>
    /// Interaction logic for PublishWindow.xaml
    /// </summary>
    internal partial class PublishWindow : DialogWindow
    {
        public PublishWindow(PublishWindowViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
