namespace DoseVolumeIndex.Models
{
    public class DoseVolumeIndex
    {
        public enum ValueType { Dose, Volume }
        public enum Type { Dvh, Average }
        public enum Unit { Relative, Absolute }

        public string Title { get; set; }
        public ValueType TargetValueType { get; set; }
        public Type TargetType { get; set; }
        public Unit TargetUnit { get; set; }
        public Unit ArgumentUnit { get; set; }

        public double ArgumentValue { get; set; }
        public double Value {get; private set;}

        public string RoiName { get; set; }
        public string RoiNameTps { get; set; }
    }
}
