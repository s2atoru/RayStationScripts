
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;
using VMS.TPS;

using Prism.Mvvm;
using System.Linq;
using System.Collections.Generic;
using System;

namespace Juntendo.MedPhys.Esapi
{
    public class DvhObjectiveViewModel : BindableBase
    {

        private bool isVisible = true;
        public bool IsVisible
        {
            get { return isVisible; }
            set
            {
                this.SetProperty(ref this.isVisible, value);
            }
        }

        private DvhObjective objective;
        public DvhObjective Objective
        {
            get { return objective; }
            private set
            {
                this.SetProperty(ref this.objective, value);
            }
        }

        private double totalPrescribedDoseTps;
        public double TotalPrescribedDoseTps
        {
            get { return totalPrescribedDoseTps; }
            set
            {
                if (this.SetProperty(ref this.totalPrescribedDoseTps, value))
                {
                    updateObjective();
                }
            }
        }

        private double systemTotalPrescribedDoseTps;
        public double SystemTotalPrescribedDoseTps
        {
            get { return systemTotalPrescribedDoseTps; }
            set
            {
                this.SetProperty(ref this.systemTotalPrescribedDoseTps, value);
            }
        }

        public DoseValue.DoseUnit DoseUnitTps { get; private set; }

        private string structureNameTps;
        public string StructureNameTps
        {
            get { return structureNameTps; }
            set
            {
                if(this.SetProperty(ref this.structureNameTps, value))
                {
                    objective.StructureNameTps = value;
                    updateStructure(value);
                    foreach (var sameStructureObjective in SameStrutureObjectives)
                    {
                        sameStructureObjective.StructureNameTps = value;
                    }
                }
            }
        }

        private Structure structure;
        public Structure Structure
        {
            get { return structure; }
            set
            {
                this.SetProperty(ref this.structure, value);
            }
        }

        public bool IsPlanSum { get; private set; } = false;

        public PlanningItem planningItem;
        public PlanningItem PlanningItem
        {
            get { return planningItem; }
            set
            {
                if(this.SetProperty(ref this.planningItem, value))
                {
                    setInitialTotalPrescribedDoseTps();
                    updateStructureSet();
                    updateObjective();
                }
            }
        }

        private StructureSet structureSet;
        public StructureSet StructureSet
        {
            get { return structureSet; }
            private set
            {
                if (this.SetProperty(ref this.structureSet, value))
                {
                    updateStructure();
                }
            }
        }

        private string targetUnit;
        public string TargetUnit
        {
            get { return targetUnit; }
            set { this.SetProperty(ref this.targetUnit, value); }
        }

        private string argumentUnit;
        public string ArgumentUnit
        {
            get { return argumentUnit; }
            set { this.SetProperty(ref this.argumentUnit, value); }
        }

        private string objectiveType;
        public string ObjectiveType
        {
            get { return objectiveType; }
            set { this.SetProperty(ref this.objectiveType, value); }
        }

        //TODO: Add conditional branching
        private double PassingTol = 0.001;

        public List<DvhObjectiveViewModel> SameStrutureObjectives { get; private set; } = new List<DvhObjectiveViewModel>();

        //Stored DVH to prevent multiple GetDVHCumulativeData calls
        public Dictionary<DvhDataParameters, DVHData> StoredDvhs { get; set; }

        public void SetSameStructureObjectives(List<DvhObjectiveViewModel> objectives)
        {
            SameStrutureObjectives.Clear();
            foreach (var obj in objectives)
            {
                if (object.ReferenceEquals(this, obj))
                {
                    continue;
                }

                if (this.objective.StructureName == obj.objective.StructureName)
                {
                    SameStrutureObjectives.Add(obj);
                }

            }
        }

        public DvhObjectiveViewModel(DvhObjective objective, PlanningItem planningItem, bool isPlanSum, double totalPrescribedDoseTps, DoseValue.DoseUnit doseUnitTps, Structure structure)
        {
            this.objective = objective;
            this.planningItem = planningItem;
            this.IsPlanSum = isPlanSum;
            this.totalPrescribedDoseTps = totalPrescribedDoseTps;
            this.systemTotalPrescribedDoseTps = totalPrescribedDoseTps;
            this.DoseUnitTps = doseUnitTps;
            this.structure = structure;

            updateStructureSet();

            if (structure != null && !structure.IsEmpty)
            {
                structureNameTps = structure.Id;
                objective.StructureNameTps = structure.Id;
                objective.Volume = structure.Volume;
            }
            else
            {
                structureNameTps = string.Empty;
                objective.StructureNameTps = string.Empty;
                objective.Volume = double.NaN;
            }

            updateObjective();
        }

