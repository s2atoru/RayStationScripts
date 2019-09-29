using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using MvvmCommon.ViewModels;
using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using RoiFormulaMaker.Models;
using RoiFormulaMaker.Notifications;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;

namespace RoiFormulaMaker.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {

        private string title = "Make composite ROIs";
        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        private string message;
        public string Message
        {
            get { return message; }
            set { SetProperty(ref message, value); }
        }

        private string filePath;
        public string FilePath
        {
            get { return filePath; }
            set { SetProperty(ref filePath, value); }
        }

        private string savedFilePath;
        public string SavedFilePath
        {
            get { return savedFilePath; }
            set { SetProperty(ref savedFilePath, value); }
        }

        private string description;
        public string Description
        {
            get { return description; }
            set { SetProperty(ref description, value); }
        }

        private string defaultDirectoryPath;
        public string DefaultDirectoryPath
        {
            get { return defaultDirectoryPath; }
            set { SetProperty(ref defaultDirectoryPath, value); }
        }

        public static readonly string DefaultFileName = "_last_roi_formulas.json";

        private Dictionary<string, Dictionary<string, object>> structureDetails = new Dictionary<string, Dictionary<string, object>>();
        public Dictionary<string, Dictionary<string, object>> StructureDetails
        {
            get { return structureDetails; }
            set
            {
                structureDetails = value;
                StructureNames = new ObservableCollection<string>(structureDetails.Keys);

                var contouredStructureNames = new List<string>();
                foreach (var s in structureDetails)
                {
                    if ((bool)s.Value["HasContours"])
                    {
                        contouredStructureNames.Add(s.Key);
                    }
                }
                ContouredStructureNames = new ObservableCollection<string>(contouredStructureNames);

                SortStructureNames();
            }
        }

        public ObservableCollection<string> StructureNames { get; set; } = new ObservableCollection<string> { "PTV", "Rectum", "Bladder", "PTV2", "CTV1", "CTV2", "GTV1", "GTV2" };
        public ObservableCollection<string> ContouredStructureNames { get; set; } = new ObservableCollection<string> { "PTV", "Rectum", "PTV2", "CTV1", "CTV2", "GTV1", "GTV2" };

        public ObservableCollection<string> StructureTypes { get; set; } = new ObservableCollection<string>
        {
            "Control",                  //ROI to be used in control of dose optimization and calculation.
            "Ptv",                     //Planning target volume (as defined in ICRU50).
            "Ctv",                     //Clinical target volume (as defined in ICRU50).
            "Gtv",                     //Gross tumor volume (as defined in ICRU50).
            "Organ",                    //Patient organ.
            "Avoidance",                //Region in which dose is to be minimized.
            "External",                //External patient contour.
            "Support",                  //External patient support device.
            "TreatedVolume",            //Treated volume (as defined in ICRU50).
            "IrradiatedVolume",         //Irradiated Volume (as defined in ICRU50).
            "Bolus",                    //Patient bolus to be used for external beam therapy.
            "Marker",                   //Patient marker or marker on a localizer.
            "Registration",             //Registration ROI
            "Isocenter",                //Treatment isocenter to be used for external beam therapy.
            "ContrastAgent",            //Volume into which a contrast agent has been injected.
            "Cavity",                   //Patient anatomical cavity.
            "BrachyChannel",            //Branchy therapy channel
            "BrachyAccessory",          //Brachy therapy accessory device.
            "BrachySourceApplicator",   //Brachy therapy source applicator.
            "BrachyChannelShield",      //Brachy therapy channel shield.
            "Fixation",                 //External patient fixation or immobilisation device.
            "DoseRegion",               //ROI to be used as a dose reference.
            "FieldOfView",              //ROI to be used for representing the Field-of-view in, e.g., a cone beam CT image.
            "AcquisitionIsocenter",     //Acquisition isocenter, the position during acquisition.
            "InitialLaserIsocenter",    //Initial laser isocenter, the position before acquisition.
            "InitialMatchIsocenter",     //Initial match isocenter, the position after acquisition.
            "Undefined"
        };

        public RoiFormulas RoiFormulas { get; set; } = new RoiFormulas();

