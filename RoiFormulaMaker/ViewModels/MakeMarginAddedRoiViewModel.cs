using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using System;
using RoiFormulaMaker.Notifications;
using System.Collections.Generic;

namespace RoiFormulaMaker.ViewModels
{
    class MakeMarginAddedRoiViewModel : BindableBase, IInteractionRequestAware
    {
        public DelegateCommand MakeMarginAddedRoiCommand { get; private set; }

        public DelegateCommand CancelCommand { get; private set; }

        public MakeMarginAddedRoiViewModel()
        {
            MakeMarginAddedRoiCommand = new DelegateCommand(AcceptMakingMarginAddedRoi);
            CancelCommand = new DelegateCommand(CancelInteraction);
        }

        private void CancelInteraction()
        {
            notification.StructureName = null;
            notification.StructureType = null;
            notification.Margin = 0;
            notification.BaseStructureNames = null;
            notification.Confirmed = false;
            FinishInteraction?.Invoke();
        }

        private void AcceptMakingMarginAddedRoi()
        {
            var baseStructureNames = new List<string>();
            foreach (var c in notification.ContouredStructureList)
            {
                if (c.IsSelected)
                {
                    baseStructureNames.Add(c.Name);
                }
            }
            notification.BaseStructureNames = baseStructureNames;
            
            if (string.IsNullOrEmpty(notification.StructureName))
            {
                notification.StructureName = $"z{string.Join("_",notification.BaseStructureNames)}_{notification.Margin}";
            }

            notification.Confirmed = true;
            FinishInteraction?.Invoke();
        }

        public Action FinishInteraction { get; set; }

        private MakeMarginAddedRoiNotification notification;

        public INotification Notification
        {
            get { return notification; }
            set { SetProperty(ref notification, (MakeMarginAddedRoiNotification)value); }
        }
    }
}
