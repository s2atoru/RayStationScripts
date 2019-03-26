using Prism.Interactivity.InteractionRequest;
using System.Collections.Generic;

namespace RoiFormulaMaker.Notifications
{
    public class MakeRoiSubtractedRoiNotification : Confirmation
    {
        public string StructureName { get; set; }
        public string BaseStructureName { get; set; }
        public string SubtractedRoiName { get; set; }
        public int Margin { get; set; }
        public IList<string> StructureNames { get; set; }
    }
}
