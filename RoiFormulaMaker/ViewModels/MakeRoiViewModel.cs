using Prism.Mvvm;
using RoiFormulaMaker.Notifications;
using RoiFormulaMaker.Models;
using System.Collections.ObjectModel;
using MvvmCommon.ViewModels;
using Prism.Commands;

namespace RoiFormulaMaker.ViewModels
{
    class MakeRoiViewModel : BindableBase
    {
        private bool isChecked;
        public bool IsChecked
        {
            get { return isChecked; }
            set { SetProperty(ref isChecked, value); }
        }

        private MainWindowViewModel mainWindowViewModel;
        public MainWindowViewModel MainWindowViewModel
        {
            get { return mainWindowViewModel; }
            set { SetProperty(ref mainWindowViewModel, value); }
        }

        public dynamic structureFormula;
        public dynamic StructureFormula
        {
            get { return structureFormula; }
            set
            {
                structureFormula = value;
            }
        }

        public DelegateCommand MakeRoiCommand { get; set; }

        public MakeRoiViewModel(dynamic structureFormula, MainWindowViewModel mainWindowViewModel)
        {
            StructureFormula = structureFormula;
            MainWindowViewModel = mainWindowViewModel;
            IsChecked = false;

            switch (StructureFormula.FormulaType)
            {
                case "RingRoi":
                    MakeRoiCommand = new DelegateCommand(RaiseMakeRingRoiInteraction);
                    break;
                case "MarginAddedRoi":
                    MakeRoiCommand = new DelegateCommand(RaiseMakeMarginAddedRoiInteraction);
                    break;
                case "OverlappedRoi":
                    MakeRoiCommand = new DelegateCommand(RaiseMakeOverlappedRoiInteraction);
                    break;
                case "RoiSubtractedRoi":
                    MakeRoiCommand = new DelegateCommand(RaiseMakeRoiSubtractedRoiInteraction);
                    break;
                default:
                    break;
            }
        }

        private void RaiseMakeRingRoiInteraction()
        {
            MainWindowViewModel.MakeRingRoiRequest.Raise(new MakeRingRoiNotification
            {
                Title = "Make Ring ROI",
                StructureNames = MainWindowViewModel.StructureNames,
                ContouredStructureNames = MainWindowViewModel.ContouredStructureNames,
                StructureTypes = MainWindowViewModel.StructureTypes,
                StructureName = ((RingRoiParameters)StructureFormula).StructureName,
                StructureType = ((RingRoiParameters)StructureFormula).StructureType,
                BaseStructureName = ((RingRoiParameters)StructureFormula).BaseStructureName,
                OuterMargin = ((RingRoiParameters)StructureFormula).OuterMargin,
                InnerMargin = ((RingRoiParameters)StructureFormula).InnerMargin
            },
            r =>
            {
                if (r.Confirmed && r.BaseStructureName != null)
                {
                    MainWindowViewModel.Message = $"User selected: { r.BaseStructureName}";

                    ((RingRoiParameters)StructureFormula).StructureName = r.StructureName;
                    ((RingRoiParameters)StructureFormula).StructureType = r.StructureType;
                    ((RingRoiParameters)StructureFormula).BaseStructureName = r.BaseStructureName;
                    ((RingRoiParameters)StructureFormula).OuterMargin = r.OuterMargin;
                    ((RingRoiParameters)StructureFormula).InnerMargin = r.InnerMargin;

                    MainWindowViewModel.UpdateStructureNames(r.StructureName);

                    var structureDescription = ((RingRoiParameters)StructureFormula).ToString();
                }
                else
                    MainWindowViewModel.Message = "User canceled or didn't select structure";
            });
        }

        private void RaiseMakeRoiSubtractedRoiInteraction()
        {
            MainWindowViewModel.MakeRoiSubtractedRoiRequest.Raise(new MakeRoiSubtractedRoiNotification
            {
                Title = "Make ROI Subtracted ROI",
                StructureNames = MainWindowViewModel.StructureNames,
                ContouredStructureNames = MainWindowViewModel.ContouredStructureNames,
                StructureTypes = MainWindowViewModel.StructureTypes,
                StructureName = ((RoiSubtractedRoiParameters)StructureFormula).StructureName,
                StructureType = ((RoiSubtractedRoiParameters)StructureFormula).StructureType,
                BaseStructureName = ((RoiSubtractedRoiParameters)StructureFormula).BaseStructureName,
                SubtractedRoiName = ((RoiSubtractedRoiParameters)StructureFormula).SubtractedRoiName,
                Margin = ((RoiSubtractedRoiParameters)StructureFormula).Margin
            },
            r =>
            {
                if (r.Confirmed && r.BaseStructureName != null && r.SubtractedRoiName != null)
                {
                    MainWindowViewModel.Message = $"User selected: Base => { r.BaseStructureName}, Subtracted ROI => {r.SubtractedRoiName}";

                    ((RoiSubtractedRoiParameters)StructureFormula).StructureName = r.StructureName;
                    ((RoiSubtractedRoiParameters)StructureFormula).StructureType = r.StructureType;
                    ((RoiSubtractedRoiParameters)StructureFormula).BaseStructureName = r.BaseStructureName;
                    ((RoiSubtractedRoiParameters)StructureFormula).SubtractedRoiName = r.SubtractedRoiName;
                    ((RoiSubtractedRoiParameters)StructureFormula).Margin = r.Margin;
                                        
                    MainWindowViewModel.UpdateStructureNames(r.StructureName);
                }
                else
                    MainWindowViewModel.Message = "User canceled or didn't select structures";
            });
        }

