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

#plan = get_current('Plan').VerificationPlans[0]
#plan = get_current('Plan')
beamSet = get_current('BeamSet')
print beamSet.DicomPlanLabel
fractionDose = beamSet.FractionDose

#x = -0.05
#y = 0.05
#z = 0.0

x = -1
y = 4.6
z = 0.3

beamDoses = beamSet.FractionDose.BeamDoses
sum = 0
for beamDose in beamDoses:
    point = {'x':x, 'y':y, 'z':z}
    dose = beamDose.InterpolateDoseInPoint(Point=point)
    print beamDose.ForBeam.Name, dose
    sum += dose

print sum
