using Prism.Mvvm;
using RoiFormulaMaker.Notifications;
using RoiFormulaMaker.Models;
using System.Collections.ObjectModel;
using MvvmCommon.ViewModels;
using Prism.Commands;
using System.Linq;

namespace RoiFormulaMaker.ViewModels
{
    public class MakeRoiViewModel : BindableBase
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

        private string structureDescription;
        public string StructureDescription
        {
            get { return structureDescription; }
            set { SetProperty(ref structureDescription, value); }
        }

        public DelegateCommand MakeRoiCommand { get; set; }

        public MakeRoiViewModel(dynamic structureFormula, MainWindowViewModel mainWindowViewModel)
        {
            StructureFormula = structureFormula;
            MainWindowViewModel = mainWindowViewModel;
            IsChecked = false;
            StructureDescription = StructureFormula.ToString();

            switch (StructureFormula.FormulaType)
            {
                case "RingRoi":
                    MakeRoiCommand = new DelegateCommand(RaiseMakeRingRoiInteraction);
                    break;
                case "WallRoi":
                    MakeRoiCommand = new DelegateCommand(RaiseMakeWallRoiInteraction);
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
            var structureFormula = (RingRoiParameters)StructureFormula;

            MainWindowViewModel.MakeRingRoiRequest.Raise(new MakeRingRoiNotification
            {
                Title = "Make Ring ROI",
                StructureNames = MainWindowViewModel.StructureNames,
                ContouredStructureNames = MainWindowViewModel.ContouredStructureNames,
                StructureTypes = MainWindowViewModel.StructureTypes,
                StructureName = structureFormula.StructureName,
                StructureType = structureFormula.StructureType,
                BaseStructureName = structureFormula.BaseStructureName,
                OuterMargin = structureFormula.OuterMargin,
                InnerMargin = structureFormula.InnerMargin
            },
            r =>
            {
                if (r.Confirmed && r.BaseStructureName != null)
                {
                    MainWindowViewModel.Message = $"User selected: { r.BaseStructureName}";

                    structureFormula.StructureName = r.StructureName;
                    structureFormula.StructureType = r.StructureType;
                    structureFormula.BaseStructureName = r.BaseStructureName;
                    structureFormula.OuterMargin = r.OuterMargin;
                    structureFormula.InnerMargin = r.InnerMargin;

                    StructureDescription = structureFormula.ToString();

                    MainWindowViewModel.UpdateStructureNames(r.StructureName);
                    MainWindowViewModel.UpdateStructureDescriptions();
                }
                else
                    MainWindowViewModel.Message = "User canceled or didn't select structure";
            });
        }

        private void RaiseMakeWallRoiInteraction()
        {
            var structureFormula = (WallRoiParameters)StructureFormula;

            MainWindowViewModel.MakeWallRoiRequest.Raise(new MakeWallRoiNotification
            {
                Title = "Make Wall ROI",
                StructureNames = MainWindowViewModel.StructureNames,
                ContouredStructureNames = MainWindowViewModel.ContouredStructureNames,
                StructureTypes = MainWindowViewModel.StructureTypes,
                StructureName = structureFormula.StructureName,
                StructureType = structureFormula.StructureType,
                BaseStructureName = structureFormula.BaseStructureName,
                OuterMargin = structureFormula.OuterMargin,
                InnerMargin = structureFormula.InnerMargin
            },
            r =>
            {
                if (r.Confirmed && r.BaseStructureName != null)
                {
                    MainWindowViewModel.Message = $"User selected: { r.BaseStructureName}";

                    structureFormula.StructureName = r.StructureName;
                    structureFormula.StructureType = r.StructureType;
                    structureFormula.BaseStructureName = r.BaseStructureName;
                    structureFormula.OuterMargin = r.OuterMargin;
                    structureFormula.InnerMargin = r.InnerMargin;

                    StructureDescription = structureFormula.ToString();

                    MainWindowViewModel.UpdateStructureNames(r.StructureName);
                    MainWindowViewModel.UpdateStructureDescriptions();
                }
                else
                    MainWindowViewModel.Message = "User canceled or didn't select structure";
            });
        }

        private void RaiseMakeRoiSubtractedRoiInteraction()
        {
            var structureFormula = (RoiSubtractedRoiParameters)StructureFormula;
            var contouredStructureList = new ObservableCollection<ListBoxItemViewModel>();

            foreach (var c in MainWindowViewModel.ContouredStructureNames)
            {
                bool isSelected = structureFormula.SubtractedRoiNames.Contains(c);
                contouredStructureList.Add(new ListBoxItemViewModel { Name = c, IsSelected = isSelected });
            }

            MainWindowViewModel.MakeRoiSubtractedRoiRequest.Raise(new MakeRoiSubtractedRoiNotification
            {
                Title = "Make ROIs Subtracted ROI",
                StructureNames = MainWindowViewModel.StructureNames,
                ContouredStructureNames = MainWindowViewModel.ContouredStructureNames,
                StructureTypes = MainWindowViewModel.StructureTypes,
                StructureName = StructureFormula.StructureName,
                StructureType = StructureFormula.StructureType,
                BaseStructureName = StructureFormula.BaseStructureName,
                ContouredStructureList = contouredStructureList,
                SubtractedRoiNames = StructureFormula.SubtractedRoiNames,
                Margin = StructureFormula.Margin
            },
            r =>
            {
                if (r.Confirmed && r.BaseStructureName != null && r.SubtractedRoiNames != null)
                {
                    MainWindowViewModel.Message = $"User selected: Base => { r.BaseStructureName}, Subtracted ROIs => {string.Join(", ", r.SubtractedRoiNames)}";

                    structureFormula.StructureName = r.StructureName;
                    structureFormula.StructureType = r.StructureType;
                    structureFormula.BaseStructureName = r.BaseStructureName;
                    structureFormula.SubtractedRoiNames = r.SubtractedRoiNames;
                    structureFormula.Margin = r.Margin;

                    StructureDescription = structureFormula.ToString();

                    MainWindowViewModel.UpdateStructureNames(r.StructureName);
                    MainWindowViewModel.UpdateStructureDescriptions();
                }
                else
                    MainWindowViewModel.Message = "User canceled or didn't select structures";
            });
        }

        private void RaiseMakeMarginAddedRoiInteraction()
        {
            var structureFormula = (MarginAddedRoiParameters)StructureFormula;
            var contouredStructureList = new ObservableCollection<ListBoxItemViewModel>();

            foreach (var c in MainWindowViewModel.ContouredStructureNames)
            {
                bool isSelected = structureFormula.BaseStructureNames.Contains(c);
                contouredStructureList.Add(new ListBoxItemViewModel { Name = c, IsSelected = isSelected });
            }

            MainWindowViewModel.MakeMarginAddedRoiRequest.Raise(new MakeMarginAddedRoiNotification
            {
                Title = "Make Margin Added ROI",
                StructureNames = MainWindowViewModel.StructureNames,
                ContouredStructureList = contouredStructureList,
                StructureTypes = MainWindowViewModel.StructureTypes,
                StructureName = structureFormula.StructureName,
                StructureType = structureFormula.StructureType,
                BaseStructureNames = structureFormula.BaseStructureNames,
                Margin = structureFormula.Margin
            },
            r =>
            {
                if (r.Confirmed && r.BaseStructureNames != null && r.BaseStructureNames.Count > 0)
                {
                    MainWindowViewModel.Message = $"User selected: Base => { string.Join(", ", r.BaseStructureNames) }";

                    structureFormula.StructureName = r.StructureName;
                    structureFormula.StructureType = r.StructureType;
                    structureFormula.BaseStructureNames = r.BaseStructureNames;
                    structureFormula.Margin = r.Margin;

                    StructureDescription = structureFormula.ToString();

                    MainWindowViewModel.UpdateStructureNames(r.StructureName);
                    MainWindowViewModel.UpdateStructureDescriptions();
                }
                else
                    MainWindowViewModel.Message = "User canceled or didn't select structures";
            });
        }

        private void RaiseMakeOverlappedRoiInteraction()
        {
            var structureFormula = (OverlappedRoiParameters)StructureFormula;

            var contouredStructureList = new ObservableCollection<ListBoxItemViewModel>();

            foreach (var c in MainWindowViewModel.ContouredStructureNames)
            {
                bool isSelected = structureFormula.BaseStructureNames.Contains(c);
                contouredStructureList.Add(new ListBoxItemViewModel { Name = c, IsSelected = isSelected });
            }

            MainWindowViewModel.MakeOverlappedRoiRequest.Raise(new MakeOverlappedRoiNotification
            {
                Title = "Make Overlapped ROI",
                StructureNames = MainWindowViewModel.StructureNames,
                ContouredStructureList = contouredStructureList,
                StructureTypes = MainWindowViewModel.StructureTypes,
                StructureName = structureFormula.StructureName,
                StructureType = structureFormula.StructureType,
                BaseStructureNames = structureFormula.BaseStructureNames,
                Margin = structureFormula.Margin
            },
            r =>
            {
                if (r.Confirmed && r.BaseStructureNames != null && r.BaseStructureNames.Count > 0)
                {
                    MainWindowViewModel.Message = $"User selected: Base => { string.Join(", ", r.BaseStructureNames) }";

                    structureFormula.StructureName = r.StructureName;
                    structureFormula.StructureType = r.StructureType;
                    structureFormula.BaseStructureNames = r.BaseStructureNames;
                    structureFormula.Margin = r.Margin;

                    StructureDescription = structureFormula.ToString();

                    MainWindowViewModel.UpdateStructureNames(r.StructureName);
                    MainWindowViewModel.UpdateStructureDescriptions();
                }
                else
                    MainWindowViewModel.Message = "User canceled or didn't select structures";
            });
        }
    }
}
