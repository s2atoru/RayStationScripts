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

        public RoiSelectionViewModel(List<Models.Roi> rois)
        {
            Rois = new ObservableCollection<Models.Roi>(rois);

            RoiViewModels = Rois.ToReadOnlyReactiveCollection(x => new RoiViewModel(x));
        }
    }
}
