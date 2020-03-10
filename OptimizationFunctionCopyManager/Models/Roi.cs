using MvvmCommon.ViewModels;
using System.Collections.Generic;

namespace OptimizationFunctionCopyManager.Models
{
    public class Roi : BindableBaseWithErrorsContainer
    {
        public string Name { get; private set; } = null;
        public bool InUse { get; set; } = true;
        private string nameInObjectiveFunction;
        public string NameInObjectiveFunction
        {
            get { return nameInObjectiveFunction; }
            set { SetProperty(ref nameInObjectiveFunction, value); }
        }
        public List<string> ObjectiveFuntionArguments { get; set; } = new List<string>();

        public Roi(string name)
        {
            Name = name;
        }
    }
}
