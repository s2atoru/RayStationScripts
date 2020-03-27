using Newtonsoft.Json;
using Prism.Mvvm;

namespace OptimizationFunctionCopyManager.Models
{
    public class Prescription : BindableBase
    {
        private string planLabel;
        [JsonProperty]
        public string PlanLabel
        {
            get { return planLabel; }
            set { SetProperty(ref planLabel, value); }
        }

        private double prescribedDose;
        [JsonProperty]
        public double PrescribedDose
        {
            get { return prescribedDose; }
            set { SetProperty(ref prescribedDose, value); }
        }

        private double prescribedDoseInObjectiveFunction;
        [JsonProperty]
        public double PrescribedDoseInObjectiveFunction
        {
            get { return prescribedDoseInObjectiveFunction; }
            set { SetProperty(ref prescribedDoseInObjectiveFunction, value); }
        }
    }
}
