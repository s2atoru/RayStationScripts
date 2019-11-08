using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Linq;

namespace MlcShifter.ViewModels
{
    public class MlcShifterViewModel : BindableBase
    {
        public ObservableCollection<Models.MlcShiftDetail> MlcDetails { get; set; } = new ObservableCollection<Models.MlcShiftDetail>();

        public int NumberOfLeafPairs = 60;

        public MlcShifterViewModel()
        {
            for (int i =0; i<NumberOfLeafPairs; i++)
            {
                MlcDetails.Add(new Models.MlcShiftDetail { LeafNumber = NumberOfLeafPairs -i , IsCheckedA = false, IsCheckedB = false, ShiftA = 0, ShiftB = 0 });
            }
        }
    }
}
