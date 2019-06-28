using Prism.Mvvm;
using System.Collections.Generic;
using System.Windows.Media;

namespace RoiManager.Models
{
    public class Roi : BindableBase
    {
        public string CaseName { get; set; }
        public string ExaminationName { get; set; }
        public string Name { get; set; }
        public bool IsDerived { get; set; }
        public bool CanUpdate { get; set; }
        public bool HasGeometry { get; set; }
        public List<string> DependentRois { get; set; } = new List<string>();
        public bool CanUnderive { get; set; }
        public bool CanDeleteGeometry { get; set; }
        public bool CanDeleteRoi { get; set; }
        public bool CanRename { get; set; }
        public string NewName { get; set; }

        public bool CanChangeColor { get; set; }
        public Color Color { get; set; }

        public override string ToString()
        {
            return $"ROI Name: {Name}, HasGeometry: {HasGeometry}, CanUnderive: {CanUnderive}, CanUpdate: {CanUpdate}, CanDeleteGeometry: {CanDeleteGeometry}, CanDeleteRoi: {CanDeleteRoi}"
                + $", IsDerved: {IsDerived}, DependentRois: [{string.Join(",", DependentRois)}], Case: {CaseName}, Examination: {ExaminationName}"
                + $", CanRename: {CanRename}, NewName: {NewName}, CanChageColor: {CanChangeColor}, Color: {Color}";
        }
    }
}
