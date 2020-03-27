using MvvmCommon.ViewModels;
using System.Collections.Generic;

namespace OptimizationFunctionCopyManager.Models
{
    public class Roi : BindableBaseWithErrorsContainer
    {
        public string Name { get; private set; } = null;
        private bool inUse;
        public bool InUse
        {
            get { return inUse; }
            set { SetProperty(ref inUse, value); }
        }
        private string nameInObjectiveFunction;
        public string NameInObjectiveFunction
        {
            get { return nameInObjectiveFunction; }
            set { SetProperty(ref nameInObjectiveFunction, value); }
        }
        public List<string> ObjectiveFunctionArguments { get; set; } = new List<string>();

        public Roi(string name)
        {
            Name = name;
        }
    }
}
