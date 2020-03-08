using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimizationFunctionCopyManager.Models
{
    public class PlanLabel
    {
        public bool InUse { get; set; } = true;
        public string Label { get; private set; }
        public string LabelInObjectiveFuntion { get; set; }

        public PlanLabel(string planLabel)
        {
            Label = planLabel;
        }
    }
}