        public DvhObjectiveViewModel(DvhObjective objective, PlanningItem planningItem, bool isPlanSum, double totalPrescribedDoseTps, DoseValue.DoseUnit doseUnitTps)
        {
            this.objective = objective;
            this.planningItem = planningItem;
            this.IsPlanSum = isPlanSum;
            this.totalPrescribedDoseTps = totalPrescribedDoseTps;
            this.systemTotalPrescribedDoseTps = totalPrescribedDoseTps;
            this.DoseUnitTps = doseUnitTps;

            if (objective.TargetType == DvhTargetType.Dose)
            {
                targetUnit = getDoseUnit(objective.DoseUnit);
                argumentUnit = getVolumeUnit(objective.VolumeUnit);
            }
            else if (objective.TargetType == DvhTargetType.Volume)
            {
                targetUnit = getVolumeUnit(objective.VolumeUnit);
                argumentUnit = getDoseUnit(objective.DoseUnit);
            }

            objectiveType = getObjectiveType(objective.ObjectiveType);

            updateStructureSet();
            updateObjective();
        }

        public DvhObjectiveViewModel(DvhObjective objective, PlanningItem planningItem, bool isPlanSum, double totalPrescribedDoseTps, DoseValue.DoseUnit doseUnitTps, 
            Dictionary<DvhDataParameters,DVHData> storedDvhs)
        {
            this.objective = objective;
            this.planningItem = planningItem;
            this.IsPlanSum = isPlanSum;
            this.totalPrescribedDoseTps = totalPrescribedDoseTps;
            this.systemTotalPrescribedDoseTps = totalPrescribedDoseTps;
            this.DoseUnitTps = doseUnitTps;
            this.StoredDvhs = storedDvhs;

            if (objective.TargetType == DvhTargetType.Dose)
            {
                targetUnit = getDoseUnit(objective.DoseUnit);
                argumentUnit = getVolumeUnit(objective.VolumeUnit);
            }
            else if (objective.TargetType == DvhTargetType.Volume)
            {
                targetUnit = getVolumeUnit(objective.VolumeUnit);
                argumentUnit = getDoseUnit(objective.DoseUnit);
            }

            objectiveType = getObjectiveType(objective.ObjectiveType);

            updateStructureSet();
            updateObjective();
        }

        private string getDoseUnit(DvhDoseUnit doseUnit)
        {
            switch (doseUnit)
            {
                case DvhDoseUnit.None:
                    return string.Empty;
                case DvhDoseUnit.Percent:
                    return "%";
                case DvhDoseUnit.Gy:
                    return "Gy";
                default:
                    return string.Empty;
            }
        }

        private string getVolumeUnit(DvhVolumeUnit volumeUnit)
        {
            switch (volumeUnit)
            {
                case DvhVolumeUnit.None:
                    return string.Empty;
                case DvhVolumeUnit.Percent:
                    return "%";
                case DvhVolumeUnit.cc:
                    return "cc";
                default:
                    return string.Empty;
            }
        }

        private string getObjectiveType(DvhObjectiveType objectiveType)
        {
            switch (objectiveType)
            {
                case DvhObjectiveType.Max:
                    return "<";
                case DvhObjectiveType.Min:
                    return ">";
                case DvhObjectiveType.MeanUpper:
                    return "<";
                case DvhObjectiveType.MeanLower:
                    return ">";
                case DvhObjectiveType.Upper:
                    return "<";
                case DvhObjectiveType.Lower:
                    return ">";
                case DvhObjectiveType.Spare:
                    return ">";
                default:
                    return string.Empty;
            }
        }

