using CsvHelper;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Juntendo.MedPhys
{
    public enum DvhObjectiveType { Max, Min, MeanUpper, MeanLower, Upper, Lower, Spare };
    public enum DvhTargetType { Dose, Volume };
    public enum DvhPresentationType { None, Abs, Rel };

    public enum DvhDoseUnit { None, Percent, Gy };

    public enum DvhVolumeUnit { None, Percent, cc };

    public class DvhObjective : BindableBase
    {

        public string ProtocolId { get; private set; }

        private string title;
        public string Title
        {
            get { return title; }
            set
            {
                this.SetProperty(ref this.title, value);
            }
        }

        private bool inUse;
        public bool InUse
        {
            get { return inUse; }
            set
            {
                this.SetProperty(ref this.inUse, value);
            }
        }

        private string structureName;
        public string StructureName
        {
            get { return structureName; }
            set
            {
                this.SetProperty(ref this.structureName, value);
            }
        }

        private string structureNameTps;
        public string StructureNameTps
        {
            get { return structureNameTps; }
            set
            {
                this.SetProperty(ref this.structureNameTps, value);
            }
        }

        private DvhObjectiveType objectiveType;
        public DvhObjectiveType ObjectiveType
        {
            get { return objectiveType; }
            set
            {
                this.SetProperty(ref this.objectiveType, value);
            }
        }

        private DvhTargetType targetType;
        public DvhTargetType TargetType
        {
            get { return targetType; }
            private set
            {
                this.SetProperty(ref this.targetType, value);
            }
        }

        private DvhPresentationType targetUnit;
        public DvhPresentationType TargetUnit
        {
            get { return targetUnit; }
            private set
            {
                this.SetProperty(ref this.targetUnit, value);
            }
        }

        private double targetValue;
        public double TargetValue
        {
            get { return targetValue; }
            private set
            {
                this.SetProperty(ref this.targetValue, value);
            }
        }

        private DvhPresentationType argumentUnit;
        public DvhPresentationType ArgumentUnit
        {
            get { return argumentUnit; }
            private set
            {
                this.SetProperty(ref this.argumentUnit, value);
            }
        }

        private double argumentValue;
        public double ArgumentValue
        {
            get { return argumentValue; }
            private set
            {
                this.SetProperty(ref this.argumentValue, value);
            }
        }

        private DvhDoseUnit doseUnit;
        public DvhDoseUnit DoseUnit
        {
            get { return doseUnit; }
            private set
            {
                this.SetProperty(ref this.doseUnit, value);
            }
        }

        private double value;
        public double Value
        {
            get { return value; }
            set
            {
                this.SetProperty(ref this.value, value);
            }
        }

        private DvhVolumeUnit volumeUnit;
        public DvhVolumeUnit VolumeUnit
        {
            get { return volumeUnit; }
            private set
            {
                this.SetProperty(ref this.volumeUnit, value);
            }
        }

        private double acceptableLimitValue;
        public double AcceptableLimitValue
        {
            get { return acceptableLimitValue; }
            private set
            {
                this.SetProperty(ref this.acceptableLimitValue, value);
            }
        }

        private string remarks;
        public string Remarks
        {
            get { return remarks; }
            set
            {
                this.SetProperty(ref this.remarks, value);
            }
        }

        private bool isPassed;
        public bool IsPassed
        {
            get { return isPassed; }
            set
            {
                this.SetProperty(ref this.isPassed, value);
            }
        }

        private double volume;
        public double Volume
        {
            get { return volume; }
            set
            {
                this.SetProperty(ref this.volume, value);
            }
        }

        public DvhObjective(ObjectiveCsv objectiveCsv)
        {
            ProtocolId = objectiveCsv.ProtocolId;
            Title = objectiveCsv.Title;
            StructureName = objectiveCsv.StructureName;
            ObjectiveType = (DvhObjectiveType)Enum.Parse(typeof(DvhObjectiveType), objectiveCsv.ObjectiveType);
            TargetType = (DvhTargetType)Enum.Parse(typeof(DvhTargetType), objectiveCsv.TargetType);
            ArgumentValue = string.IsNullOrEmpty(objectiveCsv.ArgumentValue) ? 0.0 : double.Parse(objectiveCsv.ArgumentValue);
            TargetValue = double.Parse(objectiveCsv.TargetValue);
            AcceptableLimitValue = string.IsNullOrEmpty(objectiveCsv.AcceptableLimitValue) ? 0.0 : double.Parse(objectiveCsv.AcceptableLimitValue);
            Remarks = objectiveCsv.Remarks;

            StructureNameTps = objectiveCsv.StructureNameTps;

            string argumentUnit = objectiveCsv.ArgumentUnit;
            if (argumentUnit == "%")
            {
                argumentUnit = "Percent";
            }
            string targetUnit = objectiveCsv.TargetUnit; ;
            if (targetUnit == "%")
            {
                targetUnit = "Percent";
            }

            switch (TargetType)
            {
                case DvhTargetType.Dose:
                    DoseUnit = (DvhDoseUnit)Enum.Parse(typeof(DvhDoseUnit), targetUnit);
                    VolumeUnit = string.IsNullOrEmpty(argumentUnit) ? DvhVolumeUnit.None : (DvhVolumeUnit)Enum.Parse(typeof(DvhVolumeUnit), argumentUnit);
                    if (DoseUnit == DvhDoseUnit.Percent)
                    {
                        TargetUnit = DvhPresentationType.Rel;
                    }
                    else
                    {
                        TargetUnit = DvhPresentationType.Abs;
                    }

                    if (VolumeUnit == DvhVolumeUnit.Percent)
                    {
                        ArgumentUnit = DvhPresentationType.Rel;
                    }
                    else if (VolumeUnit == DvhVolumeUnit.None)
                    {
                        ArgumentUnit = DvhPresentationType.None;
                    }
                    else
                    {
                        ArgumentUnit = DvhPresentationType.Abs;
                    }
                    break;

                case DvhTargetType.Volume:
                    DoseUnit = string.IsNullOrEmpty(argumentUnit) ? DvhDoseUnit.None : (DvhDoseUnit)Enum.Parse(typeof(DvhDoseUnit), argumentUnit);
                    VolumeUnit = (DvhVolumeUnit)Enum.Parse(typeof(DvhVolumeUnit), targetUnit);
                    if (VolumeUnit == DvhVolumeUnit.Percent)
                    {
                        TargetUnit = DvhPresentationType.Rel;
                    }
                    else
                    {
                        TargetUnit = DvhPresentationType.Abs;
                    }

                    if (DoseUnit == DvhDoseUnit.Percent)
                    {
                        ArgumentUnit = DvhPresentationType.Rel;
                    }
                    else if (DoseUnit == DvhDoseUnit.None)
                    {
                        ArgumentUnit = DvhPresentationType.None;
                    }
                    else
                    {
                        ArgumentUnit = DvhPresentationType.Abs;
                    }
                    break;
            }

        }

        public static List<DvhObjective> ReadObjectivesFromCsv(string filePath)
        {
            List<DvhObjective> objectives = new List<DvhObjective>();
            using (StreamReader sr = new StreamReader(filePath, Encoding.GetEncoding("shift_jis")))
            {
                var csv = new CsvReader(sr);
                csv.Read();
                csv.ReadHeader();
                csv.Read();
                var protocolId = csv["Protocol ID"];
                csv.Read();
                csv.ReadHeader();
                while (csv.Read())
                {
                    string title = csv["Title"];
                    string structureName = csv["Structure Name"];
                    string objectiveType = csv["Objective Type"];
                    string targetType = csv["Target Type"];
                    string targetValue = csv["Target Value"];
                    string targetUnit = csv["Target Unit"];
                    string acceptableLimitValue = csv["Acceptable Limit Value"];
                    string argumentValue = csv["Argument Value"];
                    string argumentUnit = csv["Argument Unit"];
                    string remarks = csv["Remarks"];
                    string structureNameTps = csv["Structure Name TPS"];

                    var objectiveCsv = new ObjectiveCsv()
                    {
                        ProtocolId = protocolId,
                        Title = title,
                        StructureName = structureName,
                        ObjectiveType = objectiveType,
                        TargetType = targetType,
                        TargetValue = targetValue,
                        TargetUnit = targetUnit,
                        AcceptableLimitValue = acceptableLimitValue,
                        ArgumentValue = argumentValue,
                        ArgumentUnit = argumentUnit,
                        Remarks = remarks,
                        StructureNameTps = structureNameTps
                    };

                    var objective = new DvhObjective(objectiveCsv);
                    objectives.Add(objective);
                }
            }
            return objectives;
        }

        public static void WriteObjectivesToFile(List<DvhObjective> objectives, string protocolName, string filePath, PlanInfo planInfo)
        {
            using (StreamWriter sw = new StreamWriter(filePath, false, Encoding.GetEncoding("shift_jis")))
            {
                
                sw.WriteLine("Protocol ID,Plan ID,Course ID,Patient Name,Patient ID");
                sw.WriteLine(protocolName + ',' + planInfo.PlanId + ',' + planInfo.CourseId + ',' + planInfo.PatientName + ',' + planInfo.PatientId);
                sw.WriteLine("Title,Structure Name,Structure Name TPS,Objective Type,Target Type,Target Value,Target Unit,Acceptable Limit Value,Argument Value,Argument Unit,Remarks,Value,Volume,IsPassed");
                foreach (var o in objectives)
                {
                    var line = o.Title + ",";
                    line += o.StructureName + ",";
                    line += o.StructureNameTps + ",";
                    line += o.ObjectiveType + ",";
                    line += o.TargetType + ",";
                    line += o.TargetValue + ",";
                    line += getTargetUnit(o.TargetType, o.TargetUnit) + ",";
                    line += o.AcceptableLimitValue + ",";
                    line += o.ArgumentValue + ",";
                    line += getArgumentUnit(o.TargetType, o.ArgumentUnit) + ",";
                    line += o.Remarks+",";
                    line += o.Value + ",";
                    line += o.Volume + ",";
                    line += o.IsPassed;

                    sw.WriteLine(line);
                }
            }
        }

        private static string getTargetUnit(DvhTargetType targetType, DvhPresentationType presentationType)
        {
            switch (targetType)
            {
                case DvhTargetType.Dose:
                    if(presentationType == DvhPresentationType.Abs)
                    {
                        return "Gy";
                    }
                    else if (presentationType == DvhPresentationType.Rel)
                    {
                        return "%";
                    }
                    else
                    {
                        return string.Empty;
                    }
                case DvhTargetType.Volume:
                    if (presentationType == DvhPresentationType.Abs)
                    {
                        return "cc";
                    }
                    else if (presentationType == DvhPresentationType.Rel)
                    {
                        return "%";
                    }
                    else
                    {
                        return string.Empty;
                    }
                default:
                    return string.Empty;
            }
        }

        private static string getArgumentUnit(DvhTargetType targetType, DvhPresentationType presentationType)
        {
            switch (targetType)
            {
                case DvhTargetType.Volume:
                    if (presentationType == DvhPresentationType.Abs)
                    {
                        return "Gy";
                    }
                    else if (presentationType == DvhPresentationType.Rel)
                    {
                        return "%";
                    }
                    else
                    {
                        return string.Empty;
                    }
                case DvhTargetType.Dose:
                    if (presentationType == DvhPresentationType.Abs)
                    {
                        return "cc";
                    }
                    else if (presentationType == DvhPresentationType.Rel)
                    {
                        return "%";
                    }
                    else
                    {
                        return string.Empty;
                    }
                default:
                    return string.Empty;
            }
        }

    }

    public struct ObjectiveCsv
    {
        public string ProtocolId;
        public string Title;
        public string StructureName;
        public string ObjectiveType;
        public string TargetType;
        public string ArgumentValue;
        public string ArgumentUnit;
        public string TargetValue;
        public string TargetUnit;
        public string AcceptableLimitValue;
        public string Remarks;
        public string StructureNameTps;
    }

    public struct PlanInfo
    {
        public string PatientId;
        public string PatientName;
        public string CourseId;
        public string PlanId;
 
    }
}
