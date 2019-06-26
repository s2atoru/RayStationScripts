using Prism.Mvvm;

namespace MvvmCommon.ViewModels
{
    public class ListBoxItemViewModel : BindableBase
    {
        private string name;
        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value); }
        }

        private bool isSelected;
        public bool IsSelected
        {
            get { return isSelected; }
            set { SetProperty(ref isSelected, value); }
        }
    }
}
