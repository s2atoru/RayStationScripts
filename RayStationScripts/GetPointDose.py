from connect import *

import clr
import wpf
clr.AddReference("PresentationFramework")
clr.AddReference("PresentationCore")

from System.Collections.Generic import List, Dictionary
from System.Windows import MessageBox

import sys, os
import json

RayStationScriptsPath = os.environ["USERPROFILE"] + r"\DeskTop\RayStationScripts" + "\\"

dllsPath = RayStationScriptsPath + "Dlls"
print(dllsPath)
sys.path.append(dllsPath)

scriptsPath = RayStationScriptsPath + "Scripts"
print(scriptsPath)
sys.path.append(scriptsPath)

clr.AddReference("RoiFormulaMaker")
from RoiFormulaMaker.Views import MainWindow
from RoiFormulaMaker.Models import RoiFormulas

from Helpers import GetDoseValueDCM

plan = get_current('Plan').VerificationPlans[0]
beamSet = plan.BeamSet
print beamSet.DicomPlanLabel
fractionDose = beamSet.FractionDose

doseData = fractionDose.DoseValues.DoseData
doseGrid = plan.DoseGrid

x = -0.05
y = 0.05
z = 0.0

dose = GetDoseValueDCM(x, y, z, doseGrid, doseData)
print dose

beamDoses = beamSet.FractionDose.BeamDoses
sum = 0
for beamDose in beamDoses:
    doseData = beamDose.DoseValues.DoseData
    dose =  GetDoseValueDCM(x, y, z, doseGrid, beamDose.DoseValues.DoseData)
    print beamDose.ForBeam.Name, dose
    sum += dose

print sum
