using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace RoiFormulaMaker.Models
{
    [DataContract]
    public class RoiSubtractedRoiParameters
    {
        [DataMember()]
        public string FormulaType { get; set; } = "RoiSubtractedRoi";
        [DataMember()]
        public string StructureName { get; set; }
        [DataMember()]
        public string StructureType { get; set; }
        [DataMember()]
        public string BaseStructureName { get; set; }
        [DataMember()]
        public List<string> SubtractedRoiNames { get; set; }
        [DataMember()]
        public int Margin { get; set; }

        [DataMember()]
        public string Description { get; set; } = string.Empty;

        public override string ToString()
        {
            return $"ROI Subtracted ROI: {StructureName} ({StructureType}), Base Structure: {BaseStructureName}, Subtracted ROIs = {string.Join(", ", SubtractedRoiNames)}, Margin = {Margin} mm";
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
            //Compare BaseStructureName, SubtractetRoiNames, and Margin
            RoiSubtractedRoiParameters c = (RoiSubtractedRoiParameters)obj;

            var subtractedRoiNameSet = new HashSet<string>(SubtractedRoiNames);

            return (this.BaseStructureName == c.BaseStructureName) && subtractedRoiNameSet.SetEquals(c.SubtractedRoiNames) && (this.Margin == c.Margin);
        }

        //Return the same value if Equals return true
        public override int GetHashCode()
        {
            //XOR
            return this.BaseStructureName.GetHashCode() ^ this.SubtractedRoiNames.GetHashCode() ^ this.Margin;
        }

        //Overload of equality operators, == and !=
        public static bool operator ==(RoiSubtractedRoiParameters c1, RoiSubtractedRoiParameters c2)
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

        public static bool operator !=(RoiSubtractedRoiParameters c1, RoiSubtractedRoiParameters c2)
        {
            //Return the opposite of ==
            return !(c1 == c2);
        }
    }
}
