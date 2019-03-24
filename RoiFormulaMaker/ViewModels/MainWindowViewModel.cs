using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using RoiFormulaMaker.Models;
using RoiFormulaMaker.Notifications;

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

        public ObservableCollection<string> StructureNames { get; set; } = new ObservableCollection<string> { "PTV", "Rectum", "Bladder" };

        public InteractionRequest<MakeRingRoiNotification> MakeRingRoiRequest { get; set; }
        public DelegateCommand MakeRingRoiCommand { get; set; }

        public InteractionRequest<MakeTargetSubtractedRoiNotification> MakeTargetSubtractedRoiRequest { get; set; }
        public DelegateCommand MakeTargetSubtractedRoiCommand { get; set; }

        public InteractionRequest<MakeMarginAddedRoiNotification> MakeMarginAddedRoiRequest { get; set; }
        public DelegateCommand MakeMarginAddedRoiCommand { get; set; }

        public List<dynamic> StructureDesigns { get; set; } = new List<dynamic>();

        private string structureDescriptions;
        public string StructureDescriptions
        {
            get { return structureDescriptions; }
            set { SetProperty(ref structureDescriptions, value); }
        }
        public MainWindowViewModel()
        {

            MakeRingRoiRequest = new InteractionRequest<MakeRingRoiNotification>();
            MakeRingRoiCommand = new DelegateCommand(RaiseMakeRingRoiInteraction);

            MakeTargetSubtractedRoiRequest = new InteractionRequest<MakeTargetSubtractedRoiNotification>();
            MakeTargetSubtractedRoiCommand = new DelegateCommand(RaiseMakeTargetSubtractedRoiInteraction);

            MakeMarginAddedRoiRequest = new InteractionRequest<MakeMarginAddedRoiNotification>();
            MakeMarginAddedRoiCommand = new DelegateCommand(RaiseMakeMarginAddedRoiInteraction);
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

        private void RaiseMakeTargetSubtractedRoiInteraction()
        {
            MakeTargetSubtractedRoiRequest.Raise(new MakeTargetSubtractedRoiNotification
            {
                Title = "Make Target Subtracted ROI",
                Margin = 0,
                StructureNames = this.StructureNames
            },
            r =>
            {
                if (r.Confirmed && r.BaseStructureName != null && r.SubtractedTargetName != null)
                {
                    Message = $"User selected: Base => { r.BaseStructureName}, Subtracted Target => {r.SubtractedTargetName}";
                    var targetSubtractedRoiParameters = new TargetSubtractedRoiParameters
                    {
                        StructureName = r.StructureName,
                        BaseStructureName = r.BaseStructureName,
                        SubtractedTargetName = r.SubtractedTargetName,
                        Margin = r.Margin
                    };
                    if (StructureDesigns.Contains(targetSubtractedRoiParameters))
                    {
                        Message = "The same target subtracted ROI is already in the list";
                        return;
                        ;
                    }
                    StructureDesigns.Add(targetSubtractedRoiParameters);
                    var structureDescription = targetSubtractedRoiParameters.ToString();
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
    }
}
