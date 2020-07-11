using Microsoft.VisualStudio.ProjectSystem;

using VSPropertyPages;

namespace XSharp.ProjectSystem.VS.PropertyPages
{
    internal class CompilePropertyPageViewModel : PropertyPageViewModel
    {
        private const string OutputTypeProperty = "OutputType";
        private const string OutputNameProperty = "OutputName";

        public CompilePropertyPageViewModel(
            IPropertyManager aPropertyManager,
            IProjectThreadingService aProjectThreadingService)
            : base(aPropertyManager, aProjectThreadingService)
        {
        }

        public string OutputType
        {
            get => GetProperty(OutputTypeProperty);
            set => SetProperty(OutputTypeProperty, value);
        }

        public string OutputName
        {
            get => GetProperty(OutputNameProperty);
            set => SetProperty(OutputNameProperty, value);
        }
    }
}
