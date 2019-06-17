from connect import *

import clr
import sys, math, wpf, os
from System.Collections.Generic import List;
from System.Windows import MessageBox

clr.AddReference("PresentationFramework")
clr.AddReference("PresentationCore")

RayStationScriptsPath = os.environ["USERPROFILE"] + r"\DeskTop\RayStationScripts" + "\\"

dllsPath = RayStationScriptsPath + "Dlls"
#print(dllsPath)
sys.path.append(dllsPath)

scriptsPath = RayStationScriptsPath + "Scripts"
#print(scriptsPath)
sys.path.append(scriptsPath)

clr.AddReference("RoiManager")

from RoiManager.ViewModels import ExaminationSelectionViewModel

case = get_current("Case")

examinations = ExaminationSelectionViewModel()

examinationNames = List[str]()

for e in case.Examinations:
    examinationNames.Add(e.Name)
    print(e.Name)

examinations.CanExecute = False
examinations.ExaminationNames = examinationNames
from RoiManager.Views import ExaminationSelectionView
examinationSelectionWindow = ExaminationSelectionView(examinations)
examinationSelectionWindow.ShowDialog();

print "Selected Examination: ", examinations.SelectedExamination
print "ExaminationSelection CanExecute: ", examinations.CanExecute
if not examinations.CanExecute:
    print "Canceled"
    sys.exit()

roiGeometries = case.PatientModel.StructureSets[examinations.SelectedExamination].RoiGeometries

from RoiManager.Models import Roi
rois = List[Roi]()

for r in roiGeometries:
    roi = Roi()
    ofRoi = r.OfRoi
    roi.Name = ofRoi.Name
    if ofRoi.DerivedRoiExpression is not None:
        roi.IsDerived = True
        roi.DependentRois = r.GetDependentRois()
    else:
        roi.IsDerived = False
        roi.DependentRois = List[str]()

    roi.HasGeometry = r.HasContours()
    roi.CanDeleteGeometry = False
    roi.CanUnderive = False
    roi. CanDeleteGeometry = False
    roi.CaseName = case.CaseName
    roi.ExaminationName = examinations.SelectedExamination
    rois.Add(roi)

from RoiManager.ViewModels import RoiSelectionViewModel
roiSelectionViewModel = RoiSelectionViewModel(rois)

from RoiManager.Views import MainWindow
mainWindow = MainWindow(roiSelectionViewModel)
mainWindow.ShowDialog();

print "RoiSelection CanExecute: ", roiSelectionViewModel.CanExecute

if not roiSelectionViewModel.CanExecute:
    print "Canceled"
    sys.exit()

for r in rois:
    roiGeometry = roiGeometries[r.Name]
    ofRoi = roiGeometry.OfRoi
    if r.IsDerived and r.CanUnderive:
        ofRoi.DeleteExpression()
    if r.HasGeometry and r.CanDeleteGeometry:
        roiGeometry.DeleteGeometry()
    if r.CanDeleteRoi:
        ofRoi.DeleteRoi()
pass



