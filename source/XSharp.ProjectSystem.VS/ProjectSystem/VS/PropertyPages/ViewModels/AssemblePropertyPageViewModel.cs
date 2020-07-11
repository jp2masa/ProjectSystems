using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Windows.Input;
using Microsoft.VisualStudio.ProjectSystem;
using Microsoft.Win32;

using VSPropertyPages;

using static XSharp.ProjectSystem.ConfigurationGeneral;

namespace XSharp.ProjectSystem.VS.PropertyPages
{
    internal class AssemblePropertyPageViewModel : PropertyPageViewModel
    {
        private const string AssembleProperty = "Assemble";
        private const string AssemblerProperty = "Assembler";
        private const string AssemblerOutputProperty = "AssemblerOutput";
        private const string AssemblerOutputFormatProperty = "AssemblerOutputFormat";

        public AssemblePropertyPageViewModel(
            IPropertyManager aPropertyManager,
            IProjectThreadingService aProjectThreadingService)
            : base(aPropertyManager, aProjectThreadingService)
        {
        }

        public bool Assemble
        {
            get => Boolean.Parse(GetProperty(AssembleProperty));
            set => SetProperty(AssembleProperty, value.ToString(CultureInfo.InvariantCulture), nameof(Assemble));
        }

        public string Assembler
        {
            get => GetProperty(AssemblerProperty);
            set => SetProperty(AssemblerProperty, value, nameof(Assembler), nameof(AvailableOutputFormats));
        }

        public string AssemblerOutput
        {
            get => GetPathProperty(AssemblerOutputProperty, false);
            set => SetPathProperty(AssemblerOutputProperty, value, false, nameof(AssemblerOutput));
        }

        public IReadOnlyList<string> AvailableOutputFormats => GetAvailableOutputFormats();

        public string OutputFormat
        {
            get => GetProperty(AssemblerOutputFormatProperty);
            set => SetProperty(AssemblerOutputFormatProperty, value, nameof(OutputFormat));
        }

        public ICommand BrowseAssemblerOutputCommand => new BrowseAssemblerOutputCommand(this, AssemblerOutput);

        private IReadOnlyList<string> GetAvailableOutputFormats()
        {
            switch (Assembler)
            {
                case AssemblerValues.NASM:
                    return ImmutableArray.Create("Bin", "COFF", "ELF32", "ELF64", "Win32", "Win64");
                default:
                    return Array.Empty<string>();
            }
        }
    }

    internal class BrowseAssemblerOutputCommand : ICommand
    {
        private AssemblePropertyPageViewModel mViewModel;
        private string mCurrentAssemblerOutput;

        public BrowseAssemblerOutputCommand(AssemblePropertyPageViewModel aViewModel, string aCurrentAssemblerOutput)
        {
            mViewModel = aViewModel;
            mCurrentAssemblerOutput = aCurrentAssemblerOutput;
        }

#pragma warning disable CS0067
        public event EventHandler CanExecuteChanged;
#pragma warning restore CS0067

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var xSaveFileDialog = new SaveFileDialog
            {
                FileName = mCurrentAssemblerOutput
                // todo: add filter based on available output formats?
            };

            if (xSaveFileDialog.ShowDialog().GetValueOrDefault(false))
            {
                mViewModel.AssemblerOutput = xSaveFileDialog.FileName;
            }
        }
    }
}
