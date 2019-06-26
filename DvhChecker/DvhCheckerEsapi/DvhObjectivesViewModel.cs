using System;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace Juntendo.MedPhys.Esapi
{
    public class DvhObjectivesViewModel : BindableBase
    {
        public string FolderPath { get; set; }

        private bool isAllVisible = true;
        public bool IsAllVisible
        {
            get { return isAllVisible; }
            set
            {
                if (SetProperty(ref this.isAllVisible, value))
                {

                    foreach (var obj in Objectives)
                    {
                        if (value == true)
                        {
                            obj.IsVisible = true;
                        }
                        else
                        {
                            if (obj.Objective.InUse == true)
                            {
                                obj.IsVisible = true;
                            }
                            else
                            {
                                obj.IsVisible = false;
                            }
                        }
                    }
                }
            }
        }

        public Patient Patient { get; private set; }

        public List<Protocol> Protocols { get; private set; }
        private Protocol protocol;
        public Protocol Protocol
        {
            get { return protocol; }
            set
            {
                if (SetProperty(ref this.protocol, value))
                {
                    IsAllVisible = true;
                }
            }
        }
        public List<ProtocolName> ProtocolNames { get; private set; } = new List<ProtocolName>();

        private string selectedProtocolName;
        public string SelectedProtocolName
        {
            get { return selectedProtocolName; }
            set
            {
                if (SetProperty(ref this.selectedProtocolName, value))
                {
                    var query = from p in Protocols where p.Id == value select p;
                    protocol = query.Single();
                    updateProtocol();
                }
            }
        }

        public List<PlanName> PlanNames { get; private set; } = new List<PlanName>();

        private int selectedPlanNameId;
        public int SelectedPlanNameId
        {
            get { return selectedPlanNameId; }
            set
            {
                if (SetProperty(ref this.selectedPlanNameId, value))
                {
                    var planName = (from p in PlanNames where (p.Id == value) select p).Single();
                    SelectedPlanName = planName;
                }
            }
        }

        private PlanName selectedPlanName;
        public PlanName SelectedPlanName
        {
            get { return selectedPlanName; }
            set
            {
                if (SetProperty(ref this.selectedPlanName, value))
                {
                    setPlanningItemFromPlanName(value);
                    updatePlannigItem();
                }
            }
        }

        public ObservableCollection<DvhObjectiveViewModel> Objectives { get; set; } = new ObservableCollection<DvhObjectiveViewModel>();

        private void updateSameStructureObjectives()
        {
            foreach (var obj in Objectives)
            {
                obj.SetSameStructureObjectives(Objectives.ToList());
            }
        }
        public List<StructureName> StructureNamesTps { get; private set; } = new List<StructureName>();

        public DoseValue.DoseUnit SystemDoseUnit { get; private set; }
        private double systemTotalPrescribedDose;
        public double SystemTotalPrescribedDose
        {
            get { return systemTotalPrescribedDose; }
            set
            {
                SetProperty(ref this.systemTotalPrescribedDose, value);
            }
        }

        private double assignedTotalPrescribedDoseTps;
        public double AssignedTotalPrescribedDoseTps
        {
            get { return assignedTotalPrescribedDoseTps; }
            set
            {
                if (SetProperty(ref this.assignedTotalPrescribedDoseTps, value))
                {
                    foreach (var objective in Objectives)
                    {
                        objective.TotalPrescribedDoseTps = value;
                    }
                }
            }
        }

        private double maxDose;
        public double MaxDose
        {
            get { return maxDose; }
            set
            {
                SetProperty(ref this.maxDose, value);
            }
        }

        public bool IsPlanSum { get; private set; } = false;

        private StructureSet structureSet;
        public StructureSet StructureSet
        {
            get { return structureSet; }
            set
            {
                SetProperty(ref this.structureSet, value);
            }
        }

        private PlanningItem planningItem;

        public PlanningItem PlanningItem
        {
            get { return planningItem; }
        }

        public string PlanId { get { return planningItem.Id; } }
        public string CourseId
        {
            get
            {
                if (planningItem is PlanSetup)
                {
                    return ((PlanSetup)planningItem).Course.Id;
                }
                else if (planningItem is PlanSum)
                {
                    return ((PlanSum)planningItem).Course.Id;
                }
                else
                {
                    throw new InvalidOperationException("planningItem is not PlanSetup nor PlanSum");
                }
            }
        }


        //Stored DVH to prevent multiple GetDVHCumulativeData calls
        public Dictionary<DvhDataParameters, DVHData> StoredDvhs { get; set; } = new Dictionary<DvhDataParameters, DVHData>();

        private List<PlanningItem> planningItems = new List<PlanningItem>();

        private void updateProtocol()
        {
            Objectives.Clear();
            foreach (var objective in protocol.Objectives)
            {
                var objectiveVM = new DvhObjectiveViewModel(objective, planningItem, IsPlanSum, systemTotalPrescribedDose, SystemDoseUnit, StoredDvhs);
                Objectives.Add(objectiveVM);
            }
            updateSameStructureObjectives();
        }

        private void updatePlannigItem()
        {
            Objectives.Clear();

            if (planningItem is PlanSetup)
            {
                IsPlanSum = false;
                SystemDoseUnit = ((PlanSetup)planningItem).TotalPrescribedDose.Unit;
                SystemTotalPrescribedDose = ((PlanSetup)planningItem).TotalPrescribedDose.Dose;
                this.structureSet = ((PlanSetup)planningItem).StructureSet;
            }
            else if (planningItem is PlanSum)
            {
                IsPlanSum = true;
                var planSetups = ((PlanSum)planningItem).PlanSetups;
                var planSetup = planSetups.First();
                SystemDoseUnit = planSetup.TotalPrescribedDose.Unit;
                SystemTotalPrescribedDose = 0.0;
                foreach (var p in planSetups)
                {
                    SystemTotalPrescribedDose += p.TotalPrescribedDose.Dose;
                }
                this.structureSet = ((PlanSum)planningItem).StructureSet;
            }

            setMaxDose();

            StructureNamesTps.Clear();
            StructureNamesTps.Add(new StructureName() { Id = string.Empty });
            foreach (var ss in structureSet.Structures)
            {
                if (ss.IsEmpty)
                {
                    continue;
                }

                StructureNamesTps.Add(new StructureName() { Id = ss.Id });
            }

            StoredDvhs.Clear();
            foreach (var objective in protocol.Objectives)
            {
                var objectiveVM = new DvhObjectiveViewModel(objective, planningItem, IsPlanSum, systemTotalPrescribedDose, SystemDoseUnit, StoredDvhs);
                Objectives.Add(objectiveVM);
            }
        }

        private PlanningItem getPlanningItemFromPlanName(PlanName planName)
        {
            PlanningItem pItem = null;
            foreach (var p in planningItems)
            {
                if (p is PlanSetup)
                {
                    if (((PlanSetup)p).Id == planName.PlanId && ((PlanSetup)p).Course.Id == planName.CourseId)
                    {
                        pItem = p;
                        break;
                    }
                }
                else if (p is PlanSum)
                {
                    if (((PlanSum)p).Id == planName.PlanId && ((PlanSum)p).Course.Id == planName.CourseId)
                    {
                        pItem = p;
                        break;
                    }
                }
            }
            return pItem;
        }

        private void setPlanningItemFromPlanName(PlanName planName)
        {
            this.planningItem = getPlanningItemFromPlanName(planName);
        }

        // Should be called after SystemTotalPrescribedDose is set
        private void setMaxDose()
        {
            double maxDose = planningItem.Dose.DoseMax3D.Dose;
            if (planningItem.Dose.DoseMax3D.Unit == DoseValue.DoseUnit.Percent)
            {
                maxDose = systemTotalPrescribedDose * maxDose / 100;
            }
            this.MaxDose = maxDose;
        }

        public DvhObjectivesViewModel(List<DvhObjective> objectives, PlanningItem planningItem, StructureSet structureSet)
        {
            this.planningItem = planningItem;
            this.structureSet = structureSet;

            StructureNamesTps.Add(new StructureName() { Id = string.Empty });
            foreach (var ss in structureSet.Structures)
            {
                if (ss.IsEmpty)
                {
                    continue;
                }

                StructureNamesTps.Add(new StructureName() { Id = ss.Id });
            }

            if (planningItem is PlanSetup)
            {
                IsPlanSum = false;
                SystemDoseUnit = ((PlanSetup)planningItem).TotalPrescribedDose.Unit;
                systemTotalPrescribedDose = ((PlanSetup)planningItem).TotalPrescribedDose.Dose;
            }
            else if (planningItem is PlanSum)
            {
                IsPlanSum = true;
                var planSetups = ((PlanSum)planningItem).PlanSetups;
                var planSetup = planSetups.First();
                SystemDoseUnit = planSetup.TotalPrescribedDose.Unit;
                systemTotalPrescribedDose = 0.0;
                foreach (var p in planSetups)
                {
                    systemTotalPrescribedDose += p.TotalPrescribedDose.Dose;
                }
            }
            // Initial assignment of assignedTotalPrescribedDose
            assignedTotalPrescribedDoseTps = systemTotalPrescribedDose;

            var objectiveVMs = new ObservableCollection<DvhObjectiveViewModel>();
            foreach (var objective in objectives)
            {
                var structure = EsapiHelpers.GetFilledStructure(structureSet, objective.StructureName);
                var objectiveVM = new DvhObjectiveViewModel(objective, planningItem, IsPlanSum, systemTotalPrescribedDose, SystemDoseUnit, structure);
                objectiveVMs.Add(objectiveVM);
            }

            this.Objectives = objectiveVMs;
        }

        public DvhObjectivesViewModel(Patient patient, PlanningItem planningItem, string folderPath)
        {
            if (patient == null)
            {
                throw new InvalidOperationException("No patient in the context");
            }

            Patient = patient;
            FolderPath = folderPath;

            var courses = patient.Courses;
            var numPlans = 0;
            foreach (var c in courses)
            {
                var plans = c.PlanSetups;
                foreach (var p in plans)
                {
                    if (p.IsDoseValid == false)
                    {
                        continue;
                    }
                    planningItems.Add((PlanningItem)p);
                    PlanNames.Add(new PlanName() { Id = numPlans, PlanId = p.Id, CourseId = c.Id });
                    numPlans += 1;
                }

                var planSums = c.PlanSums;
                foreach (var p in planSums)
                {
                    var setups = p.PlanSetups;
                    bool isDoseValid = true;
                    foreach (var s in setups)
                    {
                        if (s.IsDoseValid == false)
                        {
                            isDoseValid = false;
                            break;
                        }
                    }
                    if (isDoseValid == false)
                    {
                        continue;
                    }
                    planningItems.Add((PlanningItem)p);
                    PlanNames.Add(new PlanName() { Id = numPlans, PlanId = p.Id, CourseId = c.Id });
                    numPlans += 1;
                }
            }

            if (PlanNames.Count() == 0)
            {
                throw new InvalidOperationException("No plans with valid dose");
            }


            // check if plannigItem  has valid dose
            if (planningItem is PlanSetup)
            {
                if (((PlanSetup)planningItem).IsDoseValid == false)
                {
                    planningItem = null;
                }
            }
            else if (planningItem is PlanSum)
            {
                foreach (var p in ((PlanSum)planningItem).PlanSetups)
                {
                    if (p.IsDoseValid == false)
                    {
                        planningItem = null;
                        break;
                    }
                }
            }
            else
            {
                planningItem = null;
            }

            if (planningItem == null)
            {
                var selectPlanVM = new SelectPlanViewModel() { SelectedPlanNameId = 0, PlanNames = PlanNames };
                var selectPlanWindow = new SelectPlanView(selectPlanVM);
                selectPlanWindow.ShowDialog();
                var planName = (from p in PlanNames where p.Id == selectPlanVM.SelectedPlanNameId select p).Single();
                planningItem = getPlanningItemFromPlanName(planName);
            }

            // Planning Item
            this.planningItem = planningItem;

            Course course = null;
            if (planningItem is PlanSetup)
            {
                IsPlanSum = false;
                SystemDoseUnit = ((PlanSetup)planningItem).TotalPrescribedDose.Unit;
                systemTotalPrescribedDose = ((PlanSetup)planningItem).TotalPrescribedDose.Dose;
                this.structureSet = ((PlanSetup)planningItem).StructureSet;
                course = ((PlanSetup)planningItem).Course;
            }
            else if (planningItem is PlanSum)
            {
                IsPlanSum = true;
                var planSetups = ((PlanSum)planningItem).PlanSetups;
                var planSetup = planSetups.First();
                SystemDoseUnit = planSetup.TotalPrescribedDose.Unit;
                systemTotalPrescribedDose = 0.0;
                foreach (var p in planSetups)
                {
                    systemTotalPrescribedDose += p.TotalPrescribedDose.Dose;
                }
                this.structureSet = ((PlanSum)planningItem).StructureSet;
                course = ((PlanSum)planningItem).Course;
            }
            // Initial assignment of assignedTotalPrescribedDose
            assignedTotalPrescribedDoseTps = systemTotalPrescribedDose;

            setMaxDose();

            var planQuery = from p in PlanNames where (p.PlanId == planningItem.Id && p.CourseId == course.Id) select p;
            if (planQuery.Count() > 0)
            {
                selectedPlanName = planQuery.First();
            }
            else
            {
                selectedPlanName = PlanNames.First();
            }
            setPlanningItemFromPlanName(selectedPlanName);
            selectedPlanNameId = selectedPlanName.Id;

            StructureNamesTps.Add(new StructureName() { Id = string.Empty });
            foreach (var ss in structureSet.Structures)
            {
                if (ss.IsEmpty)
                {
                    continue;
                }

                StructureNamesTps.Add(new StructureName() { Id = ss.Id });
            }

            // Template
            var templateFolderPath = Path.Combine(folderPath, Path.Combine("DvhChecker", "templates"));
            var protocols = Protocol.GetProtocolsFromFolder(templateFolderPath);
            string defaultProtocolId = "Default";

            string patientId = this.Patient.Id;
            string courseId = CourseId;
            string planId = PlanId;
            string planFolderPath = Path.Combine(this.FolderPath, Path.Combine(patientId, Path.Combine(planId, "PlanCheckData")));

            if (Directory.Exists(planFolderPath))
            {
                //TODO: Assuming the existence of LastSaved_sjis.csv
                var savedProtocolFiles = Directory.GetFiles(planFolderPath, "LastSaved_sjis.csv");

                foreach (var savedProtocolFile in savedProtocolFiles)
                {
                    var objectives = DvhObjective.ReadObjectivesFromCsv(savedProtocolFile);
                    var id = (objectives.First()).ProtocolId;
                    var protocol = new Protocol(id, savedProtocolFile, objectives);
                    protocols.Add(protocol);
                    //TODO: Better solution?
                    defaultProtocolId = id;
                }

                //var savedProtocols = new List<Protocol>();
                //savedProtocols = Protocol.GetProtocolsFromFolder(planFolderPath);
                //if (savedProtocols.Count() > 0)
                //{
                //    protocols.AddRange(savedProtocols);
                //    defaultProtocolId = savedProtocols.First().Id;
                //}
            }

            //TODO: Implement a code to manage a duplicative Protocol.Id
            foreach (var protocol in protocols)
            {
                ProtocolNames.Add(new ProtocolName() { Id = protocol.Id });
            }

            this.Protocols = protocols;
            var query = from p in protocols where p.Id == defaultProtocolId select p;
            if (query.Count() >= 1)
            {
                this.protocol = query.First();
            }
            else
            {
                this.protocol = protocols.First();
            }

            //updateProtocol is called in Setter of SelectedProtocolName
            SelectedProtocolName = this.protocol.Id;
        }
    }

    public class StructureName
    {
        public string Id { get; set; }
        public override string ToString()
        {
            return Id;
        }
    }

    public class ProtocolName
    {
        public string Id { get; set; }
        public override string ToString()
        {
            return Id;
        }
    }

    public class PlanName
    {
        public int Id { get; set; }
        public string PlanId { get; set; }
        public string CourseId { get; set; }
        public override string ToString()
        {
            return $"{CourseId}: {PlanId}";
        }
    }

    public class SelectPlanViewModel
    {
        public List<PlanName> PlanNames { get; set; }
        public int SelectedPlanNameId { get; set; }
    }
}
