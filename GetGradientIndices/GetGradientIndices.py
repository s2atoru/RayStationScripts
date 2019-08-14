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
print "Dlls path: " + dllsPath
sys.path.append(dllsPath)

scriptsPath = RayStationScriptsPath + "Scripts"
print "Scripts path: " + scriptsPath
sys.path.append(scriptsPath)

clr.AddReference("BrainDoseIndices")
from BrainDoseIndices.Views import MainWindow
from BrainDoseIndices.Models import StructureDetail

from Helpers import GetStructureSet, GetRoiDetails
from Helpers import MakeMarginAddedRoi, MakeRingRoi, MakeRoiSubtractedRoi

try:
    plan = get_current("Plan")
except:
    MessageBox.Show("Plan is not selected. Select Plan")
    sys.exit()

structureSet = plan.GetStructureSet()
roiDetails = GetRoiDetails(structureSet)

structureDetails = List[StructureDetail]()
for key, value in roiDetails.items():
    if value["HasContours"]:
        structureDetail = StructureDetail();
        structureDetail.Name = key
        structureDetail.Volume = value["Volume"]
        structureDetails.Add(structureDetail)

mainWindow = MainWindow(structureDetails)
mainWindow.ShowDialog();

