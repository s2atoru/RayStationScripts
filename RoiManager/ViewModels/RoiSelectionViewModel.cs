using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Media;

namespace RoiManager.ViewModels
{
    public class RoiSelectionViewModel : BindableBase
    {
        public ObservableCollection<Models.Roi> Rois;
        public ObservableCollection<ViewModels.RoiViewModel> RoiViewModels { get; } = new ObservableCollection<RoiViewModel>();

        public DelegateCommand OkCommand { get; private set; }
        public DelegateCommand CancelCommand { get; private set; }

        public DelegateCommand<string> SelectColorCommand { get; private set; }

        public bool CanExecute { get; set; } = false;

        public List<string> RoiNameList { get; set; } = new List<string>();
        public Dictionary<string, string> RoiNameMappingTable { get; set; } = new Dictionary<string, string>();

        public List<string> RoiTypeList { get; set; } = Enum.GetNames(typeof(Models.RoiType)).ToList();

        public RoiSelectionViewModel(List<Models.Roi> rois)
        {
            Rois = new ObservableCollection<Models.Roi>(rois);

            foreach (var r in Rois)
            {
                RoiViewModels.Add(new RoiViewModel(r));
            }

            OkCommand = new DelegateCommand(() => { CanExecute = true; });
            CancelCommand = new DelegateCommand(() => { CanExecute = false; });
        }

        public RoiSelectionViewModel(List<Models.Roi> rois, Dictionary<string,string> roiNameMappingTable)
        {

            RoiNameMappingTable = roiNameMappingTable;
            RoiNameList = roiNameMappingTable.Keys.ToList();

            Rois = new ObservableCollection<Models.Roi>(rois);

            foreach (var r in Rois)
            {
                var roiViewModel = new RoiViewModel(r);
                if (RoiNameMappingTable.ContainsKey(roiViewModel.Name.Value))
                {
                    roiViewModel.NewName.Value = RoiNameMappingTable[roiViewModel.Name.Value];
                }
                RoiViewModels.Add(roiViewModel);
            }

            OkCommand = new DelegateCommand(() => { CanExecute = true; });
            CancelCommand = new DelegateCommand(() => { CanExecute = false; });
        }
    }
}
