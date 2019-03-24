namespace RoiFormulaMaker.Models
{
    class TargetSubtractedRoiParameters
    {
        public string FormulaType { get; } = "TargetSubtractedRoi";
        public string StructureName { get; set; }
        public string BaseStructureName { get; set; }
        public string SubtractedTargetName { get; set; }
        public int Margin { get; set; }

        public override string ToString()
        {
            return $"Target Subtracted ROI: {StructureName}, Base Structure : {BaseStructureName}, Subtracted Target = {SubtractedTargetName}, Margin = {Margin} mm";
        }

        //Return true if obj is equivalent to this
        public override bool Equals(object obj)
        {
            //Return false if obj is null or different type from this
            if (obj == null || this.GetType() != obj.GetType())
            {
                return false;
            }
            //Compare BaseStructureName, SubtractedTargetName, and Margin
            TargetSubtractedRoiParameters c = (TargetSubtractedRoiParameters)obj;
            return (this.BaseStructureName == c.BaseStructureName) && (this.SubtractedTargetName == c.SubtractedTargetName) && (this.Margin == c.Margin);
        }

        //Return the same value if Equals return true
        public override int GetHashCode()
        {
            //XOR
            return this.BaseStructureName.GetHashCode() ^ this.SubtractedTargetName.GetHashCode() ^ this.Margin;
        }

        //Overload of equality operators, == and !=
        public static bool operator ==(TargetSubtractedRoiParameters c1, TargetSubtractedRoiParameters c2)
        {
            //Check if null
            if ((object)c1 == null)
            {
                return ((object)c2 == null);
            }
            if ((object)c2 == null)
            {
                return false;
            }
            //call Equals
            return c1.Equals(c2);
        }

        public static bool operator !=(TargetSubtractedRoiParameters c1, TargetSubtractedRoiParameters c2)
        {
            //Return the opposite of ==
            return !(c1 == c2);
        }
    }
}
