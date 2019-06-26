using System.IO;
using CsvHelper;
using Juntendo.MedPhys;
using System;
using System.Collections.Generic;

namespace TestDvhChecker
{
    class Program
    {
        static void Main(string[] args)
        {
            var homePath = Environment.GetEnvironmentVariable("HOMEPATH");
            string filePath = Path.Combine(homePath, @"Desktop\DVHObjectives.csv");

            using (StreamReader sr = new StreamReader(filePath))
            {
                var csv = new CsvReader(sr);
                csv.Read();
                csv.ReadHeader();

                List<DvhObjective> Objectives = new List<DvhObjective>();
                while (csv.Read())
                {
                    string title = csv["Title"];
                    string structureName = csv["Structure Name"];
                    string objectiveType = csv["Objective Type"];
                    DvhObjectiveType objectiveTypeEnum = (DvhObjectiveType)Enum.Parse(typeof(DvhObjectiveType), objectiveType);
                    string targetType = csv["Target Type"];
                    DvhTargetType targetTypeEnum = (DvhTargetType)Enum.Parse(typeof(DvhTargetType), targetType);
                    //double argumentValue = string.IsNullOrEmpty(csv["Argument"]) ? 0.0 : csv.GetField<double>("Argument");
                    string argumentValue = csv["Argument Value"];
                    string argumentUnit = csv["Argument Unit"];
                    //double targetValue = csv.GetField < double >("Target");
                    string targetValue = csv["Target Value"];
                    string targetUnit = csv["Target Unit"];
                    //double acceptableLimitValue = string.IsNullOrEmpty(csv["Acceptable Limit Value"]) ? 0.0 : csv.GetField<double>("Acceptable Limit");
                    string acceptableLimitValue = csv["Acceptable Limit Value"];
                    string remarks = csv["Remarks"];

                    var objectiveCsv = new ObjectiveCsv()
                    {
                        Title = title,
                        StructureName = structureName,
                        ObjectiveType = objectiveType,
                        TargetType = targetType,
                        TargetValue = targetValue,
                        TargetUnit = targetUnit,
                        AcceptableLimitValue = acceptableLimitValue,
                        ArgumentValue = argumentValue,
                        ArgumentUnit = argumentUnit,
                        Remarks = remarks
                    };

                    var objective = new DvhObjective(objectiveCsv);
                    Objectives.Add(objective);
                }
            }
        }
    }
}
