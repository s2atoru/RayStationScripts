using Prism.Mvvm;

namespace OptimizatoinSettings.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        public SetParametersViewModel SetParametersViewModel { get; set; }

        public MainWindowViewModel()
        {
            SetParametersViewModel = new SetParametersViewModel();
        }
    }
}