        public InteractionRequest<MakeRingRoiNotification> MakeRingRoiRequest { get; set; }
        public DelegateCommand MakeRingRoiCommand { get; set; }

        public InteractionRequest<MakeRoiSubtractedRoiNotification> MakeRoiSubtractedRoiRequest { get; set; }
        public DelegateCommand MakeRoiSubtractedRoiCommand { get; set; }

        public InteractionRequest<MakeMarginAddedRoiNotification> MakeMarginAddedRoiRequest { get; set; }
        public DelegateCommand MakeMarginAddedRoiCommand { get; set; }

        public InteractionRequest<MakeOverlappedRoiNotification> MakeOverlappedRoiRequest { get; set; }
        public DelegateCommand MakeOverlappedRoiCommand { get; set; }

        public List<dynamic> structureFormulas = new List<dynamic>();
        public List<dynamic> StructureFormulas
        {
            get { return structureFormulas; }
            set
            {
                structureFormulas = value;
                UpdateStructureDescriptions();
            }
        }

        public ObservableCollection<MakeRoiViewModel> MakeRoiViewModels { get; set; } = new ObservableCollection<MakeRoiViewModel>();

        public void UpdateStructureDescriptions()
        {
            StructureDescriptions = string.Empty;
            foreach (var sf in StructureFormulas)
            {
                var structureDescription = sf.ToString();
                if (string.IsNullOrEmpty(StructureDescriptions))
                {
                    StructureDescriptions = structureDescription;
                }
                else
                {
                    StructureDescriptions += "\n" + structureDescription;
                }
            }
        }

        public void SortStructureNames()
        {
            StructureNames = new ObservableCollection<string>(StructureNames.OrderBy(s => s));
            ContouredStructureNames = new ObservableCollection<string>(ContouredStructureNames.OrderBy(s => s));
        }

        private string structureDescriptions;
        public string StructureDescriptions
        {
            get { return structureDescriptions; }
            set { SetProperty(ref structureDescriptions, value); }
        }

        public DelegateCommand OkCommand { get; private set; }
        public DelegateCommand CancelCommand { get; private set; }

        public DelegateCommand ChooseFileCommand { get; private set; }
        public DelegateCommand SaveFileCommand { get; private set; }

        public DelegateCommand DeleteStructureFormulasCommand { get; private set; }

        public MainWindowViewModel()
        {

            MakeRingRoiRequest = new InteractionRequest<MakeRingRoiNotification>();
            MakeRingRoiCommand = new DelegateCommand(RaiseMakeRingRoiInteraction);

            MakeRoiSubtractedRoiRequest = new InteractionRequest<MakeRoiSubtractedRoiNotification>();
            MakeRoiSubtractedRoiCommand = new DelegateCommand(RaiseMakeRoiSubtractedRoiInteraction);

            MakeMarginAddedRoiRequest = new InteractionRequest<MakeMarginAddedRoiNotification>();
            MakeMarginAddedRoiCommand = new DelegateCommand(RaiseMakeMarginAddedRoiInteraction);

            MakeOverlappedRoiRequest = new InteractionRequest<MakeOverlappedRoiNotification>();
            MakeOverlappedRoiCommand = new DelegateCommand(RaiseMakeOverlappedRoiInteraction);

            OkCommand = new DelegateCommand(() => { RoiFormulas.CanExecute = true; RoiFormulas.WriteToFile(Path.Combine(DefaultDirectoryPath, DefaultFileName)); });
            CancelCommand = new DelegateCommand(() => { RoiFormulas.CanExecute = false; });

            ChooseFileCommand = new DelegateCommand(ChooseFile);
            SaveFileCommand = new DelegateCommand(SaveFile);

            DeleteStructureFormulasCommand = new DelegateCommand(DeleteStructureFromulas);
        }

        public void UpdateStructureNames(string structureName)
        {
            if (!StructureNames.Contains(structureName))
            {
                StructureNames.Add(structureName);
            }

            if (!ContouredStructureNames.Contains(structureName))
            {
                ContouredStructureNames.Add(structureName);
            }

            SortStructureNames();
        }

