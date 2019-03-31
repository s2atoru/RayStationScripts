using System.Collections.ObjectModel;
using OptimizatoinSettings.Models;

namespace OptimizatoinSettings.ViewModels
{
    public class OptimizationFunctionViewModel : BindableBaseWithErrorsContainer
    {
        public ObservableCollection<OptimizationFunction> OptimizationFunctions { get; set; } = new ObservableCollection<OptimizationFunction>();
        public ObservableCollection<string> StructureNames { get; set; } = new ObservableCollection<string>();

        public OptimizationFunctionViewModel()
        {
            OptimizationFunctions.Add(new OptimizationFunction { FunctionType = "MaxEud", RoiName = "Rectum", DoseLevel = 2500, EudParameterA = 1, Weight = 1 });
            OptimizationFunctions.Add(new OptimizationFunction { FunctionType = "MaxEud", RoiName = "Bladder", DoseLevel = 2000, EudParameterA = 1, Weight = 1 });
            OptimizationFunctions.Add(new OptimizationFunction { FunctionType = "UniformDose", RoiName = "PTV", DoseLevel = 7600,  Weight = 100 });

            StructureNames = new ObservableCollection<string>{ "Rectum", "PTV" };

        }
    }
}
