using System.Windows;
using System.Windows.Interactivity;

namespace MvvmCommon.Actions
{
    /// <summary> Action class for closing Window </summary>
    public class CloseWindowAction : TriggerAction<FrameworkElement>
    {
        protected override void Invoke(object parameter)
        {
            Window.GetWindow(AssociatedObject).Close();
        }
    }

}
