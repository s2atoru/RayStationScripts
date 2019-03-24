using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using System;

using RoiFormulaMaker.Notifications;

namespace RoiFormulaMaker.ViewModels
{
    public class MakeRingViewModel : BindableBase, IInteractionRequestAware
    {
        public string SelectedStructureName { get; set; }

        public DelegateCommand MakeRingCommand { get; private set; }

        public DelegateCommand CancelCommand { get; private set; }

        public MakeRingViewModel()
        {
            MakeRingCommand = new DelegateCommand(AcceptMakingRing);
            CancelCommand = new DelegateCommand(CancelInteraction);
        }

        private void CancelInteraction()
        {
            notification.StructureName = null;
            notification.OuterMargin = 0;
            notification.InnerMargin = 0;
            notification.Confirmed = false;
            FinishInteraction?.Invoke();
        }

        private void AcceptMakingRing()
        {
            notification.BaseStructureName = SelectedStructureName;
            if (string.IsNullOrEmpty(notification.StructureName))
            {
                if (notification.InnerMargin == 0)
                {
                    notification.StructureName = $"NS_Ring_{notification.OuterMargin}";
                }
                else
                {
                    notification.StructureName = $"NS_{notification.InnerMargin}_Ring_{notification.OuterMargin}";
                }
            }
            
            notification.Confirmed = true;
            FinishInteraction?.Invoke();
        }

        public Action FinishInteraction { get; set; }

        private MakeRingNotification notification;

        public INotification Notification
        {
            get { return notification; }
            set { SetProperty(ref notification, (MakeRingNotification)value); }
        }
    }
}   
