from connect import *

import clr
import sys, math, wpf, os
from System.Collections.Generic import List;
from System.Windows import MessageBox

RayStationScriptsPath = os.environ["USERPROFILE"] + r"\DeskTop\RayStationScripts" + "\\"

dllsPath = RayStationScriptsPath + "Dlls"
#print(dllsPath)
sys.path.append(dllsPath)

scriptsPath = RayStationScriptsPath + "Scripts"
sys.path.append(scriptsPath)

clr.AddReference("RoiCenterOfMass")

plan = get_current("Plan")
ss = plan.GetStructureSet()
rois = ss.RoiGeometries

from RoiCenterOfMass.Models import RoiCenterOfMass
from RoiCenterOfMass.Views import MainWindow

roiCenterOfMasses = List[RoiCenterOfMass]()

xsum = 0
ysum = 0
zsum = 0
numRois = 0
for roi in rois:    
    name = roi.OfRoi.Name

    typeOfRoi = roi.OfRoi.Type
    if not typeOfRoi == "Ptv":
        continue

    hasContours = bool(roi.HasContours())
    if not hasContours:
        continue

    print "Name: {0}, Type: {1}, HasContours: {2}".format(name, typeOfRoi, hasContours)

    
    center = roi.GetCenterOfRoi()
    roiCenterOfMasses.Add(RoiCenterOfMass(name, center.x, -center.y, center.z))
    print "{0}: {1:.2f}, {2:.2f}, {3:.2f}".format(name, center.x, center.y, center.z)
    numRois += 1
    xsum += center.x
    ysum += center.y
    zsum += center.z

if len(roiCenterOfMasses) == 0:
    MessageBox.Show('No PTV')
    sys.exit()
        

for r in roiCenterOfMasses:
    print "ID: {0}, Coordinates: ({1:.2f}, {3:.2f}, {2:.2f}), InUse: {4}, IsVisible: {5}".format(r.Id, r.Coordinates.X, r.Coordinates.Y, r.Coordinates.Z, r.InUse, r.IsVisible) 

mainWindow = MainWindow(roiCenterOfMasses)
mainWindow.ShowDialog()