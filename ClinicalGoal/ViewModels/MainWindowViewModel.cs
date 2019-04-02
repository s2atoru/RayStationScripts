using Prism.Commands;


namespace ClinicalGoal.ViewModels
{
    class MainWindowViewModel : BindableBaseWithErrorsContainer
    {
        public ClinicalGoalViewModel ClinicalGoalViewModel { get; set; }  = new ClinicalGoalViewModel();
    }
}
