using MvvmCommon.ViewModels;
using System.Collections.Generic;

namespace OptimizationFunctionCopyManager.ViewModels
{
    public class OptimizationFunctionCopyManagerViewModel : BindableBaseWithErrorsContainer
    {
        List<string> RoiNamesInObjectiveFunctions = new List<string>();
        List<string> ContouredRoiNames = new List<string>();
    }
}
