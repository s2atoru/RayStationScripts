using Prism.Commands;
using System.Collections.Generic;

namespace RoiManager.ViewModels
{
    public class ExaminationSelectionViewModel
    {
        public bool CanExecute { get; set; } = false;
        public List<string> ExaminationNames { get; set; } = new List<string>();
        public string SelectedExamination { get; set; }

        public DelegateCommand OkCommand { get; private set; }
        public DelegateCommand CancelCommand { get; private set; }

        public ExaminationSelectionViewModel()
        {
            OkCommand = new DelegateCommand(() => { CanExecute = true; });
            CancelCommand = new DelegateCommand(() => { CanExecute = false; });
        }
    }
}
