using Microsoft.WindowsAPICodePack.Dialogs;
using Prism.Commands;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;
using System.Windows;

namespace OptimizationFunctionCopyManager.ViewModels
{
    public class LoadObjectiveFunctionsViewModel : BindableBase
    {
        private static readonly string RoiNameNone = "(none)";
        private static readonly string PlanLabelNone = "(none)";
        private static readonly string PlanLabelCombinedDose = "Combined dose";

        public bool CanExecute { get; set; } = false;

        private string message;
        public string Message
        {
            get { return message; }
            set { SetProperty(ref message, value); }
        }

        private string filePath;
        public string FilePath
        {
            get { return filePath; }
            set { SetProperty(ref filePath, value); }
        }

        private string defaultDirectoryPath;
        public string DefaultDirectoryPath
        {
            get { return defaultDirectoryPath; }
            set { SetProperty(ref defaultDirectoryPath, value); }
        }

        public bool DoesRescaleDose { get; set; } = true;

        public ObservableCollection<Models.ObjectiveFunction> ObjectiveFunctions { get; private set; } = new ObservableCollection<Models.ObjectiveFunction>();

        public JObject ObjectiveFunctionsJObject { get; private set; }

        public ObservableCollection<Models.Roi> Rois { get; set; } = new ObservableCollection<Models.Roi>();

        public ObservableCollection<string> RoiNamesInObjectiveFunctions { get; private set; } = new ObservableCollection<string>();

        public ObservableCollection<Models.PlanLabel> PlanLabels { get; private set; } = new ObservableCollection<Models.PlanLabel>();

        public ObservableCollection<string> PlanLabelsInObjectiveFuntions { get; private set; } = new ObservableCollection<string>();

        private ObservableCollection<Models.Prescription> prescriptions = new ObservableCollection<Models.Prescription>();
        public ObservableCollection<Models.Prescription> Prescriptions
        {
            get { return prescriptions; }
            set { SetProperty(ref prescriptions, value); }
        }

        public bool DoesClearObjectiveFunctions { get; set; } = true;

        private string description;
        public string Description
        {
            get { return description; }
            set { SetProperty(ref description, value); }
        }

        public DelegateCommand OkCommand { get; private set; }
        public DelegateCommand CancelCommand { get; private set; }
        public DelegateCommand ChooseFileCommand { get; private set; }

        public LoadObjectiveFunctionsViewModel()
        {
            List<Models.PlanLabel> planLabels0 = new List<Models.PlanLabel>();
            planLabels0.Add(new Models.PlanLabel("1-1-1", 4600));
            planLabels0[0].LabelInObjectiveFunction = PlanLabelNone;
            planLabels0.Add(new Models.PlanLabel("1-1-2", 2600));
            planLabels0[1].LabelInObjectiveFunction = PlanLabelNone;
            PlanLabels = new ObservableCollection<Models.PlanLabel>(planLabels0);

            List<Models.Roi> rois0 = new List<Models.Roi>();
            rois0.Add(new Models.Roi("PTV1"));
            rois0.Add(new Models.Roi("CTV"));
            rois0.Add(new Models.Roi("Rectum"));
            rois0.Add(new Models.Roi("Bladder"));

            Rois = new ObservableCollection<Models.Roi>(rois0);
            foreach (var r in Rois)
            {
                r.NameInObjectiveFunction = RoiNameNone;
            }
            RoiNamesInObjectiveFunctions.Add(RoiNameNone);
            PlanLabelsInObjectiveFuntions.Add(PlanLabelNone);

            Prescriptions.Add(new Models.Prescription { PlanLabel = "test", PrescribedDose = 300, PrescribedDoseInObjectiveFunction = 400 });

            OkCommand = new DelegateCommand(() => { CanExecute = true; SetObjectiveFunctionArguments(); });
            CancelCommand = new DelegateCommand(() => { CanExecute = false; });
            ChooseFileCommand = new DelegateCommand(ChooseFile);
        }

        public LoadObjectiveFunctionsViewModel(List<Models.Roi> rois, List<Models.PlanLabel> planLabels, string defaultDirectoryPath)
        {
            DefaultDirectoryPath = defaultDirectoryPath;

            PlanLabels = new ObservableCollection<Models.PlanLabel>(planLabels);
            foreach (var p in PlanLabels)
            {
                p.LabelInObjectiveFunction = PlanLabelNone;
            }
            PlanLabelsInObjectiveFuntions.Add(PlanLabelNone);

            Rois = new ObservableCollection<Models.Roi>(rois);
            foreach (var r in Rois)
            {
                r.NameInObjectiveFunction = RoiNameNone;
            }
            RoiNamesInObjectiveFunctions.Add(RoiNameNone);

            OkCommand = new DelegateCommand(() => { CanExecute = true; SetObjectiveFunctionArguments(); });
            CancelCommand = new DelegateCommand(() => { CanExecute = false; });
            ChooseFileCommand = new DelegateCommand(ChooseFile);
        }

