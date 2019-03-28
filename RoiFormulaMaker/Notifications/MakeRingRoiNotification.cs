using Prism.Interactivity.InteractionRequest;
using System.Collections.Generic;

namespace RoiFormulaMaker.Notifications
{
    public class MakeRingRoiNotification : Confirmation
    {
        public string StructureName { get; set; }
        public string BaseStructureName { get; set; }
        public int InnerMargin { get; set; }
        public int OuterMargin { get; set; }
        public IList<string> StructureNames { get; set; }

        public string StructureType { get; set; }
        public IList<string> StructureTypes { get; set; }
    }
}
