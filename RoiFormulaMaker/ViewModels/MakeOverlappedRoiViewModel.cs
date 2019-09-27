using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using System;
using RoiFormulaMaker.Notifications;
using System.Collections.Generic;

namespace RoiFormulaMaker.ViewModels
{
    class MakeOverlappedRoiViewModel : BindableBase, IInteractionRequestAware
    {
        public string SelectedStructureName { get; set; }
        public string SelectedStructureType { get; set; } = "Control";

        public DelegateCommand MakeOverlappedRoiCommand { get; private set; }

        public DelegateCommand CancelCommand { get; private set; }

        public MakeOverlappedRoiViewModel()
        {
            MakeOverlappedRoiCommand = new DelegateCommand(AcceptMakingOverlappedRoi);
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

        private void AcceptMakingOverlappedRoi()
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

            notification.StructureName = SelectedStructureName;
            notification.StructureType = SelectedStructureType;
            
            if (string.IsNullOrEmpty(notification.StructureName))
            {
                notification.StructureName = $"z{notification.BaseStructureNames[0]}_{notification.Margin}";
            }

            notification.Confirmed = true;
            FinishInteraction?.Invoke();
        }

        public Action FinishInteraction { get; set; }

        private MakeOverlappedRoiNotification notification;

        public INotification Notification
        {
            get { return notification; }
            set { SetProperty(ref notification, (MakeOverlappedRoiNotification)value); }
        }
    }
}
