using CsvHelper;
using System.IO;
using Prism.Mvvm;

namespace Juntendo.MedPhys
{
    public enum DvhType { Dose, Volume };
    public enum DvhTargetType { Max, Min, Mean, Upper, Lower, Spare };
    public enum DvhPresentationType { Abs, Rel };

    public class DvhObjective : BindableBase
    {
        private string title;
        public string Title {
            get { return title; }
            set
            {
                this.SetProperty(ref this.title, value);
            }
        }

        private string structureName;
        public string StructureName {
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

        private DvhType type; 
        public DvhType Type {
            get { return type; }
            set
            {
                this.SetProperty(ref this.type, value);
            }
        }

        private DvhTargetType targetType;
        public DvhTargetType TargetType {
            get { return targetType; }
            private set
            {
                this.SetProperty(ref this.targetType, value);
            }
        }

        private DvhPresentationType valueType;
        public DvhPresentationType ValueType {
            get { return valueType; }
            private set
            {
                this.SetProperty(ref this.valueType, value);
            }
        }

        private double volume;
        public double Volume {
            get { return volume; }
            private set
            {
                this.SetProperty(ref this.volume, value);
            }
        }

        private DvhPresentationType argumentType;
        public DvhPresentationType ArgumentType {
            get { return argumentType; }
            private set
            {
                this.SetProperty(ref this.argumentType, value);
            }
        }

        private double tolerance;
        public double Tolerance {
            get { return tolerance; }
            private set
            {
                this.SetProperty(ref this.tolerance, value);
            }
        }

        private double acceptableLimit;
        public double AcceptableLimit {
            get { return acceptableLimit; }
            private set
            {
                this.SetProperty(ref this.acceptableLimit, value);
            }
        }

        private string remarks;
        public string Remarks {
            get { return remarks; }
            set
            {
                this.SetProperty(ref this.remarks, value);
            }
        }

        private bool isPassed;
        public bool IsPassed {
            get { return isPassed; }
            set
            {
                this.SetProperty(ref this.isPassed, value);
            }
        }
    }
}
