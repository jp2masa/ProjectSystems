using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Microsoft.VisualStudio.ProjectSystem;

namespace Bootable.ProjectSystem.VS.Build
{
    internal class PublishWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public IEnumerable<IBootablePublishProvider> PublishProviders { get; }

        public IBootablePublishProvider ActiveProvider
        {
            get => _activeProvider;
            set => SetAndRaiseIfChanged(ref _activeProvider, value);
        }

        public ICommand OkCommand { get; }
        public ICommand CancelCommand { get; }

        private IBootablePublishProvider _activeProvider;

        public PublishWindowViewModel(ConfiguredProject configuredProject)
        {
            var publishProviders = new OrderPrecedenceImportCollection<IBootablePublishProvider>(
                projectCapabilityCheckProvider: configuredProject);

            foreach (var provider in configuredProject.Services.ExportProvider
                .GetExports<IBootablePublishProvider, IOrderPrecedenceMetadataView>())
            {
                publishProviders.Add(provider);
            }

            PublishProviders = publishProviders.Select(p => p.Value);
            ActiveProvider = PublishProviders.FirstOrDefault();

            OkCommand = new CloseDialogCommand(true);
            CancelCommand = new CloseDialogCommand(false);
        }

        private void SetAndRaiseIfChanged<T>(ref T property, T value, [CallerMemberName] string propertyName = null)
        {
            if (!EqualityComparer<T>.Default.Equals(property, value))
            {
                property = value;
                OnPropertyChanged(propertyName);
            }
        }

        private void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private class CloseDialogCommand : ICommand
        {
            public event EventHandler CanExecuteChanged;

            private bool _dialogResult;

            public CloseDialogCommand(bool dialogResult)
            {
                _dialogResult = dialogResult;
            }

            public bool CanExecute(object parameter) => parameter is Window;

            public void Execute(object parameter)
            {
                var window = (Window)parameter;

                window.DialogResult = _dialogResult;
                window.Close();
            }

            public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
