using MvvmCommon.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimizationFunctionCopyManager.Models
{
    public class PlanLabel : BindableBaseWithErrorsContainer
    {
        public bool InUse { get; set; } = true;
        public string Label { get; private set; }

        private string labelInObjectiveFunction;
        public string LabelInObjectiveFunction
        {
            get { return labelInObjectiveFunction; }
            set { SetProperty(ref labelInObjectiveFunction, value); }
        }

        public PlanLabel(string planLabel)
        {
            Label = planLabel;
        }
    }
}
