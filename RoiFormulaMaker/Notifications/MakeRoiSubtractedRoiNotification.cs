using Prism.Interactivity.InteractionRequest;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace RoiFormulaMaker.Notifications
{
    public class MakeRoiSubtractedRoiNotification : Confirmation
    {
        public string StructureName { get; set; }
        public string BaseStructureName { get; set; }
        public string SubtractedRoiName { get; set; }
        public int Margin { get; set; }

        public ObservableCollection<string> StructureNames { get; set; }
        public ObservableCollection<string> ContouredStructureNames { get; set; }

        public string StructureType { get; set; }
        public IList<string> StructureTypes { get; set; }
    }
}
