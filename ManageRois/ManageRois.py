#from connect import *

import clr
import sys, math, wpf, os
from System.Collections.Generic import List;
from System.Windows import MessageBox

clr.AddReference("PresentationFramework")
clr.AddReference("PresentationCore")

RayStationScriptsPath = os.environ["USERPROFILE"] + r"\DeskTop\RayStationScripts" + "\\"

dllsPath = RayStationScriptsPath + "Dlls"
print(dllsPath)
sys.path.append(dllsPath)

scriptsPath = RayStationScriptsPath + "Scripts"
print(scriptsPath)
sys.path.append(scriptsPath)

clr.AddReference("RoiManager")

from RoiManager.ViewModels import ExaminationSelectionViewModel

examinations = ExaminationSelectionViewModel()

examinationNames = List[str]()
examinationNames.Add("CT1")
examinationNames.Add("CT2")
examinationNames.Add("CT3")

examinations.CanExecute = False
examinations.ExaminationNames = examinationNames

from RoiManager.Views import ExaminationSelectionView

examinationSelectionWindow = ExaminationSelectionView(examinations)
examinationSelectionWindow.ShowDialog();

print "Selected Examination: ", examinations.SelectedExamination
print "CanExecute: ", examinations.CanExecute

from RoiManager.Models import Roi
rois = List[Roi]()

# Add a new Roi
roi = Roi()
roi.Name = "PTV-CTV"
roi.IsDerived = True
dependentRois = List[str]()
dependentRois.Add("PTV")
dependentRois.Add("CTV")
roi.DependentRois = dependentRois
roi.HasGeometry = True
roiCanDeleteGeometry = False
roi.CanUnderive = False
roi.CaseName = "C1"
roi.ExaminationName = "CT1"
rois.Add(roi)

# Add a new Roi
roi = Roi()
roi.Name = "PTV"
roi.IsDerived = False
roi.DependentRois =List[str]()
roi.HasGeometry = True
roiCanDeleteGeometry = False
roi.CanUnderive = False
roi.CaseName = "C1"
roi.ExaminationName = "CT1"

rois.Add(roi)

from RoiManager.ViewModels import RoiSelectionViewModel

roiSelectionViewModel = RoiSelectionViewModel(rois)

from RoiManager.Views import MainWindow
mainWindow = MainWindow(roiSelectionViewModel)
mainWindow.ShowDialog();

print "CanExecute: ", roiSelectionViewModel.CanExecute

for r in rois:
    print r
pass