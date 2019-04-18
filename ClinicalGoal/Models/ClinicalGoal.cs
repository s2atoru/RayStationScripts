using Juntendo.MedPhys;

namespace ClinicalGoal.Models
{
    public class ClinicalGoal
    {
        public string RoiName { get; set; }
        public string GoalCriteria { get; set; }
        public string GoalType { get; set; }
        public double AcceptanceLevel { get; set; }
        public double ParameterValue { get; set; }
        public bool IsComparativeGoal { get; set; } = false;
        public int Priority { get; set; } = 2147483647;

        public override string ToString()
        {
            return $"RoiName: {RoiName}, GoalCriteria: {GoalCriteria}, GoalType: {GoalType}, AcceptanceLevel: {AcceptanceLevel}, "
                + $"ParameterValue: {ParameterValue}, IsComparativeGoal: {IsComparativeGoal}, Priority: {Priority}";
        }

        /// <summary>
        /// Constructor from DvhObjective
        /// </summary>
        /// <param name="dvhObjective"> DvhObjective </param>
        /// <param name="prescribedDose"> Prescribed dose in cGy </param>
        /// <param name="volume"> volume in cc </param>
        public ClinicalGoal(DvhObjective dvhObjective, double prescribedDose = 0, double volume = 0)
        {

            RoiName = dvhObjective.StructureNameTps;

            switch (dvhObjective.TargetType)
            {
                case DvhTargetType.Dose:

                    if (dvhObjective.TargetUnit == DvhPresentationType.Rel)
                    {
                        //% to absolute dose (prescribed dose is given in cGy)
                        AcceptanceLevel = prescribedDose * dvhObjective.TargetValue / 100;
                    }
                    else if (dvhObjective.TargetUnit == DvhPresentationType.Abs)
                    {
                        //Gy to cGy
                        AcceptanceLevel = dvhObjective.TargetValue * 100;
                    }

                    switch (dvhObjective.ObjectiveType)
                    {

                        case DvhObjectiveType.Max:

                            GoalCriteria = "AtMost";
                            GoalType = "DoseAtVolume";
                            ParameterValue = 0;

                            break;
                        case DvhObjectiveType.Min:

                            GoalCriteria = "AtLeast";
                            GoalType = "DoseAtVolume";
                            ParameterValue = 1;

                            break;

                        case DvhObjectiveType.MeanUpper:

                            GoalCriteria = "AtMost";
                            GoalType = "AverageDose";
                            ParameterValue = 0;

                            break;
                        case DvhObjectiveType.MeanLower:

                            GoalCriteria = "AtLeast";
                            GoalType = "AverageDose";
                            ParameterValue = 0;

                            break;
                        case DvhObjectiveType.Upper:

                            GoalCriteria = "AtMost";
                            if (dvhObjective.ArgumentUnit == DvhPresentationType.Rel)
                            {
                                GoalType = "DoseAtVolume";
                                ParameterValue = dvhObjective.ArgumentValue / 100;
                            }
                            else if (dvhObjective.ArgumentUnit == DvhPresentationType.Abs)
                            {
                                GoalType = "DoseAtAbsoluteVolume";
                                ParameterValue = dvhObjective.ArgumentValue;
                            }

                            break;
                        case DvhObjectiveType.Lower:

                            GoalCriteria = "AtLeast";
                            if (dvhObjective.ArgumentUnit == DvhPresentationType.Rel)
                            {
                                GoalType = "DoseAtVolume";
                                ParameterValue = dvhObjective.ArgumentValue / 100;
                            }
                            else if (dvhObjective.ArgumentUnit == DvhPresentationType.Abs)
                            {
                                GoalType = "DoseAtAbsoluteVolume";
                                ParameterValue = dvhObjective.ArgumentValue;
                            }

                            break;

                        default:
                            break;
                    }

                    break;
                case DvhTargetType.Volume:

                    if (dvhObjective.TargetUnit == DvhPresentationType.Rel)
                    {
                        AcceptanceLevel = dvhObjective.TargetValue / 100;
                        GoalType = "VolumeAtDose";
                    }
                    else if (dvhObjective.TargetUnit == DvhPresentationType.Abs)
                    {
                        AcceptanceLevel = dvhObjective.TargetValue;
                        GoalType = "AbsoluteVolumeAtDose";
                    }

                    if (dvhObjective.ArgumentUnit == DvhPresentationType.Rel)
                    {
                        //prescribed dose is given in cGy
                        ParameterValue = prescribedDose * dvhObjective.ArgumentValue / 100;
                    }
                    else if (dvhObjective.ArgumentUnit == DvhPresentationType.Abs)
                    {
                        //Gy to cGy
                        ParameterValue = dvhObjective.ArgumentValue * 100;
                    }


                    switch (dvhObjective.ObjectiveType)
                    {
                        case DvhObjectiveType.Upper:

                            GoalCriteria = "AtMost";

                            break;
                        case DvhObjectiveType.Lower:

                            GoalCriteria = "AtLeast";

                            break;
                        case DvhObjectiveType.Spare:

                            GoalCriteria = "AtMost";
                            if (dvhObjective.TargetUnit == DvhPresentationType.Rel)
                            {
                                AcceptanceLevel = 1 - AcceptanceLevel;
                            }
                            else if (dvhObjective.TargetUnit == DvhPresentationType.Rel)
                            {
                                AcceptanceLevel = volume - AcceptanceLevel;
                                // 0 if acceptance level is larger than volume
                                if (AcceptanceLevel < 0)
                                {
                                    AcceptanceLevel = 0;
                                }
                            }
                            break;
                        default:
                            break;
                    }

                    break;
                default:
                    break;
            }
        }
    }
}
