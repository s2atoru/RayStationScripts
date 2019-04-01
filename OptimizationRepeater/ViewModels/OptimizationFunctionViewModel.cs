using System.Collections.ObjectModel;
using OptimizationRepeater.Models;

namespace OptimizationRepeater.ViewModels
{
    public class OptimizationFunctionViewModel : BindableBaseWithErrorsContainer
    {
        public ObservableCollection<OptimizationFunction> OptimizationFunctions { get; set; } = new ObservableCollection<OptimizationFunction>();

        public OptimizationFunctionViewModel()
        {
            OptimizationFunctions.Add(new OptimizationFunction { FunctionType = "MaxEud", RoiName = "Rectum", DoseLevel = 2500, EudParameterA = 1, Weight = 1, PlanLabel = "1-1-1" });
            OptimizationFunctions.Add(new OptimizationFunction { FunctionType = "MaxEud", RoiName = "Bladder", DoseLevel = 2000, EudParameterA = 1, Weight = 1, PlanLabel = "Combined dose" });
            OptimizationFunctions.Add(new OptimizationFunction { FunctionType = "UniformDose", RoiName = "PTV", DoseLevel = 7600,  Weight = 100, PlanLabel = "1-1-2" });
        }
    }
}
