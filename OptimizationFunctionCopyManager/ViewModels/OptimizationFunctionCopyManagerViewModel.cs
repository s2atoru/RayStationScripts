using OptimizationFunctionCopyManager.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace OptimizationFunctionCopyManager.ViewModels
{
    public class OptimizationFunctionCopyManagerViewModel
    {
        List<string> RoiNamesInObjectiveFunctions = new List<string>();
        ObservableCollection<Roi> ContouredRoi = new ObservableCollection<Roi>();
    }
}
