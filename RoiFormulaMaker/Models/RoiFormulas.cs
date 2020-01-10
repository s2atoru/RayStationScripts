using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;

namespace RoiFormulaMaker.Models
{
    public class RoiFormulas
    {
        public bool CanExecute { get; set; } = false;
        public string Description { get; set; } = string.Empty;
        public List<dynamic> Formulas { get; set; } = new List<dynamic>();

        public void WriteToFile(string filePath)
        {
            var formulaJArray = new JArray();
            foreach (dynamic f in Formulas)
            {
                formulaJArray.Add(JObject.Parse(f.ToJson()));
            }

            var jsonJObject = new JObject
            {
                ["Description"] = Description,
                ["Formulas"] = formulaJArray
            };

            using (StreamWriter file = File.CreateText(filePath))
            using (JsonTextWriter writer = new JsonTextWriter(file))
            {
                jsonJObject.WriteTo(writer);
            }
        }

        public void ReadFromFile(string filePath)
        {
            using (StreamReader file = File.OpenText(filePath))
            using (JsonTextReader reader = new JsonTextReader(file))
            {
                JObject json = (JObject)JToken.ReadFrom(reader);

                Description = (string)json["Description"];

                var formulaJArray = (JArray)json["Formulas"];

                Formulas.Clear();
                foreach (JObject formulaJObject in formulaJArray)
                {
                    string formulaType = (string)formulaJObject["FormulaType"];
                    Console.WriteLine(formulaType);

                    dynamic formula;
                    switch (formulaType)
                    {
                        case "RingRoi":
                            formula = formulaJObject.ToObject<RingRoiParameters>();
                            Formulas.Add(formula);
                            break;
                        case "WallRoi":
                            formula = formulaJObject.ToObject<WallRoiParameters>();
                            Formulas.Add(formula);
                            break;
                        case "MarginAddedRoi":
                            formula = formulaJObject.ToObject<MarginAddedRoiParameters>();
                            Formulas.Add(formula);
                            break;
                        case "OverlappedRoi":
                            formula = formulaJObject.ToObject<OverlappedRoiParameters>();
                            Formulas.Add(formula);
                            break;
                        case "RoiSubtractedRoi":
                            formula = formulaJObject.ToObject<RoiSubtractedRoiParameters>();
                            Formulas.Add(formula);
                            break;
                        default:
                            break;
                    }
                }

            }
        }
    }

}
