using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using System;
using RoiFormulaMaker.Notifications;

namespace RoiFormulaMaker.ViewModels
{
    class MakeTargetSubtractedRoiViewModel : BindableBase, IInteractionRequestAware
    {
        public string SelectedStructureName { get; set; }
        public string SelectedSubtractedTarget { get; set; }


        public DelegateCommand MakeTargetSubtractedRoiCommand { get; private set; }

        public DelegateCommand CancelCommand { get; private set; }

        public MakeTargetSubtractedRoiViewModel()
        {
            MakeTargetSubtractedRoiCommand = new DelegateCommand(AcceptMakingTargetSubtractedRoi);
            CancelCommand = new DelegateCommand(CancelInteraction);
        }

        private void CancelInteraction()
        {
            notification.StructureName = null;
            notification.Margin = 0;
            notification.BaseStructureName = null;
            notification.SubtractedTargetName = null;
            notification.Confirmed = false;
            FinishInteraction?.Invoke();
        }

        private void AcceptMakingTargetSubtractedRoi()
        {
            notification.BaseStructureName = SelectedStructureName;
            notification.SubtractedTargetName = SelectedSubtractedTarget;
            if (string.IsNullOrEmpty(notification.StructureName))
            {
                if (notification.Margin == 0)
                {
                    notification.StructureName = $"NS_OPT_{notification.BaseStructureName}";
                }
                else
                {
                    notification.StructureName = $"NS_{notification.Margin}_OPT_{notification.BaseStructureName}";
                }
            }

            notification.Confirmed = true;
            FinishInteraction?.Invoke();
        }

        public Action FinishInteraction { get; set; }

        private MakeTargetSubtractedRoiNotification notification;

        public INotification Notification
        {
            get { return notification; }
            set { SetProperty(ref notification, (MakeTargetSubtractedRoiNotification)value); }
        }
    }
}
