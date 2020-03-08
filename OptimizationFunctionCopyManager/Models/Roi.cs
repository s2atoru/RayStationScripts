using System.Collections.Generic;

namespace OptimizationFunctionCopyManager.Models
{
    public class Roi
    {
        public string Name { get; private set; } = null;
        public bool InUse { get; set; } = true;
        public string NameInObjectiveFunction { get; set; } = null;
        public List<string> ObjectiveFuntionArguments { get; set; } = new List<string>();

        public Roi(string name)
        {
            Name = name;
        }
    }
}
