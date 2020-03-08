using Newtonsoft.Json.Linq;

namespace OptimizationFunctionCopyManager.Models
{
    public class ObjectiveFunction
    {
        public bool InUse { get; set; } = true;
        public string RoiName { get; private set; }
        public string Description { get; private set; }
        public double Weight { get; set; } = 0.0;
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
    }
}
