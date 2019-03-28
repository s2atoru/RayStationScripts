using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using System;
using RoiFormulaMaker.Notifications;

namespace RoiFormulaMaker.ViewModels
{
    class MakeRoiSubtractedRoiViewModel : BindableBase, IInteractionRequestAware
    {
        public string SelectedStructureName { get; set; }
        public string SelectedSubtractedRoi { get; set; }

        public string SelectedStructureType { get; set; } = "Control";

        public DelegateCommand MakeRoiSubtractedRoiCommand { get; private set; }

        public DelegateCommand CancelCommand { get; private set; }

        public MakeRoiSubtractedRoiViewModel()
        {
            MakeRoiSubtractedRoiCommand = new DelegateCommand(AcceptMakingRoiSubtractedRoi);
            CancelCommand = new DelegateCommand(CancelInteraction);
        }

        private void CancelInteraction()
        {
            notification.StructureName = null;
            notification.StructureType = null;
            notification.Margin = 0;
            notification.BaseStructureName = null;
            notification.SubtractedRoiName = null;
            notification.Confirmed = false;
            FinishInteraction?.Invoke();
        }

        private void AcceptMakingRoiSubtractedRoi()
        {
            notification.BaseStructureName = SelectedStructureName;
            notification.SubtractedRoiName = SelectedSubtractedRoi;
            notification.StructureType = SelectedStructureType;
            if (string.IsNullOrEmpty(notification.StructureName))
            {
                if (notification.Margin == 0)
                {
                    notification.StructureName = $"z{notification.BaseStructureName}-{notification.SubtractedRoiName}";
                }
                else
                {
                    notification.StructureName = $"z{notification.BaseStructureName}-{notification.SubtractedRoiName}_{notification.Margin}";
                }
            }

            notification.Confirmed = true;
            FinishInteraction?.Invoke();
        }

        public Action FinishInteraction { get; set; }

        private MakeRoiSubtractedRoiNotification notification;

        public INotification Notification
        {
            get { return notification; }
            set { SetProperty(ref notification, (MakeRoiSubtractedRoiNotification)value); }
        }
    }
}