        private void ChooseFile()
        {
            var dialog = new CommonOpenFileDialog("Choose File");

            if (Directory.Exists(DefaultDirectoryPath))
            {
                dialog.InitialDirectory = DefaultDirectoryPath;
            }

            dialog.IsFolderPicker = false;

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                FilePath = dialog.FileName;

                using (var sr = new StreamReader(FilePath))
                using (JsonTextReader reader = new JsonTextReader(sr))
                {
                    ObjectiveFunctionsJObject = (JObject)JToken.ReadFrom(reader);
                }

                var prescriptionsJArray = (JArray)ObjectiveFunctionsJObject["Prescriptions"];
                Prescriptions = prescriptionsJArray.ToObject<ObservableCollection<Models.Prescription>>();

                var descriptionJObject = ObjectiveFunctionsJObject["Description"];
                if (descriptionJObject != null)
                {
                    Description = descriptionJObject.ToObject<string>();
                }
                else
                {
                    Description = string.Empty;
                }

                var argumentsJArray = (JArray)ObjectiveFunctionsJObject["Arguments"];
                ObjectiveFunctions.Clear();
                foreach (var a in argumentsJArray)
                {
                    var jObject = JObject.Parse(a.ToString());
                    ObjectiveFunctions.Add(new Models.ObjectiveFunction(jObject));
                    string roiName = jObject["RoiName"].ToObject<string>();
                    if (!RoiNamesInObjectiveFunctions.Contains(roiName))
                    {
                        RoiNamesInObjectiveFunctions.Add(roiName);
                    }

                    string planLabel = jObject["PlanLabel"].ToObject<string>();
                    if (!(planLabel == PlanLabelCombinedDose) && !PlanLabelsInObjectiveFuntions.Contains(planLabel))
                    {
                        PlanLabelsInObjectiveFuntions.Add(planLabel);
                    }
                }
                RoiNamesInObjectiveFunctions.OrderBy(r => r);

                foreach (var r in Rois)
                {
                    if (RoiNamesInObjectiveFunctions.Contains(r.Name))
                    {
                        r.InUse = true;
                        r.NameInObjectiveFunction = r.Name;
                    }
                }

                foreach (var p in PlanLabels)
                {
                    if (p.InUse && PlanLabelsInObjectiveFuntions.Contains(p.Label))
                    {
                        p.LabelInObjectiveFunction = p.Label;
                    }
                }

                var planSumDose = PlanLabels.Select(p => p.PrescribedDose).Sum();
                foreach (var p in Prescriptions)
                {
                    var query = PlanLabels.Where(pl => pl.Label == p.PlanLabel);
                    if (query.Count() > 0)
                    {
                        p.PrescribedDose = query.First().PrescribedDose;
                        continue;
                    }

                    if (p.PlanLabel == PlanLabelCombinedDose)
                    {
                        p.PrescribedDose = planSumDose;
                    }
                }
            }
            else
            {
                Message = "\"Choose File\" is canceled";
            }
        }

        private void SetObjectiveFunctionArguments()
        {
            foreach (var r in Rois)
            {
                if (!r.InUse || r.NameInObjectiveFunction == RoiNameNone) continue;

                var query = ObjectiveFunctions.Where(o => (o.InUse && o.RoiName == r.NameInObjectiveFunction));
                if (query.Count() == 0) continue;

                foreach (var o in query)
                {
                    var planLabelInObjectiveFunction = o.Arguments["PlanLabel"].ToObject<string>();

                    var planLabelQuery = PlanLabels.Where(p => p.LabelInObjectiveFunction == planLabelInObjectiveFunction);
                    string newLabel = string.Empty;
                    if (planLabelQuery.Count() > 0)
                    {
                        var newPlanLabel = planLabelQuery.First();
                        bool inUse = newPlanLabel.InUse;
                        if (!inUse) break;
                        newLabel = newPlanLabel.Label;
                        if (planLabelQuery.Count() >= 2)
                        {
                            Message = $"Multiple plans are assigned to {planLabelInObjectiveFunction}. Use {newLabel}.";
                        }
                        o.SetPlanLabelInArguments(newLabel);
                    }
                    else if (!(planLabelInObjectiveFunction == PlanLabelCombinedDose))
                    {
                        break;
                    }

                    o.UpdateWeightInArguments();
                    o.SetRoiNameInArguments(r.Name);

                    if (o.PlanLabel == PlanLabelCombinedDose) newLabel = PlanLabelCombinedDose;
                    var prescriptionQuery = Prescriptions.Where(p => p.PlanLabel == newLabel);
                    double scale = 0;
                    if (prescriptionQuery.Count() == 0)
                    {
                        scale = 1.0;
                        Message = $"Prescription does not exist for {newLabel}";
                    }
                    else
                    {
                        var prescription = prescriptionQuery.First();
                        var originalPrescribedDose = prescription.PrescribedDoseInObjectiveFunction;
                        var prescribedDose = prescription.PrescribedDose;
                        if (DoesRescaleDose)
                        {
                            if (originalPrescribedDose == 0)
                            {
                                Message = $"No rescale because the original prescribed dose = 0";
                                scale = 1.0;
                            }
                            else
                            {
                                scale = prescribedDose / originalPrescribedDose;
                            }
                        }
                    }

                    var functionType = o.Arguments["FunctionType"].ToObject<string>();
                    if (functionType == "DoseFallOff")
                    {
                        var highDoseLevel = scale * o.Arguments["HighDoseLevel"].ToObject<double>();
                        var lowDoseLevel = scale * o.Arguments["LowDoseLevel"].ToObject<double>();
                        o.Arguments["HighDoseLevel"] = highDoseLevel;
                        o.Arguments["LowDoseLevel"] = lowDoseLevel;
                    }
                    else if (functionType == "UniformDose"
                        || functionType == "MaxDose" || functionType == "MinDose"
                        || functionType == "MaxEud" || functionType == "MinEud"
                        || functionType == "MaxDvh" || functionType == "MinDvh")
                    {
                        var doseLevel = scale * o.Arguments["DoseLevel"].ToObject<double>();
                        o.Arguments["DoseLevel"] = doseLevel;
                    }

                    r.ObjectiveFunctionArguments.Add(o.Arguments.ToString());
                }
            }
        }
    }
}