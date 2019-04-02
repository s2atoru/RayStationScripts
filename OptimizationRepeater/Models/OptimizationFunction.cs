using Prism.Mvvm;

namespace OptimizationRepeater.Models
{
    public class OptimizationFunction : BindableBase
    {

        public int Order { get; set; } = 0;

        private string functionType;
        public string FunctionType
        {
            get { return functionType; }
            set
            {
                if (SetProperty(ref functionType, value))
                {
                    UpdateDescription();
                }
            }
        }

        public string PlanLabel { get; set; }

        private string roiNameTps;
        public string RoiNameTps
        {
            get { return roiNameTps; }
            set { SetProperty(ref roiNameTps, value); }
        }

        public double Weight { get; set; } = 1.0;

        private bool isBoosted = false;
        public bool IsBoosted
        {
            get { return isBoosted; }
            set { SetProperty(ref isBoosted, value); }
        }

        private double boostedWeight = 1.0;
        public double BoostedWeight
        {
            get { return boostedWeight; }
            set { SetProperty(ref boostedWeight, value); }
        }

        public string BoostedWeightInput { get; set; }

        public string RoiName { get; set; }
        public bool IsConstraint { get; set; } = false;
        public bool IsRobust { get; set; } = false;
        public string RestrictToBeamSet { get; set; } = null;
        public bool UseRbeDose { get; set; } = false;

        public int DoseLevel { get; set; } = 0;
        public double EudParameterA { get; set; } = 1.0;
        public int PercentVolume { get; set; } = 0;

        public string LqModelParameters { get; set; } = null;

        public bool AdaptToTargetDoseLevels { get; set; } = false;
        public int HighDoseLevel { get; set; } = 0;
        public double LowDoseDistance { get; set; } = 1.0;
        public int LowDoseLevel { get; set; } = 0;
        public double RelativeLowDoseWeight { get; set; } = 1;
        public double VicinityExponent { get; set; } = 1;

        public bool RestrictAllBeamsIndividually { get; set; } = false;
        public string RestrictToBeam { get; set; } = null;

        private string description;
        public string Description
        {
            get { return description; }
            set { SetProperty(ref description, value); }
        }

        private void UpdateDescription()
        {
            Description = ToString();
        }

        public override string ToString()
        {
            string description = string.Empty;
            switch (FunctionType)
            {
                case "MinDose":
                    description = $"Min Dose: Dose level: {DoseLevel}";
                    break;
                case "MaxDose":
                    description = $"Max Dose: Dose level: {DoseLevel}";
                    break;
                case "MinDvh":
                    description = $"Min DVH: Dose level: {DoseLevel}, Percent volume: {PercentVolume}";
                    break;
                case "MaxDvh":
                    description = $"Max DVH: Dose level: {DoseLevel}, Percent volume: {PercentVolume}";
                    break;
                case "UniformDose":
                    description = $"Uniform dose: dose Level: {DoseLevel}";
                    break;
                case "MinEud":
                    description = $"Min EUD: Dose level: {DoseLevel}, Percent volume: {PercentVolume}, EUD parameter A: {EudParameterA}";
                    break;
                case "MaxEud":
                    description = $"Max EUD: Dose level: {DoseLevel}, Percent volume: {PercentVolume}, EUD parameter A: {EudParameterA}";
                    break;
                case "TargetEud":
                    description = $"Target EUD: Dose level: {DoseLevel}, Percent volume: {PercentVolume}, EUD parameter A: {EudParameterA}";
                    break;
                case "DoseFallOff":
                    description = $"Dose Fall-Off: High dose level: {HighDoseLevel}, Low dose level: {LowDoseDistance}, Low dose distance: {LowDoseDistance}"
                        + $" Adapt to target dose levels: {AdaptToTargetDoseLevels}";
                    break;
                case "UniformityConstraint":
                    description = $"Uniformity constraint: NOt implemented";
                    break;
                default:
                    description = $"No implementation for {FunctionType}";
                    break;
            }
            return description;
        }
    }
}
