using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using RoiFormulaMaker.Models;
using RoiFormulaMaker.Notifications;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace RoiFormulaMaker.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {

        private string title = "Make Dummy ROIs";
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
            }
        }

        public ObservableCollection<string> StructureNames { get; set; } = new ObservableCollection<string> { "PTV", "Rectum", "Bladder" };
        public ObservableCollection<string> ContouredStructureNames { get; set; } = new ObservableCollection<string> { "PTV", "Rectum" };

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

        public InteractionRequest<MakeRingRoiNotification> MakeRingRoiRequest { get; set; }
        public DelegateCommand MakeRingRoiCommand { get; set; }

        public InteractionRequest<MakeRoiSubtractedRoiNotification> MakeRoiSubtractedRoiRequest { get; set; }
        public DelegateCommand MakeRoiSubtractedRoiCommand { get; set; }

        public InteractionRequest<MakeMarginAddedRoiNotification> MakeMarginAddedRoiRequest { get; set; }
        public DelegateCommand MakeMarginAddedRoiCommand { get; set; }

        public List<dynamic> StructureDesigns { get; set; } = new List<dynamic>();

        private string structureDescriptions;
        public string StructureDescriptions
        {
            get { return structureDescriptions; }
            set { SetProperty(ref structureDescriptions, value); }
        }

        public DelegateCommand ChooseFileCommand { get; private set; }
        public DelegateCommand SaveFileCommand { get; private set; }

        public MainWindowViewModel()
        {

            MakeRingRoiRequest = new InteractionRequest<MakeRingRoiNotification>();
            MakeRingRoiCommand = new DelegateCommand(RaiseMakeRingRoiInteraction);

            MakeRoiSubtractedRoiRequest = new InteractionRequest<MakeRoiSubtractedRoiNotification>();
            MakeRoiSubtractedRoiCommand = new DelegateCommand(RaiseMakeRoiSubtractedRoiInteraction);

            MakeMarginAddedRoiRequest = new InteractionRequest<MakeMarginAddedRoiNotification>();
            MakeMarginAddedRoiCommand = new DelegateCommand(RaiseMakeMarginAddedRoiInteraction);

            ChooseFileCommand = new DelegateCommand(ChooseFile);
            SaveFileCommand = new DelegateCommand(SaveFile);
        }

        private void UpdateStructureNames(string structureName)
        {
            if (!StructureNames.Contains(structureName))
            {
                StructureNames.Add(structureName);
            }

            if (!ContouredStructureNames.Contains(structureName))
            {
                ContouredStructureNames.Add(structureName);
            }
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
                    if (StructureDesigns.Contains(ringRoiParameters))
                    {
                        Message = "The same ring is already in the list";
                        return;
                        ;
                    }

                    UpdateStructureNames(r.StructureName);

                    StructureDesigns.Add(ringRoiParameters);
                    var structureDescription = ringRoiParameters.ToString();
                    if (string.IsNullOrEmpty(StructureDescriptions))
                    {
                        StructureDescriptions = structureDescription;
                    }
                    else
                    {
                        StructureDescriptions += "\n" + structureDescription;
                    }
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
                    if (StructureDesigns.Contains(roiSubtractedRoiParameters))
                    {
                        Message = "The same ROI subtracted ROI is already in the list";
                        return;
                        ;
                    }

                    UpdateStructureNames(r.StructureName);

                    StructureDesigns.Add(roiSubtractedRoiParameters);
                    var structureDescription = roiSubtractedRoiParameters.ToString();
                    if (string.IsNullOrEmpty(StructureDescriptions))
                    {
                        StructureDescriptions = structureDescription;
                    }
                    else
                    {
                        StructureDescriptions += "\n" + structureDescription;
                    }
                }
                else
                    Message = "User canceled or didn't select structures";
            });
        }

        private void RaiseMakeMarginAddedRoiInteraction()
        {
            MakeMarginAddedRoiRequest.Raise(new MakeMarginAddedRoiNotification
            {
                Title = "Make Margin Added ROI",
                Margin = 0,
                StructureNames = this.StructureNames,
                ContouredStructureNames = this.ContouredStructureNames,
                StructureTypes = this.StructureTypes
            },
            r =>
            {
                if (r.Confirmed && r.BaseStructureName != null)
                {
                    Message = $"User selected: Base => { r.BaseStructureName}";
                    var marginAddedRoiParameters = new MarginAddedRoiParameters
                    {
                        StructureName = r.StructureName,
                        StructureType = r.StructureType,
                        BaseStructureName = r.BaseStructureName,
                        Margin = r.Margin
                    };
                    if (StructureDesigns.Contains(marginAddedRoiParameters))
                    {
                        Message = "The margin added ROI is already in the list";
                        return;
                        ;
                    }

                    UpdateStructureNames(r.StructureName);

                    StructureDesigns.Add(marginAddedRoiParameters);
                    var structureDescription = marginAddedRoiParameters.ToString();
                    if (string.IsNullOrEmpty(StructureDescriptions))
                    {
                        StructureDescriptions = structureDescription;
                    }
                    else
                    {
                        StructureDescriptions += "\n" + structureDescription;
                    }
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

            dialog.InitialDirectory = DefaultDirectoryPath;

            dialog.IsFolderPicker = false;

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                FilePath = dialog.FileName;
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

            dialog.InitialDirectory = DefaultDirectoryPath;

            //dialog.Filter = "text file|*.txt";
            if (dialog.ShowDialog() == true)
            {
                SavedFilePath = dialog.FileName;
            }
            else
            {
                Message = "\"Save to File\" is canceled";
            }
        }
    }
}
