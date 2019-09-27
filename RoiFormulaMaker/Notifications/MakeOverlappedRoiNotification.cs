using MvvmCommon.ViewModels;
using Prism.Interactivity.InteractionRequest;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace RoiFormulaMaker.Notifications
{
    public class MakeOverlappedRoiNotification : Confirmation
    {
        public string StructureName { get; set; }
        public List<string> BaseStructureNames { get; set; }
        public int Margin { get; set; }

        public ObservableCollection<string> StructureNames { get; set; }
        public ObservableCollection<ListBoxItemViewModel> ContouredStructureList { get; set; }

        public string StructureType { get; set; }
        public IList<string> StructureTypes { get; set; }
    }
}
