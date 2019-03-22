using Prism.Commands;
using Prism.Mvvm;

namespace OptimizatoinSettings.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        public SetParametersViewModel SetParametersViewModel { get; set; } = new SetParametersViewModel();

        public MainWindowViewModel()
        {
            OkCommand = new DelegateCommand(() => { SetParametersViewModel.SettingParameters.CanSetParameters = true; }).ObservesCanExecute(() => SetParametersViewModel.CanOk);
            CancelCommand = new DelegateCommand(() => SetParametersViewModel.SettingParameters.CanSetParameters = false);
        }

        public MainWindowViewModel(Models.SettingParameters settingParameters)
        {
            SetParametersViewModel.SettingParameters = settingParameters;
            OkCommand = new DelegateCommand(() => { SetParametersViewModel.SettingParameters.CanSetParameters = true; }).ObservesCanExecute(() => SetParametersViewModel.CanOk);
            CancelCommand = new DelegateCommand(() => SetParametersViewModel.SettingParameters.CanSetParameters = false);
        }

        public DelegateCommand OkCommand { get; private set; }
        public DelegateCommand CancelCommand { get; private set; }

    }
}