        private void updateObjective()
        {
            if(structure != null && !this.structure.IsEmpty)
            {
                objective.InUse = true;
                setDvhIndexValue();
                setIsPassed();
                setEvalResult();
            }
            else
            {
                objective.InUse = false;
                objective.Value = double.NaN;
                objective.IsPassed = false;
                objective.IsAcceptable = false;
                objective.EvalResult = DvhEvalResult.Na;
            }
        }

        private void updateStructure(string structureName)
        {
            structure = EsapiHelpers.GetFilledStructure(StructureSet, structureName);
            if(structure != null && !structure.IsEmpty)
            {
                objective.Volume = structure.Volume;
            }
            else
            {
                objective.Volume = double.NaN;
            }
            updateObjective();
        }

        private void updateStructure()
        {
            if (string.IsNullOrEmpty(objective.StructureNameTps))
            {
                Structure = EsapiHelpers.GetFilledStructure(StructureSet, objective.StructureName);
            }
            else
            {
                Structure = EsapiHelpers.GetFilledStructure(StructureSet, objective.StructureNameTps);
            }

            if (this.structure != null && !this.structure.IsEmpty)
            {
                structureNameTps = structure.Id;
                objective.StructureNameTps = structure.Id;
                objective.Volume = structure.Volume;
            }
            else
            {
                structureNameTps = string.Empty;
                objective.StructureNameTps = string.Empty;
                objective.Volume = double.NaN;
            }
        }

        private void updateStructureSet()
        {
            if (planningItem is PlanSetup)
            {
                StructureSet = ((PlanSetup)planningItem).StructureSet;
            }
            else if (planningItem is PlanSum)
            {
                StructureSet = ((PlanSum)planningItem).StructureSet;
            }
            else
            {
                StructureSet = null;
            }
        }

        private void setInitialTotalPrescribedDoseTps()
        {
            if (planningItem is PlanSetup)
            {
                IsPlanSum = false;
                DoseUnitTps = ((PlanSetup)planningItem).TotalPrescribedDose.Unit;
                totalPrescribedDoseTps = ((PlanSetup)planningItem).TotalPrescribedDose.Dose;
            }
            else if (planningItem is PlanSum)
            {
                IsPlanSum = true;
                var planSetups = ((PlanSum)planningItem).PlanSetups;
                var planSetup = planSetups.First();
                DoseUnitTps = planSetup.TotalPrescribedDose.Unit;
                totalPrescribedDoseTps = 0.0;
                foreach (var p in planSetups)
                {
                    totalPrescribedDoseTps += p.TotalPrescribedDose.Dose;
                }
            }
        }

