using Prism.Commands;
using Prism.Mvvm;

namespace OptimizatoinSettings.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        public SetParametersViewModel SetParametersViewModel { get; set; }

        public MainWindowViewModel()
        {
            SetParametersViewModel = new SetParametersViewModel();

            OkCommand = new DelegateCommand(() => { SetParametersViewModel.SettingParameters.CanSetParameters = true; }).ObservesCanExecute(() => SetParametersViewModel.CanOk);
            CancelCommand = new DelegateCommand(() => SetParametersViewModel.SettingParameters.CanSetParameters = false);
        }

        public DelegateCommand OkCommand { get; private set; }
        public DelegateCommand CancelCommand { get; private set; }

    }
}
