using Prism.Mvvm;

namespace OptimizatoinSettings.Models
{
    public class SettingParameters : BindableBase
    {
        private bool isValid = false;
        public bool IsValid
        {
            get { return isValid; }
            set { SetProperty(ref isValid, value); }
        }

        private int maximumNumberOfIterations = 40;
        public int MaximumNumberOfIterations
        {
            get { return maximumNumberOfIterations; }
            set
            {
                maximumNumberOfIterations = value;
                IsValid = CheckValidity();
            }
        }

        private int initialNumberOfIterations = 20;
        public int InitialNumberOfIterations
        {
            get { return initialNumberOfIterations; }
            set
            {
                initialNumberOfIterations = value;
                IsValid = CheckValidity();
            }
        }

        public bool FinalDoseCalculation { get; set; } = true;

        public bool CanSetParameters { get; set; } = false;

        public SettingParameters()
        {
            IsValid = CheckValidity();
        }

        private bool CheckValidity()
        {
            return (MaximumNumberOfIterations >= 1) && (InitialNumberOfIterations >= 0) && (MaximumNumberOfIterations >= InitialNumberOfIterations);
        } 
    }
}
