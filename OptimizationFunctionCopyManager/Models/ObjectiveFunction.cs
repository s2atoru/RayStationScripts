using MvvmCommon.ViewModels;
using Newtonsoft.Json.Linq;

namespace OptimizationFunctionCopyManager.Models
{
    public class ObjectiveFunction : BindableBaseWithErrorsContainer
    {
        private bool inUse;
        public bool InUse
        {
            get { return inUse; }
            set { SetProperty(ref inUse, value); }
        }
        public string RoiName { get; private set; }
        public string Description { get; private set; }
        private double weight = 0.0;
        public double Weight
        {
            get { return weight; }
            set { SetProperty(ref weight, value); }
        }

        public JObject Arguments { get; private set ; }

        public string PlanLabel { get; private set; }

        public ObjectiveFunction(JObject arguments)
        {
            Arguments = arguments;

            RoiName = Arguments["RoiName"].ToObject<string>();
            Weight = Arguments["Weight"].ToObject<double>();
            PlanLabel = Arguments["PlanLabel"].ToObject<string>();
            Description = ToDescription();
        }

        public string ToDescription()
        {
            var functionType = Arguments["FunctionType"].ToObject<string>();

            string description;
            switch (functionType)
            {
                case "DoseFallOff":
                    var highDoseLevel = Arguments["HighDoseLevel"].ToObject<double>();
                    var lowDoseLevel = Arguments["LowDoseLevel"].ToObject<double>();
                    var lowDoseDistance = Arguments["LowDoseDistance"].ToObject<double>();
                    description = $"Dose Fall-Off [H]{highDoseLevel:F0} cGy [L]{lowDoseLevel:F0} cGy, Low dose distance {lowDoseDistance:F2} cm";
                    break;
                case "UniformDose":
                    var doseLevel = Arguments["DoseLevel"].ToObject<double>();
                    description = $"Uniform Dose {doseLevel:F0} cGy";
                    break;
                case "MaxDose":
                    doseLevel = Arguments["DoseLevel"].ToObject<double>();
                    description = $"Max Dose {doseLevel:F0} cGy";
                    break;
                case "MinDose":
                    doseLevel = Arguments["DoseLevel"].ToObject<double>();
                    description = $"Min Dose {doseLevel:F0} cGy";
                    break;
                case "MaxEud":
                    doseLevel = Arguments["DoseLevel"].ToObject<double>();
                    var eudParameterA = Arguments["EudParameterA"].ToObject<double>();
                    description = $"Max EUD {doseLevel:F0} cGy, Parameter A {eudParameterA}";
                    break;
                case "MinEud":
                    doseLevel = Arguments["DoseLevel"].ToObject<double>();
                    eudParameterA = Arguments["EudParameterA"].ToObject<double>();
                    description = $"Min EUD {doseLevel:F0} cGy, Parameter A {eudParameterA}";
                    break;
                case "MaxDvh":
                    doseLevel = Arguments["DoseLevel"].ToObject<double>();
                    var percentVolume = Arguments["PercentVolume"].ToObject<double>();
                    description = $"Max DVH {doseLevel:F0} cGy to {percentVolume:F0}% volume";
                    break;
                case "MinDvh":
                    doseLevel = Arguments["DoseLevel"].ToObject<double>();
                    percentVolume = Arguments["PercentVolume"].ToObject<double>();
                    description = $"Min DVH {doseLevel:F0} cGy to {percentVolume:F0}% volume";
                    break;
                case "UniformEud":
                    doseLevel = Arguments["DoseLevel"].ToObject<double>();
                    eudParameterA = Arguments["EudParameterA"].ToObject<double>();
                    description = $"Target EUD {doseLevel:F0} cGy, Parameter A {eudParameterA}";
                    break;
                default:
                    description = "Not implemented";
                    break;
            }

            return description;
        }

        public void UpdateWeightInArguments()
        {
            Arguments["Weight"] = Weight;
        }
        public void SetRoiNameInArguments(string roiName)
        {
            Arguments["RoiName"] = roiName;
        }

        public void SetPlanLabelInArguments(string planLabel)
        {
            Arguments["PlanLabel"] = planLabel;
        }
    }
}
