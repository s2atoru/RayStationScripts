using MvvmCommon.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimizationFunctionCopyManager.Models
{
    public class Roi : BindableBaseWithErrorsContainer
    {
        public bool InUse { get; set; } = true;
        public string NameInObjectiveFunction { get; private set; }
        public string NameInTps { get; set; } = null;
    }
}
