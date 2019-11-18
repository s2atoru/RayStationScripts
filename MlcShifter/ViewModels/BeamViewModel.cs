using Prism.Mvvm;
using System.Collections.ObjectModel;

namespace MlcShifter.ViewModels
{
    public class BeamViewModel : BindableBase
    {
        private string beamId;
        public string BeamId
        {
            get { return beamId; }
            set { SetProperty(ref beamId, value); }
        }

        public ObservableCollection<Models.MlcShiftDetail> MlcDetails { get; set; } = new ObservableCollection<Models.MlcShiftDetail>();

        public int NumberOfLeafPairs = 60;

        public BeamViewModel(string beamId)
        {
            BeamId = beamId;
            for (int i = 0; i < NumberOfLeafPairs; i++)
            {
                MlcDetails.Add(new Models.MlcShiftDetail { LeafNumber = NumberOfLeafPairs - i, IsCheckedA = false, IsCheckedB = false, ShiftA = 0, ShiftB = 0 });
            }
        }
    }
}