        private void setDvhIndexValue()
        {
            double dvhIndex = double.NaN;

            double binWidth = 0.001;
            //Bin width
            if (DoseUnitTps == DoseValue.DoseUnit.cGy)
            {
                binWidth = 0.1;
            }

            if (objective.TargetType == DvhTargetType.Dose)
            {
                VolumePresentation volumePresentation = VolumePresentation.Relative;
                if (objective.VolumeUnit == DvhVolumeUnit.cc)
                {
                    volumePresentation = VolumePresentation.AbsoluteCm3;
                }

                DoseValuePresentation requestedDosePresentation = DoseValuePresentation.Relative;
                //DoseValuePresentation should be Absolute for Plan Sum
                if (objective.DoseUnit == DvhDoseUnit.Gy || IsPlanSum == true)
                {
                    requestedDosePresentation = DoseValuePresentation.Absolute;
                }

                DoseValue doseEsapi = DoseValue.UndefinedDose();
                switch (objective.ObjectiveType)
                {
                    case DvhObjectiveType.Upper:
                        doseEsapi = planningItem.GetDoseAtVolume(structure, objective.ArgumentValue, volumePresentation, requestedDosePresentation);
                        break;
                    case DvhObjectiveType.Lower:
                        doseEsapi = planningItem.GetDoseAtVolume(structure, objective.ArgumentValue, volumePresentation, requestedDosePresentation);
                        break;
                    case DvhObjectiveType.Max:
                        DVHData dvh = getDvh(requestedDosePresentation, volumePresentation, binWidth);
                        doseEsapi = dvh.MaxDose;
                        break;
                    case DvhObjectiveType.Min:
                        dvh = getDvh(requestedDosePresentation, volumePresentation, binWidth);
                        doseEsapi = dvh.MinDose;
                        break;
                    case DvhObjectiveType.MeanUpper:
                        dvh = getDvh(requestedDosePresentation, volumePresentation, binWidth);
                        doseEsapi = dvh.MeanDose;
                        break;
                    case DvhObjectiveType.MeanLower:
                        dvh = getDvh(requestedDosePresentation, volumePresentation, binWidth);
                        doseEsapi = dvh.MeanDose;
                        break;
                    default:
                        doseEsapi = DoseValue.UndefinedDose();
                        break;
                }

                dvhIndex = doseEsapi.Dose;
                // cGy to Gy
                if (doseEsapi.Unit == DoseValue.DoseUnit.cGy && objective.DoseUnit == DvhDoseUnit.Gy)
                {
                    dvhIndex /= 100.0;
                }

                // Plan Sum with DoseUnit Percent 
                if (IsPlanSum == true && objective.DoseUnit == DvhDoseUnit.Percent)
                {
                    // Absolute to Percent
                    dvhIndex /= SystemTotalPrescribedDoseTps;
                    dvhIndex *= 100.0;
                }

                // Make relative to TotalPrescribedDoseTps
                if (objective.DoseUnit == DvhDoseUnit.Percent)
                {
                    dvhIndex = dvhIndex * SystemTotalPrescribedDoseTps / TotalPrescribedDoseTps;
                }
            }
            else if (objective.TargetType == DvhTargetType.Volume)
            {
                VolumePresentation requestedVolumePresentation = VolumePresentation.Relative;
                if (objective.VolumeUnit == DvhVolumeUnit.cc)
                {
                    requestedVolumePresentation = VolumePresentation.AbsoluteCm3;
                }

                DoseValue dose = DoseValue.UndefinedDose();
                if (objective.DoseUnit == DvhDoseUnit.Gy)
                {
                    //cGy to Gy
                    if (DoseUnitTps == DoseValue.DoseUnit.cGy)
                    {
                        dose = new DoseValue(objective.ArgumentValue * 100, "cGy");
                    }
                    else if (DoseUnitTps == DoseValue.DoseUnit.Gy)
                    {
                        dose = new DoseValue(objective.ArgumentValue, "Gy");
                    }
                }
                else if (objective.DoseUnit == DvhDoseUnit.Percent)
                {
                    //if (IsPlanSum == true)
                    //{
                    //    dose = new DoseValue(objective.ArgumentValue * TotalPrescribedDoseTps, DoseUnitTps);
                    //}
                    //else
                    //{
                    //    //Make relative to SystemTotalPrrescribedDose
                    //    dose = new DoseValue(objective.ArgumentValue*TotalPrescribedDoseTps/SystemTotalPrescribedDoseTps, DoseValue.DoseUnit.Percent);
                    //}

                    dose = new DoseValue(objective.ArgumentValue * TotalPrescribedDoseTps/100, DoseUnitTps);
                }

                double volume = double.NaN;
                switch (objective.ObjectiveType)
                {
                    case DvhObjectiveType.Upper:
                        volume = planningItem.GetVolumeAtDose(structure, dose, requestedVolumePresentation);
                        break;
                    case DvhObjectiveType.Lower:
                        volume = planningItem.GetVolumeAtDose(structure, dose, requestedVolumePresentation);
                        break;
                    case DvhObjectiveType.Spare:
                        volume = planningItem.GetVolumeAtDose(structure, dose, requestedVolumePresentation);
                        if (requestedVolumePresentation == VolumePresentation.AbsoluteCm3)
                        {
                            volume = structure.Volume - volume;
                        }
                        else
                        {
                            volume = 100.0 - volume;
                        }
                        break;
                    case DvhObjectiveType.Veff:
                        DVHData dvh = getDvh(DoseValuePresentation.Absolute, VolumePresentation.Relative, binWidth);
                        var maxDose = dvh.MaxDose.Dose;
                        double veff = 0;
                        for (int i = 1; i<dvh.CurveData.Length; i++)
                        {
                            var d = dvh.CurveData[i].DoseValue.Dose;
                            var v = dvh.CurveData[i-1].Volume - dvh.CurveData[i].Volume;

                            if(d > maxDose)
                            {
                                break;
                            }

                            veff += Math.Pow(d / maxDose, 1 / objective.ArgumentValue) * v;
                        }
                        volume = veff;
                        break;
                    default:
                        volume = double.NaN;
                        break;
                }

                dvhIndex = volume;
            }

            this.objective.Value = dvhIndex;
        }

