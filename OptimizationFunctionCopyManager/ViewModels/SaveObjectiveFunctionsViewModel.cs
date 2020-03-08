using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using Prism.Commands;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.IO;

namespace OptimizationFunctionCopyManager.ViewModels
{
    public class SaveObjectiveFunctionsViewModel : BindableBase
    { 
        public bool CanExecute { get; set; } = false;

        public string Description { get; set; }

        private string message;
        public string Message
        {
            get { return message; }
            set { SetProperty(ref message, value); }
        }

        private string savedFilePath;
        public string SavedFilePath
        {
            get { return savedFilePath; }
            set { SetProperty(ref savedFilePath, value); }
        }

        private string defaultDirectoryPath;
        public string DefaultDirectoryPath
        {
            get { return defaultDirectoryPath; }
            set { SetProperty(ref defaultDirectoryPath, value); }
        }
        public double PrescribedDose { get; set; }

        public ObservableCollection<Models.ObjectiveFunction> ObjectiveFunctions = new ObservableCollection<Models.ObjectiveFunction>();

        public DelegateCommand OkCommand { get; private set; }
        public DelegateCommand CancelCommand { get; private set; }
        public DelegateCommand SaveFileCommand { get; private set; }

        public SaveObjectiveFunctionsViewModel()
        {
            PrescribedDose = 7000;
            OkCommand = new DelegateCommand(() => { CanExecute = true; UpdateWeightInObjectiveFunctions(); });
            CancelCommand = new DelegateCommand(() => { CanExecute = false; });
            SaveFileCommand = new DelegateCommand(SaveFile);
        }
        public SaveObjectiveFunctionsViewModel(string objectiveFunctionsJson, string defaultDirectoryPath)
        {
            DefaultDirectoryPath = defaultDirectoryPath;

            var objectiveFunctionsJObject = JObject.Parse(objectiveFunctionsJson);
            var objectiveFunctionArguments = (JArray)objectiveFunctionsJson;

            ObjectiveFunctions.Clear();
            foreach (var a in objectiveFunctionArguments)
            {
                var jObject = JObject.Parse(a.ToString());
                ObjectiveFunctions.Add(new Models.ObjectiveFunction(jObject));
            }

            OkCommand = new DelegateCommand(() => { CanExecute = true; UpdateWeightInObjectiveFunctions(); });
            CancelCommand = new DelegateCommand(() => { CanExecute = false; });
            SaveFileCommand = new DelegateCommand(SaveFile);
        }

        private void SaveFile()
        {
            var dialog = new SaveFileDialog();
            dialog.Title = "Save to File";

            if (Directory.Exists(DefaultDirectoryPath))
            {
                dialog.InitialDirectory = DefaultDirectoryPath;
            }

            //dialog.Filter = "text file|*.txt";
            if (dialog.ShowDialog() == true)
            {
                SavedFilePath = dialog.FileName;
            }
            else
            {
                Message = "\"Save to File\" is canceled";
            }
        }

        private void UpdateWeightInObjectiveFunctions()
        {
            foreach (var o in ObjectiveFunctions)
            {
                o.UpdateWeightInArguments();
            }
        }
    }
}
