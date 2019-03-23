using OptimizationRepeater.Models;
using Prism.Commands;
using System.ComponentModel.DataAnnotations;

namespace OptimizationRepeater.ViewModels
{
    public class OptimizationRepeaterViewModel : BindableBaseWithErrorsContainer
    {

        private RepetitionParameters repetitionParameters;
        public RepetitionParameters RepetitionParameters
        {
            get { return repetitionParameters; }
            set
            {
                repetitionParameters = value;
                NumberOfRepetitionTimes = repetitionParameters.NumberOfRepetitionTimes.ToString();
                ScaleDoseAfterEachOptimization = repetitionParameters.ScaleDoseAfterEachIteration;
                scaleDoseAfterLastOptimization = repetitionParameters.ScaleDoseAfterLastIteration;
                ResetBeforeStartingOptimization = repetitionParameters.ResetBeforeStartingOptimization;
                CanOk = (repetitionParameters.NumberOfRepetitionTimes >= 1);
            }
        }

        private string numberOfRepetitionTimes = "0";
        [Required(ErrorMessage = "Required")]
        [RegularExpression(@"([1-9]\d*)", ErrorMessage = "Input integer ≧ 1")]
        public string NumberOfRepetitionTimes
        {
            get { return numberOfRepetitionTimes; }
            set { SetProperty(ref numberOfRepetitionTimes, value);
                CanOk = !HasErrors;
            }
        }

        private bool scaleDoseAfterEachOptimization = true;
        public bool ScaleDoseAfterEachOptimization
        {
            get { return scaleDoseAfterEachOptimization; }
            set { SetProperty(ref scaleDoseAfterEachOptimization, value); }
        }

        private bool scaleDoseAfterLastOptimization = true;
        public bool ScaleDoseAfterLastOptimization
        {
            get { return scaleDoseAfterLastOptimization; }
            set { SetProperty(ref scaleDoseAfterLastOptimization, value); }
        }

        private bool resetBeforeStartingOptimization = false;
        public bool ResetBeforeStartingOptimization
        {
            get { return resetBeforeStartingOptimization; }
            set { SetProperty(ref resetBeforeStartingOptimization, value); }
        }

        private bool canOk = false;
        public bool CanOk
        {
            get { return canOk; }
            set { SetProperty(ref canOk, value); }
        }

        private void SetRepetitonParameters()
        {
            RepetitionParameters.NumberOfRepetitionTimes = int.Parse(NumberOfRepetitionTimes);
            RepetitionParameters.ScaleDoseAfterEachIteration = ScaleDoseAfterEachOptimization;
            RepetitionParameters.ScaleDoseAfterLastIteration = ScaleDoseAfterLastOptimization;
            RepetitionParameters.ResetBeforeStartingOptimization = ResetBeforeStartingOptimization;
        }

        public OptimizationRepeaterViewModel()
        {
            RepetitionParameters = new RepetitionParameters();

            CanOk = (RepetitionParameters.NumberOfRepetitionTimes >= 1);

            OkCommand = new DelegateCommand(() => { RepetitionParameters.CanExecute = true; SetRepetitonParameters(); }).ObservesCanExecute(() => CanOk);
            CancelCommand = new DelegateCommand(() => { RepetitionParameters.CanExecute = false; });
        }

        public DelegateCommand OkCommand { get; private set; }
        public DelegateCommand CancelCommand { get; private set; }
    }
}
