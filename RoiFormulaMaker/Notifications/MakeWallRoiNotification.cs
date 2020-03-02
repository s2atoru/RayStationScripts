using Prism.Interactivity.InteractionRequest;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace RoiFormulaMaker.Notifications
{
    public class MakeWallRoiNotification : Confirmation
    {
        public string StructureName { get; set; }
        public string BaseStructureName { get; set; }
        public double InnerMargin { get; set; }
        public double OuterMargin { get; set; }

        public ObservableCollection<string> StructureNames { get; set; }
        public ObservableCollection<string> ContouredStructureNames { get; set; }

        public string StructureType { get; set; }
        public IList<string> StructureTypes { get; set; }
    }
}
