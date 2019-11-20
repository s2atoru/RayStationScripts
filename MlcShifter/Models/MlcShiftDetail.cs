using Prism.Mvvm;
namespace MlcShifter.Models
{
    public class MlcShiftDetail : BindableBase
    {
        private int leafNumber;
        public int LeafNumber
        {
            get { return leafNumber; }
            set { SetProperty(ref leafNumber, value); }
        }

        private bool isCheckedA = false;
        public bool IsCheckedA
        {
            get { return isCheckedA; }
            set { SetProperty(ref isCheckedA, value); }
        }

        private bool isCheckedB = false;
        public bool IsCheckedB
        {
            get { return isCheckedB; }
            set { SetProperty(ref isCheckedB, value); }
        }

        private double shiftA;
        public double ShiftA
        {
            get { return shiftA; }
            set { SetProperty(ref shiftA, value); }
        }

        private double shiftB;
        public double ShiftB
        {
            get { return shiftB; }
            set { SetProperty(ref shiftB, value); }
        }

        private double maximumGap;
        public double MaximumGap
        {
            get { return maximumGap; }
            set { SetProperty(ref maximumGap, value); }
        }

    }
}
