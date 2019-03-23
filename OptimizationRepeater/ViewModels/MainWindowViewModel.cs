using OptimizationRepeater.Models;

namespace OptimizationRepeater.ViewModels
{
    public class MainWindowViewModel
    {

        public OptimizationRepeaterViewModel OptimizationRepeaterViewModel { get; set; } = new OptimizationRepeaterViewModel();

        public MainWindowViewModel() { }

        public MainWindowViewModel(RepetitionParameters repetitionParameters)
        {
            OptimizationRepeaterViewModel.RepetitionParameters = repetitionParameters;
        }
    }
}
