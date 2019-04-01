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

        private int maxNumberOfIterations = 40;
        public int MaxNumberOfIterations
        {
            get { return maxNumberOfIterations; }
            set
            {
                maxNumberOfIterations = value;
                IsValid = CheckValidity();
            }
        }

        private int iterationsInPreparationsPhase = 20;
        public int IterationsInPreparationsPhase
        {
            get { return iterationsInPreparationsPhase; }
            set
            {
                iterationsInPreparationsPhase = value;
                IsValid = CheckValidity();
            }
        }

        public bool ComputeFinalDose { get; set; } = true;

        public bool CanSetParameters { get; set; } = false;

        public SettingParameters()
        {
            IsValid = CheckValidity();
        }

        private bool CheckValidity()
        {
            return (MaxNumberOfIterations >= 1) && (IterationsInPreparationsPhase >= 0) && (MaxNumberOfIterations >= IterationsInPreparationsPhase);
        }
    }
}