        private void RaiseMakeMarginAddedRoiInteraction()
        {

            var contouredStructureList = new ObservableCollection<ListBoxItemViewModel>();

            foreach (var c in MainWindowViewModel.ContouredStructureNames)
            {
                bool isSelected = false;
                if (((MarginAddedRoiParameters)StructureFormula).BaseStructureNames.Contains(c))
                {
                    isSelected = true;
                }
                contouredStructureList.Add(new ListBoxItemViewModel { Name = c, IsSelected = isSelected });
            }

            MainWindowViewModel.MakeMarginAddedRoiRequest.Raise(new MakeMarginAddedRoiNotification
            {
                Title = "Make Margin Added ROI",
                StructureNames = MainWindowViewModel.StructureNames,
                ContouredStructureList = contouredStructureList,
                StructureTypes = MainWindowViewModel.StructureTypes,
                StructureName = ((MarginAddedRoiParameters)StructureFormula).StructureName,
                StructureType = ((MarginAddedRoiParameters)StructureFormula).StructureType,
                BaseStructureNames = ((MarginAddedRoiParameters)StructureFormula).BaseStructureNames,
                Margin = ((MarginAddedRoiParameters)StructureFormula).Margin
            },
            r =>
            {
                if (r.Confirmed && r.BaseStructureNames != null && r.BaseStructureNames.Count > 0)
                {
                    MainWindowViewModel.Message = $"User selected: Base => { string.Join(", ", r.BaseStructureNames) }";

                    ((MarginAddedRoiParameters)StructureFormula).StructureName = r.StructureName;
                    ((MarginAddedRoiParameters)StructureFormula).StructureType = r.StructureType;
                    ((MarginAddedRoiParameters)StructureFormula).BaseStructureNames = r.BaseStructureNames;
                    ((MarginAddedRoiParameters)StructureFormula).Margin = r.Margin;

                    MainWindowViewModel.UpdateStructureNames(r.StructureName);
                }
                else
                    MainWindowViewModel.Message = "User canceled or didn't select structures";
            });
        }

        private void RaiseMakeOverlappedRoiInteraction()
        {

            var contouredStructureList = new ObservableCollection<ListBoxItemViewModel>();

            foreach (var c in MainWindowViewModel.ContouredStructureNames)
            {
                bool isSelected = false;
                if (((OverlappedRoiParameters)StructureFormula).BaseStructureNames.Contains(c)){
                    isSelected = true;
                }
                contouredStructureList.Add(new ListBoxItemViewModel { Name = c, IsSelected = isSelected });
            }

            MainWindowViewModel.MakeOverlappedRoiRequest.Raise(new MakeOverlappedRoiNotification
            {
                Title = "Make Overlapped ROI",
                StructureNames = MainWindowViewModel.StructureNames,
                ContouredStructureList = contouredStructureList,
                StructureTypes = MainWindowViewModel.StructureTypes,
                StructureName = ((OverlappedRoiParameters)StructureFormula).StructureName,
                StructureType = ((OverlappedRoiParameters)StructureFormula).StructureType,
                BaseStructureNames = ((OverlappedRoiParameters)StructureFormula).BaseStructureNames,
                Margin = ((OverlappedRoiParameters)StructureFormula).Margin
            },
            r =>
            {
                if (r.Confirmed && r.BaseStructureNames != null && r.BaseStructureNames.Count > 0)
                {
                    MainWindowViewModel.Message = $"User selected: Base => { string.Join(", ", r.BaseStructureNames) }";

                    ((OverlappedRoiParameters)StructureFormula).StructureName = r.StructureName;
                    ((OverlappedRoiParameters)StructureFormula).StructureType = r.StructureType;
                    ((OverlappedRoiParameters)StructureFormula).BaseStructureNames = r.BaseStructureNames;
                    ((OverlappedRoiParameters)StructureFormula).Margin = r.Margin;

                    MainWindowViewModel.UpdateStructureNames(r.StructureName);
                }
                else
                    MainWindowViewModel.Message = "User canceled or didn't select structures";
            });
        }
    }
}
