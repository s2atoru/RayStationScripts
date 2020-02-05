using Prism.Commands;


namespace ClinicalGoal.ViewModels
{
    class MainWindowViewModel : BindableBaseWithErrorsContainer
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
