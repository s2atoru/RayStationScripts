using VMS.TPS.Common.Model.Types;

namespace Juntendo.MedPhys.Esapi
{
    public class DvhDataParameters
    {
        public string StructureId { get; set; }
        public DoseValuePresentation DoseValuePresentation { get; set; }
        public VolumePresentation VolumePresentation { get; set; }
        public double BinWidth { get; set; }
        public override bool Equals(object obj)
        {
            if (obj == null || this.GetType() != obj.GetType())
            {
                return false;
            }
            DvhDataParameters c = (DvhDataParameters)obj;
            return (this.StructureId == c.StructureId) && (this.DoseValuePresentation == c.DoseValuePresentation)
                && (this.VolumePresentation == c.VolumePresentation) && (this.BinWidth == c.BinWidth);
        }

        public override int GetHashCode()
        {
            //XOR
            return this.StructureId.GetHashCode() ^ this.DoseValuePresentation.GetHashCode() ^ this.VolumePresentation.GetHashCode() ^ this.BinWidth.GetHashCode();
        }

    }
}
