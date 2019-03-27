from connect import *

import sys, math, wpf
from System.Windows import MessageBox

def ScaleDoseBeamSet(beamSet):
    roiName = beamSet.Prescription.PrimaryDosePrescription.OnStructure.Name
    doseValue = beamSet.Prescription.PrimaryDosePrescription.DoseValue
    doseVolume = beamSet.Prescription.PrimaryDosePrescription.DoseVolume
    prescriptionType = beamSet.Prescription.PrimaryDosePrescription.PrescriptionType
    lockedBeamNames = None
    evaluateAfterScaling = True
    beamSet.NormalizeToPrescription(RoiName=roiName, DoseValue=doseValue, DoseVolume=doseVolume, PrescriptionType=prescriptionType, LockedBeamNames=lockedBeamNames, EvaluateAfterScaling=evaluateAfterScaling)

def ScaleDoseBeamSets(plan, beamSet):

    dicomPlanLabel = beamSet.DicomPlanLabel
    planOptimizations = plan.PlanOptimizations

    with CompositeAction('Scale Dose Beam Sets ({0})'.format(dicomPlanLabel)):

        for planOptimization in planOptimizations:
            optimizedBeamSets = planOptimization.OptimizedBeamSets
    
            #print type(optimizedBeamSets), dicomPlanLabel, dicomPlanLabel in optimizedBeamSets
    
            isPlanIncluded = False
            for optimizedBeamSet in optimizedBeamSets:
                #print optimizedBeamSet.DicomPlanLabel
                if(optimizedBeamSet.DicomPlanLabel == dicomPlanLabel):
                    isPlanIncluded = True
                    break
     
            if(isPlanIncluded):
                for optimizedBeamSet in optimizedBeamSets:
                    message = "Scale Dose of {0}".format(optimizedBeamSet.DicomPlanLabel)
                    #MessageBox.Show(message)
                    print message
                    ScaleDoseBeamSet(optimizedBeamSet)

      # CompositeAction ends

if __name__ == '__main__':
    plan = get_current("Plan")
    beamSet = get_current("BeamSet")
    dicomPlanLabel = beamSet.DicomPlanLabel
    #with CompositeAction('Scale Dose Beam Sets ({0})'.format(dicomPlanLabel)):

        #ScaleDoseBeamSets(plan, beamSet)

      # CompositeAction ends

    ScaleDoseBeamSets(plan, beamSet)
