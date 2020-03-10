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

        public double OriginalPrescribedDose { get; set; }

        public double PrescribedDose { get; set; }

        public bool DoesRescaleDose { get; set; } = true;

        public ObservableCollection<Models.ObjectiveFunction> ObjectiveFunctions { get; private set; } = new ObservableCollection<Models.ObjectiveFunction>();

        public JObject ObjectiveFunctionsJObject { get; private set; }

        public ObservableCollection<Models.Roi> Rois { get; set; } = new ObservableCollection<Models.Roi>();

        public ObservableCollection<string> RoiNamesInObjectiveFunctions { get; private set; } = new ObservableCollection<string>();

        public ObservableCollection<Models.PlanLabel> PlanLabels { get; private set; } = new ObservableCollection<Models.PlanLabel>();

        public ObservableCollection<string> PlanLabelsInObjectiveFuntions { get; private set; } = new ObservableCollection<string>();

        public DelegateCommand OkCommand { get; private set; }
        public DelegateCommand CancelCommand { get; private set; }
        public DelegateCommand ChooseFileCommand { get; private set; }

        public LoadObjectiveFunctionsViewModel()
        {
            PrescribedDose = 7000.0;
            OriginalPrescribedDose = 5000.0;

            List<Models.PlanLabel> planLabels0 = new List<Models.PlanLabel>();
            planLabels0.Add(new Models.PlanLabel("test"));
            planLabels0[0].LabelInObjectiveFunction = PlanLabelNone;
            planLabels0.Add(new Models.PlanLabel("1-1-2"));
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

            OkCommand = new DelegateCommand(() => { CanExecute = true; SetObjectiveFunctionArguments(); });
            CancelCommand = new DelegateCommand(() => { CanExecute = false; });
            ChooseFileCommand = new DelegateCommand(ChooseFile);
        }

        public LoadObjectiveFunctionsViewModel(List<Models.Roi> rois, List<string> planLabels, string defaultDirectoryPath)
        {
            DefaultDirectoryPath = defaultDirectoryPath;

            foreach (var p in planLabels)
            {
                var planLabel = new Models.PlanLabel(p);
                planLabel.LabelInObjectiveFunction = PlanLabelNone;
                PlanLabels.Add(planLabel);
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

                OriginalPrescribedDose = ObjectiveFunctionsJObject["PrescribedDose"].ToObject<double>();
                PrescribedDose = OriginalPrescribedDose;

                var arguments = (JArray)ObjectiveFunctionsJObject["Arguments"];

                ObjectiveFunctions.Clear();
                foreach (var a in arguments)
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
                    if (r.InUse && RoiNamesInObjectiveFunctions.Contains(r.Name))
                    {
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
            }
            else
            {
                Message = "\"Choose File\" is canceled";
            }
        }

        private void SetObjectiveFunctionArguments()
        {
            double scale = 1.0;
            if (DoesRescaleDose)
            {
                if (OriginalPrescribedDose == 0)
                {
                    MessageBox.Show($"No rescale because OriginalPrescribedDose = 0");
                    scale = 1.0;
                }
                else
                {
                    scale = PrescribedDose / OriginalPrescribedDose;
                }
            }

            foreach (var r in Rois)
            {
                if (!r.InUse || r.NameInObjectiveFunction == RoiNameNone) continue;

                var query = ObjectiveFunctions.Where(o => (o.InUse && o.RoiName == r.NameInObjectiveFunction));
                if (query.Count() == 0) continue;

                foreach (var o in query)
                {
                    var planLabelInObjectiveFunction = o.Arguments["PlanLabel"].ToObject<string>();

                    var planLabelQuery = PlanLabels.Where(p => p.LabelInObjectiveFunction == planLabelInObjectiveFunction);
                    if (planLabelQuery.Count() > 0)
                    {
                        var newPlanLabel = planLabelQuery.First();
                        bool inUse = newPlanLabel.InUse;
                        if (!inUse) break;
                        string newLabel = newPlanLabel.Label;
                        if (planLabelQuery.Count() >= 2)
                        {
                            MessageBox.Show($"Multiple plans are assigned to {planLabelInObjectiveFunction}. Use {newLabel}");
                        }
                        o.SetPlanLabelInArguments(newLabel);
                    }
                    else if (!(planLabelInObjectiveFunction == PlanLabelCombinedDose))
                    {
                        break;
                    }

                    o.UpdateWeightInArguments();
                    o.SetRoiNameInArguments(r.Name);

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

                    r.ObjectiveFuntionArguments.Add(o.Arguments.ToString());
                }
            }
        }
    }
}