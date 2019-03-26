using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using RoiFormulaMaker.Models;
using RoiFormulaMaker.Notifications;
using Microsoft.WindowsAPICodePack.Dialogs;

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

        public ObservableCollection<string> StructureNames { get; set; } = new ObservableCollection<string> { "PTV", "Rectum", "Bladder" };

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

        public MainWindowViewModel()
        {

            MakeRingRoiRequest = new InteractionRequest<MakeRingRoiNotification>();
            MakeRingRoiCommand = new DelegateCommand(RaiseMakeRingRoiInteraction);

            MakeRoiSubtractedRoiRequest = new InteractionRequest<MakeRoiSubtractedRoiNotification>();
            MakeRoiSubtractedRoiCommand = new DelegateCommand(RaiseMakeRoiSubtractedRoiInteraction);

            MakeMarginAddedRoiRequest = new InteractionRequest<MakeMarginAddedRoiNotification>();
            MakeMarginAddedRoiCommand = new DelegateCommand(RaiseMakeMarginAddedRoiInteraction);

            ChooseFileCommand = new DelegateCommand(ChooseFile);
        }

        private void RaiseMakeRingRoiInteraction()
        {
            MakeRingRoiRequest.Raise(new MakeRingRoiNotification
            {
                Title = "Make Ring ROI",
                OuterMargin = 15,
                InnerMargin = 0,
                StructureNames = this.StructureNames
            },
            r =>
            {
                if (r.Confirmed && r.BaseStructureName != null)
                {
                    Message = $"User selected: { r.BaseStructureName}";
                    var ringRoiParameters = new RingRoiParameters
                    {
                        StructureName = r.StructureName,
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
                StructureNames = this.StructureNames
            },
            r =>
            {
                if (r.Confirmed && r.BaseStructureName != null && r.SubtractedRoiName != null)
                {
                    Message = $"User selected: Base => { r.BaseStructureName}, Subtracted ROI => {r.SubtractedRoiName}";
                    var roiSubtractedRoiParameters = new RoiSubtractedRoiParameters
                    {
                        StructureName = r.StructureName,
                        BaseStructureName = r.BaseStructureName,
                        SubtractetRoiName = r.SubtractedRoiName,
                        Margin = r.Margin
                    };
                    if (StructureDesigns.Contains(roiSubtractedRoiParameters))
                    {
                        Message = "The same ROI subtracted ROI is already in the list";
                        return;
                        ;
                    }
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
                StructureNames = this.StructureNames
            },
            r =>
            {
                if (r.Confirmed && r.BaseStructureName != null)
                {
                    Message = $"User selected: Base => { r.BaseStructureName}";
                    var MarginAddedRoiParameters = new MarginAddedRoiParameters
                    {
                        StructureName = r.StructureName,
                        BaseStructureName = r.BaseStructureName,
                        Margin = r.Margin
                    };
                    if (StructureDesigns.Contains(MarginAddedRoiParameters))
                    {
                        Message = "The margin added ROI is already in the list";
                        return;
                        ;
                    }
                    StructureDesigns.Add(MarginAddedRoiParameters);
                    var structureDescription = MarginAddedRoiParameters.ToString();
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

    }
}
