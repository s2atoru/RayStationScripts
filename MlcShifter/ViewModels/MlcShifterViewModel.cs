using Prism.Commands;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MlcShifter.ViewModels
{
    public class MlcShifterViewModel : BindableBase
    {
        public ObservableCollection<BeamViewModel> BeamViewModels { get; set; } = new ObservableCollection<BeamViewModel>();

        private string selectedBeamId;
        public string SelectedBeamId
        {
            get { return selectedBeamId; }
            set { SetProperty(ref selectedBeamId, value); }
        }

        private bool isOk = false;
        public bool IsOk
        {
            get { return isOk; }
            set { SetProperty(ref isOk, value); }
        }

        private bool isDifferentShift = false;
        public bool IsDifferentShift
        {
            get { return isDifferentShift; }
            set { SetProperty(ref isDifferentShift, value); }
        }

        public DelegateCommand OkCommand { get; private set; }
        public DelegateCommand CancelCommand { get; private set; }

        public MlcShifterViewModel()
        {
            List<string> beamIds = new List<string>{ "Beam1", "Beam2", "Beam3" };

            foreach(var beamId in beamIds)
            {
                BeamViewModels.Add(new BeamViewModel(beamId));
            }

            OkCommand = new DelegateCommand(() => { IsOk = true; });
            CancelCommand = new DelegateCommand(() => { IsOk = false; });
        }

        public MlcShifterViewModel(List<BeamViewModel> beamViewModels)
        {
            BeamViewModels = new ObservableCollection<BeamViewModel>(beamViewModels);
            OkCommand = new DelegateCommand(() => { IsOk = true; });
            CancelCommand = new DelegateCommand(() => { IsOk = false; });
        }
    }
}
