using Prism.Interactivity.InteractionRequest;
using System.Collections.Generic;

namespace RoiFormulaMaker.Notifications
{
    public class MakeTargetSubtractedRoiNotification : Confirmation
    {
        public string StructureName { get; set; }
        public string BaseStructureName { get; set; }
        public string SubtractedTargetName { get; set; }
        public int Margin { get; set; }
        public IList<string> StructureNames { get; set; }
    }
}
