using Prism.Mvvm;

namespace ClinicalGoal.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string title = "Clinical Goal";
        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }
        public ClinicalGoalViewModel ClinicalGoalViewModel { get; set; }  = new ClinicalGoalViewModel();
    }
}
