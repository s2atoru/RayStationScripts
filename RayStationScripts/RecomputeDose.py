from connect import *
import wpf
from System.Windows import MessageBox

case = get_current('Case')
plan = get_current('Plan')
planName = plan.Name
newPlanName = planName + ' Dose'
case.CopyPlan(PlanName=planName, NewPlanName=newPlanName)
newPlan = case.TreatmentPlans[newPlanName]
for bs in newPlan.BeamSets:
    bs.ComputeDose(ComputeBeamDoses=True, DoseAlgorithm='CCDose', ForceRecompute=True)

MessageBox.Show('Done: Copy plan and Recompute dose')
