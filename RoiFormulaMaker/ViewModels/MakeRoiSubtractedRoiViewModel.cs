using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using System;
using RoiFormulaMaker.Notifications;
using System.Collections.Generic;

namespace RoiFormulaMaker.ViewModels
{
    class MakeRoiSubtractedRoiViewModel : BindableBase, IInteractionRequestAware
    {
        public string SelectedStructureName { get; set; }
        public string SelectedSubtractedRoiName { get; set; }

        public string SelectedStructureType { get; set; } = "Control";

        public string SelectedBaseStructureName { get; set; }

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
            notification.SubtractedRoiNames = null;
            notification.Confirmed = false;
            FinishInteraction?.Invoke();
        }

        private void AcceptMakingRoiSubtractedRoi()
        {
            notification.BaseStructureName = SelectedBaseStructureName;

            var subtractedRoiNames = new List<string>();
            foreach (var c in notification.ContouredStructureList)
            {
                if (c.IsSelected)
                {
                    subtractedRoiNames.Add(c.Name);
                }
            }

            notification.SubtractedRoiNames = subtractedRoiNames;

            notification.StructureName = SelectedStructureName;
            notification.StructureType = SelectedStructureType;

            if (string.IsNullOrEmpty(notification.StructureName))
            {
                if (notification.Margin == 0)
                {
                    notification.StructureName = $"z{notification.BaseStructureName}-{string.Join("-", notification.SubtractedRoiNames)}";
                }
                else
                {
                    notification.StructureName = $"z{notification.BaseStructureName}-{string.Join("-", notification.SubtractedRoiNames)}_{notification.Margin}";
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
