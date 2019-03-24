namespace RoiFormulaMaker.Models
{
    public class RingParameters
    {
        public string StructureName { get; set; }
        public string BaseStructureName { get; set; }
        public int InnerMargin { get; set; }
        public int OuterMargin { get; set; }

        public override string ToString()
        {
            return $"Ring: {StructureName}, BaseStrucutre : {BaseStructureName}, Outer Margin = {OuterMargin} mm, Inner Margin = {InnerMargin} mm";
        }

        //Return true if obj is equivalent to this
        public override bool Equals(object obj)
        {
            //Return false if obj is null or different type from this
            if (obj == null || this.GetType() != obj.GetType())
            {
                return false;
            }
            //Compare BaseStructureName, InnerMargin, and OuterMargin
            RingParameters c = (RingParameters)obj;
            return (this.BaseStructureName == c.BaseStructureName) && (this.InnerMargin == c.InnerMargin) && (this.OuterMargin == c.OuterMargin);
        }

        //Return the same value if Equals return true
        public override int GetHashCode()
        {
            //XOR
            return this.BaseStructureName.GetHashCode() ^ this.OuterMargin ^ this.InnerMargin;
        }

        //Overload of equality operators, == and !=
        public static bool operator ==(RingParameters c1, RingParameters c2)
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

        public static bool operator !=(RingParameters c1, RingParameters c2)
        {
            //Return the opposite of ==
            return !(c1 == c2);
        }
    }
}
