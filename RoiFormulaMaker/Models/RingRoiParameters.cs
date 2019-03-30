using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace RoiFormulaMaker.Models
{
    [DataContract]
    public class RingRoiParameters
    {
        [DataMember()]
        public string FormulaType { get; set; } = "RingRoi";
        [DataMember()]
        public string StructureName { get; set; }
        [DataMember()]
        public string StructureType { get; set; }
        [DataMember()]
        public string BaseStructureName { get; set; }
        [DataMember()]
        public int InnerMargin { get; set; }
        [DataMember()]
        public int OuterMargin { get; set; }

        [DataMember()]
        public string Description { get; set; } = string.Empty;

        public override string ToString()
        {
            return $"Ring ROI: {StructureName} ({StructureType}), Base Structure : {BaseStructureName}, Outer Margin = {OuterMargin} mm, Inner Margin = {InnerMargin} mm";
        }

        public string ToJson()
        {
            var serializer = new DataContractJsonSerializer(this.GetType());
            var ms = new MemoryStream();
            serializer.WriteObject(ms, this);
            string jsonString = Encoding.UTF8.GetString(ms.ToArray());
            return jsonString;
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
            RingRoiParameters c = (RingRoiParameters)obj;
            return (this.BaseStructureName == c.BaseStructureName) && (this.InnerMargin == c.InnerMargin) && (this.OuterMargin == c.OuterMargin);
        }

        //Return the same value if Equals return true
        public override int GetHashCode()
        {
            //XOR
            return this.BaseStructureName.GetHashCode() ^ this.OuterMargin ^ this.InnerMargin;
        }

        //Overload of equality operators, == and !=
        public static bool operator ==(RingRoiParameters c1, RingRoiParameters c2)
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

        public static bool operator !=(RingRoiParameters c1, RingRoiParameters c2)
        {
            //Return the opposite of ==
            return !(c1 == c2);
        }
    }
}
