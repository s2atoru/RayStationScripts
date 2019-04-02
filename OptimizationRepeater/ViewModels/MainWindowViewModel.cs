using OptimizationRepeater.Models;

namespace OptimizationRepeater.ViewModels
{
    public class MainWindowViewModel
    {
        public OptimizationRepeaterViewModel OptimizationRepeaterViewModel { get; set; } = new OptimizationRepeaterViewModel();
        public OptimizationFunctionViewModel OptimizationFunctionViewModel { get; set; } = new OptimizationFunctionViewModel();
    }
}