        private void RaiseMakeRingRoiInteraction()
        {
            MakeRingRoiRequest.Raise(new MakeRingRoiNotification
            {
                Title = "Make Ring ROI",
                OuterMargin = 15,
                InnerMargin = 0,
                StructureNames = this.StructureNames,
                ContouredStructureNames = this.ContouredStructureNames,
                StructureTypes = this.StructureTypes
            },
            r =>
            {
                if (r.Confirmed && r.BaseStructureName != null)
                {
                    Message = $"User selected: { r.BaseStructureName}";
                    var ringRoiParameters = new RingRoiParameters
                    {
                        StructureName = r.StructureName,
                        StructureType = r.StructureType,
                        BaseStructureName = r.BaseStructureName,
                        OuterMargin = r.OuterMargin,
                        InnerMargin = r.InnerMargin
                    };
                    if (StructureFormulas.Contains(ringRoiParameters))
                    {
                        Message = "The same ring is already in the list";
                        return;
                        ;
                    }

                    UpdateStructureNames(r.StructureName);

                    StructureFormulas.Add(ringRoiParameters);
                    UpdateStructureDescriptions();

                    MakeRoiViewModels.Add(new MakeRoiViewModel(ringRoiParameters, this));
                }
                else
                    Message = "User canceled or didn't select structure";
            });
        }

        private void RaiseMakeRoiSubtractedRoiInteraction()
        {
            MakeRoiSubtractedRoiRequest.Raise(new MakeRoiSubtractedRoiNotification
            {
                Title = "Make ROI Subtracted ROI",
                Margin = 0,
                StructureNames = this.StructureNames,
                ContouredStructureNames = this.ContouredStructureNames,
                StructureTypes = this.StructureTypes
            },
            r =>
            {
                if (r.Confirmed && r.BaseStructureName != null && r.SubtractedRoiName != null)
                {
                    Message = $"User selected: Base => { r.BaseStructureName}, Subtracted ROI => {r.SubtractedRoiName}";
                    var roiSubtractedRoiParameters = new RoiSubtractedRoiParameters
                    {
                        StructureName = r.StructureName,
                        StructureType = r.StructureType,
                        BaseStructureName = r.BaseStructureName,
                        SubtractedRoiName = r.SubtractedRoiName,
                        Margin = r.Margin
                    };
                    if (StructureFormulas.Contains(roiSubtractedRoiParameters))
                    {
                        Message = "The same ROI subtracted ROI is already in the list";
                        return;
                        ;
                    }

                    UpdateStructureNames(r.StructureName);

                    StructureFormulas.Add(roiSubtractedRoiParameters);
                    UpdateStructureDescriptions();

                    MakeRoiViewModels.Add(new MakeRoiViewModel(roiSubtractedRoiParameters, this));
                }
                else
                    Message = "User canceled or didn't select structures";
            });
        }

        private void RaiseMakeMarginAddedRoiInteraction()
        {

            var contouredStructureList = new ObservableCollection<ListBoxItemViewModel>();

            foreach (var c in ContouredStructureNames)
            {
                contouredStructureList.Add(new ListBoxItemViewModel { Name = c, IsSelected = false });
            }

            MakeMarginAddedRoiRequest.Raise(new MakeMarginAddedRoiNotification
            {
                Title = "Make Margin Added ROI",
                Margin = 0,
                StructureNames = this.StructureNames,
                ContouredStructureList = contouredStructureList,
                StructureTypes = this.StructureTypes
            },
            r =>
            {
                if (r.Confirmed && r.BaseStructureNames != null && r.BaseStructureNames.Count > 0)
                {
                    Message = $"User selected: Base => { string.Join(", ", r.BaseStructureNames) }";
                    var marginAddedRoiParameters = new MarginAddedRoiParameters
                    {
                        StructureName = r.StructureName,
                        StructureType = r.StructureType,
                        BaseStructureNames = r.BaseStructureNames,
                        Margin = r.Margin
                    };
                    if (StructureFormulas.Contains(marginAddedRoiParameters))
                    {
                        Message = "The margin added ROI is already in the list";
                        return;
                    }

                    UpdateStructureNames(r.StructureName);

                    StructureFormulas.Add(marginAddedRoiParameters);
                    UpdateStructureDescriptions();

                    MakeRoiViewModels.Add(new MakeRoiViewModel(marginAddedRoiParameters, this));
                }
                else
                    Message = "User canceled or didn't select structures";
            });
        }

