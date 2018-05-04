using CsvHelper;
using System.IO;

namespace Juntendo.MedPhys
{
    public enum DvhType { Dose, Volume };
    public enum DvhTargetType { Max, Min, Mean, Upper, Lower, Spare };
    public enum DvhPresentationType { Abs, Rel };

    public class DvhObjective
    {
        public string StructureName { get; set; }
        public string StructureNameTps { get; set; }

        public DvhType Type { get; private set; }
        public DvhTargetType TargetType { get; private set; }
        public DvhPresentationType ValueType { get; private set; }

        public double Volume { get; private set; }
        public DvhPresentationType ArgumentType { get; set; }

        public double Tolerance { get; private set; }
        public double AcceptableLimit { get; private set; }

        public string Remarks { get; set; }
    }
}