        private DVHData getDvh(DoseValuePresentation doseValuePresentation, VolumePresentation volumePresentation, double binWidth)
        {
            var dvhDataParameters = new DvhDataParameters { StructureId = Structure.Id, DoseValuePresentation = doseValuePresentation, VolumePresentation = volumePresentation, BinWidth = binWidth };
            if (StoredDvhs.ContainsKey(dvhDataParameters))
            {
                return StoredDvhs[dvhDataParameters];
            }
            else
            {
                var dvh = PlanningItem.GetDVHCumulativeData(Structure, doseValuePresentation, volumePresentation, binWidth);
                StoredDvhs[dvhDataParameters] = dvh;
                return dvh;
            }
        }

        private void setIsPassed()
        {
            bool isPassed = false;
            bool isAcceptable = false;

            double diff = objective.Value - objective.TargetValue;
            double diffAcceptable = diff;
            if (objective.AcceptableLimitValue > 0)
            {
                diffAcceptable = objective.Value - objective.AcceptableLimitValue;
            }
            
            switch (objective.ObjectiveType)
            {
                case (DvhObjectiveType.Lower):
                    if (diff >= -PassingTol)
                    {
                        isPassed = true;
                    }
                    else if (objective.AcceptableLimitValue > 0 && diffAcceptable >= -PassingTol)
                    {
                        isAcceptable = true;
                    }
                    break;
                case (DvhObjectiveType.Upper):
                    if (diff <= PassingTol)
                    {
                        isPassed = true;
                    }
                    else if (objective.AcceptableLimitValue > 0 && diffAcceptable <= PassingTol)
                    {
                        isAcceptable = true;
                    }
                    break;
                case (DvhObjectiveType.MeanLower):
                    if (diff >= -PassingTol)
                    {
                        isPassed = true;
                    }
                    else if (objective.AcceptableLimitValue > 0 && diffAcceptable >= -PassingTol)
                    {
                        isAcceptable = true;
                    }
                    break;
                case (DvhObjectiveType.MeanUpper):
                    if (diff <= PassingTol)
                    {
                        isPassed = true;
                    }
                    else if (objective.AcceptableLimitValue > 0 && diffAcceptable <= PassingTol)
                    {
                        isAcceptable = true;
                    }
                    break;
                case (DvhObjectiveType.Min):
                    if (diff >= -PassingTol)
                    {
                        isPassed = true;
                    }
                    else if (objective.AcceptableLimitValue > 0 && diffAcceptable >= -PassingTol)
                    {
                        isAcceptable = true;
                    }
                    break;
                case (DvhObjectiveType.Max):
                    if (diff <= PassingTol)
                    {
                        isPassed = true;
                    }
                    else if (objective.AcceptableLimitValue > 0 && diffAcceptable <= PassingTol)
                    {
                        isAcceptable = true;
                    }
                    break;
                case (DvhObjectiveType.Spare):
                    if (diff >= -PassingTol)
                    {
                        isPassed = true;
                    }
                    else if (objective.AcceptableLimitValue > 0 && diffAcceptable >= -PassingTol)
                    {
                        isAcceptable = true;
                    }
                    break;
                case (DvhObjectiveType.Veff):
                    isPassed = false;
                    isAcceptable = false;
                    break;
            }
            objective.IsPassed = isPassed;
            objective.IsAcceptable = isAcceptable;
        }

        private void setEvalResult()
        {
            if(!objective.InUse)
            {
                objective.EvalResult = DvhEvalResult.Na;
                return;
            }
            if(objective.IsPassed)
            {
                objective.EvalResult = DvhEvalResult.Pass;
                return;
            }
            else if (objective.IsAcceptable)
            {
                objective.EvalResult = DvhEvalResult.Acceptable;
                return;
            }
            else
            {
                objective.EvalResult = DvhEvalResult.Fail;
                return;
            }
        }
    }
}
