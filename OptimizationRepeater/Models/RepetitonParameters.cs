using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimizationRepeater.Models
{
    public class RepetitionParameters
    {
        public int NumberOfRepetitionTimes { get; set; } = 2;
        public bool ScaleDoseAfterEachIteration { get; set; } = true;
        public bool ScaleDoseAfterLastIteration { get; set; } = true;
        public bool ResetBeforeStartingOptimization { get; set; } = false;

        public bool CanExecute { get; set; } = false;
    }
}
