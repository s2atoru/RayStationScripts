using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Prism.Commands;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

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

        public ObservableCollection<Models.ObjectiveFunction> ObjectiveFunctions { get; private set; } = new ObservableCollection<Models.ObjectiveFunction>();

        public ObservableCollection<Models.Prescription> Prescriptions { get; private set; } = new ObservableCollection<Models.Prescription>();

        public DelegateCommand OkCommand { get; private set; }
        public DelegateCommand CancelCommand { get; private set; }
        public DelegateCommand SaveFileCommand { get; private set; }

        public SaveObjectiveFunctionsViewModel()
        {
            Prescriptions.Add(new Models.Prescription { PlanLabel = "1-1-1", PrescribedDose = 4600 });
            Prescriptions.Add(new Models.Prescription { PlanLabel = "1-1-2", PrescribedDose = 2600 });
            Prescriptions.Add(new Models.Prescription { PlanLabel = "Combined dose", PrescribedDose = 7200 });
            OkCommand = new DelegateCommand(() => { CanExecute = true; });
            CancelCommand = new DelegateCommand(() => { CanExecute = false; });
            SaveFileCommand = new DelegateCommand(SaveFile);
        }
        public SaveObjectiveFunctionsViewModel(string objectiveFunctionsJson, string defaultDirectoryPath)
        {
            DefaultDirectoryPath = defaultDirectoryPath;

            var objectiveFunctionsJObject = JObject.Parse(objectiveFunctionsJson);
            var objectiveFunctionArguments = (JArray)objectiveFunctionsJObject["Arguments"];
            
            ObjectiveFunctions.Clear();
            foreach (var a in objectiveFunctionArguments)
            {
                var jObject = JObject.Parse(a.ToString());
                ObjectiveFunctions.Add(new Models.ObjectiveFunction(jObject));
            }

            var prescriptionsJArray = (JArray)objectiveFunctionsJObject["Prescriptions"];
            var PrescriptionsAll = prescriptionsJArray.ToObject<List<Models.Prescription>>();
            Prescriptions.Clear();
            foreach (var p in PrescriptionsAll)
            {
                if (ObjectiveFunctions.Where( o => (o.PlanLabel == p.PlanLabel)).Count() > 0)
                {
                    Prescriptions.Add(p);
                }
            }

            OkCommand = new DelegateCommand(() => { CanExecute = true; });
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

                UpdateWeightInObjectiveFunctions();
                var argumentsJArray = new JArray();
                foreach (var o in ObjectiveFunctions)
                {
                    if (!o.InUse) continue;
                    var a = o.Arguments;
                    argumentsJArray.Add(JToken.Parse(a.ToString()));
                }

                foreach (var p in Prescriptions)
                {
                    p.PrescribedDoseInObjectiveFunction = p.PrescribedDose;
                }
                var prescriptionsJArray = (JArray)JToken.FromObject(Prescriptions);
                
                var jObject = new JObject();
                jObject["Description"] = Description;
                jObject["Prescriptions"] = prescriptionsJArray;
                jObject["Arguments"] = argumentsJArray;

                using (StreamWriter file = File.CreateText(SavedFilePath))
                using (JsonTextWriter writer = new JsonTextWriter(file))
                {
                    writer.Formatting = Formatting.Indented;
                    jObject.WriteTo(writer);
                }
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
