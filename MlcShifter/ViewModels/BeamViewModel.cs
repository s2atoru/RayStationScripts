using Prism.Mvvm;
using System.Collections.Generic;
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

        public ObservableCollection<Models.MlcShiftDetail> MlcShiftDetails { get; set; } = new ObservableCollection<Models.MlcShiftDetail>();

        public int NumberOfLeafPairs = 60;

        public BeamViewModel(string beamId)
        {
            BeamId = beamId;
            for (int i = 0; i < NumberOfLeafPairs; i++)
            {
                // Stored reversely for displaying MlcShiftDetail from 60 to 1
                MlcShiftDetails.Add(new Models.MlcShiftDetail { LeafNumber = NumberOfLeafPairs - i, IsCheckedA = false, IsCheckedB = false, ShiftA = 0, ShiftB = 0 });
            }
        }

        public BeamViewModel(string beamId, List<double> maximumGaps)
        {
            BeamId = beamId;
            for (int i = 0; i < NumberOfLeafPairs; i++)
            {
                // Stored reversely for displaying MlcShiftDetail from 60 to 1
                MlcShiftDetails.Add(new Models.MlcShiftDetail { LeafNumber = NumberOfLeafPairs - i, IsCheckedA = false, IsCheckedB = false, ShiftA = 0, ShiftB = 0, MaximumGap = maximumGaps[NumberOfLeafPairs - i - 1] });
            }
        }

        public BeamViewModel(string beamId, List<double> maximumGaps, double maximumX2)
        {
            BeamId = beamId;
            for (int i = 0; i < NumberOfLeafPairs; i++)
            {
                // Stored reversely for displaying MlcShiftDetail from 60 to 1
                MlcShiftDetails.Add(new Models.MlcShiftDetail { LeafNumber = NumberOfLeafPairs - i, IsCheckedA = false, IsCheckedB = false, ShiftA = maximumX2, ShiftB = maximumX2, MaximumGap = maximumGaps[NumberOfLeafPairs - i - 1] });
            }
        }
    }
}
