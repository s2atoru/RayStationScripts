using Prism.Commands;
using Prism.Mvvm;
using Reactive.Bindings;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace RoiManager.ViewModels
{
    public class RoiSelectionViewModel : BindableBase
    {
        public ObservableCollection<Models.Roi> Rois;
        public ReadOnlyReactiveCollection<ViewModels.RoiViewModel> RoiViewModels { get; }

        public DelegateCommand OkCommand { get; private set; }
        public DelegateCommand CancelCommand { get; private set; }

        public bool CanExecute { get; set; } = false;

        public List<string> RoiNameList { get; set; } = new List<string>(); 

        public RoiSelectionViewModel(List<Models.Roi> rois)
        {
            Rois = new ObservableCollection<Models.Roi>(rois);

            RoiViewModels = Rois.ToReadOnlyReactiveCollection(x => new RoiViewModel(x));

            OkCommand = new DelegateCommand(() => { CanExecute = true; });
            CancelCommand = new DelegateCommand(() => { CanExecute = false; });
        }
    }
}
