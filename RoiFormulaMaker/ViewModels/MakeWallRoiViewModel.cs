using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using System;

using RoiFormulaMaker.Notifications;

namespace RoiFormulaMaker.ViewModels
{
    public class MakeWallRoiViewModel : BindableBase, IInteractionRequestAware
    {
        public string SelectedStructureName { get; set; }

        public string SelectedStructureType { get; set; } = "Control";

        public string SelectedBaseStructureName { get; set; }

        public DelegateCommand MakeWallRoiCommand { get; private set; }

        public DelegateCommand CancelCommand { get; private set; }

        public MakeWallRoiViewModel()
        {
            MakeWallRoiCommand = new DelegateCommand(AcceptMakingWallRoi);
            CancelCommand = new DelegateCommand(CancelInteraction);
        }

        private void CancelInteraction()
        {
            notification.StructureName = null;
            notification.StructureType = null;
            notification.BaseStructureName = null;
            notification.OuterMargin = 0;
            notification.InnerMargin = 0;
            notification.Confirmed = false;
            FinishInteraction?.Invoke();
        }

        private void AcceptMakingWallRoi()
        {
            notification.BaseStructureName = SelectedBaseStructureName;

            notification.StructureName = SelectedStructureName;
            notification.StructureType = SelectedStructureType;

            if (string.IsNullOrEmpty(notification.StructureName))
            {
                    notification.StructureName = $"z{notification.BaseStructureName}_Wall";
            }
            
            notification.Confirmed = true;
            FinishInteraction?.Invoke();
        }

        public Action FinishInteraction { get; set; }

        private MakeWallRoiNotification notification;

        public INotification Notification
        {
            get { return notification; }
            set { SetProperty(ref notification, (MakeWallRoiNotification)value); }
        }
    }
}   
