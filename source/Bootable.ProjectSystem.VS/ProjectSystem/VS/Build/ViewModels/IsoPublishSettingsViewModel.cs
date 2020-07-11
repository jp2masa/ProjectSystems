using System;
using System.Windows.Input;
using Microsoft.Win32;

namespace Bootable.ProjectSystem.VS.Build
{
    internal class IsoPublishSettingsViewModel : PublishSettingsViewModelBase
    {
        public string PublishPath
        {
            get => _publishPath;
            set => SetAndRaiseIfChanged(ref _publishPath, value);
        }

        public ICommand BrowsePublishPath { get; }

        private string _publishPath;

        public IsoPublishSettingsViewModel()
        {
            BrowsePublishPath = new BrowsePublishPathCommand(this);
        }

        private class BrowsePublishPathCommand : ICommand
        {
            private IsoPublishSettingsViewModel _viewModel;

            public BrowsePublishPathCommand(IsoPublishSettingsViewModel viewModel)
            {
                _viewModel = viewModel;
            }

#pragma warning disable CS0067
            public event EventHandler CanExecuteChanged;
#pragma warning restore CS0067

            public bool CanExecute(object parameter) => true;

            public void Execute(object parameter)
            {
                var saveFileDialog = new SaveFileDialog
                {
                    FileName = _viewModel.PublishPath,
                    Filter = "ISO Image (*.iso) | *.iso",
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    _viewModel.PublishPath = saveFileDialog.FileName;
                }
            }
        }
    }
}