        private void RaiseMakeOverlappedRoiInteraction()
        {

            var contouredStructureList = new ObservableCollection<ListBoxItemViewModel>();

            foreach (var c in ContouredStructureNames)
            {
                contouredStructureList.Add(new ListBoxItemViewModel { Name = c, IsSelected = false });
            }

            MakeOverlappedRoiRequest.Raise(new MakeOverlappedRoiNotification
            {
                Title = "Make Overlapped ROI",
                Margin = 0,
                StructureNames = this.StructureNames,
                ContouredStructureList = contouredStructureList,
                StructureTypes = this.StructureTypes
            },
            r =>
            {
                if (r.Confirmed && r.BaseStructureNames != null && r.BaseStructureNames.Count > 0)
                {
                    Message = $"User selected: Base => { string.Join(", ", r.BaseStructureNames) }";
                    var overlappedRoiParameters = new OverlappedRoiParameters
                    {
                        StructureName = r.StructureName,
                        StructureType = r.StructureType,
                        BaseStructureNames = r.BaseStructureNames,
                        Margin = r.Margin
                    };
                    if (StructureFormulas.Contains(overlappedRoiParameters))
                    {
                        Message = "The overlapped ROI is already in the list";
                        return;
                    }

                    UpdateStructureNames(r.StructureName);
                    
                    StructureFormulas.Add(overlappedRoiParameters);
                    UpdateStructureDescriptions();

                    MakeRoiViewModels.Add(new MakeRoiViewModel(overlappedRoiParameters, this));
                }
                else
                    Message = "User canceled or didn't select structures";
            });
        }

        private void ChooseFile()
        {
            //var dialog = new OpenFileDialog();
            //dialog.Title = "Choose file";
            ////dialog.Filter = "All files(*.*)|*.*";
            //if (dialog.ShowDialog() == true)
            //{
            //    FilePath = dialog.FileName;
            //}
            //else
            //{
            //    Message = "\"Choose file\" is canceled";
            //}

            var dialog = new CommonOpenFileDialog("Choose File");

            if (Directory.Exists(DefaultDirectoryPath))
            {
                dialog.InitialDirectory = DefaultDirectoryPath;
            }

            dialog.IsFolderPicker = false;

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                FilePath = dialog.FileName;

                var roiFormulas = new Models.RoiFormulas() { Formulas = StructureFormulas };
                // StructureFromulas will be cleared in roiFormulas.ReadFromFile
                roiFormulas.ReadFromFile(FilePath);

                Description = roiFormulas.Description;
                UpdateStructureDescriptions();

                MakeRoiViewModels.Clear();
                foreach (var sf in StructureFormulas)
                {
                    MakeRoiViewModels.Add(new MakeRoiViewModel(sf, this));
                    UpdateStructureNames(sf.StructureName);
                }
            }
            else
            {
                Message = "\"Choose File\" is canceled";
            }
        }

        private void SaveFile()
        {
            var dialog = new SaveFileDialog();
            dialog.Title = "Save to File";

            if (Directory.Exists(DefaultDirectoryPath))
            {
                dialog.InitialDirectory = DefaultDirectoryPath;
            }

            //dialog.Filter = "text file|*.txt";
            if (dialog.ShowDialog() == true)
            {
                SavedFilePath = dialog.FileName;
                var roiFormulas = new Models.RoiFormulas { Description = Description, Formulas = StructureFormulas };
                roiFormulas.WriteToFile(SavedFilePath);
            }
            else
            {
                Message = "\"Save to File\" is canceled";
            }
        }

        private void DeleteStructureFromulas()
        {
            List<MakeRoiViewModel> makeRoiViewModels = MakeRoiViewModels.ToList();

            foreach (var m in makeRoiViewModels)
            {
                if (m.IsChecked)
                {
                    var structureFormula = m.StructureFormula;
                    MakeRoiViewModels.Remove(m);
                    StructureFormulas.Remove(structureFormula);
                }
            }
        }
    }
}
