from connect import *

import clr
import sys, math, wpf, os
from System.Collections.Generic import List, Dictionary;
from System.Windows import MessageBox
from System.Windows import Media
from System import Enum

RayStationScriptsPath = os.environ["USERPROFILE"] + r"\DeskTop\RayStationScripts" + "\\"

dllsPath = RayStationScriptsPath + "Dlls"
#print(dllsPath)
sys.path.append(dllsPath)

scriptsPath = RayStationScriptsPath + "Scripts"
#print(scriptsPath)
sys.path.append(scriptsPath)

clr.AddReference("RoiManager")

import csv
import codecs

roiNameMappingTable = Dictionary[str, str]()

with codecs.open(RayStationScriptsPath + r"RoiNameMappingTable.csv","r", "utf-8-sig") as csvfile:
    csvreader = csv.DictReader(csvfile, delimiter = ",")
    for row in csvreader:
        roiNameMappingTable[row['Old']] = row['New']
        roiNameMappingTable[row['New']] = row['Old']

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

selectedExamination = case.Examinations[examinations.SelectedExamination]

roiGeometries = case.PatientModel.StructureSets[examinations.SelectedExamination].RoiGeometries

from RoiManager.Models import Roi
from RoiManager.Models import RoiType

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
    roi.CanUpdate = False
    roi. CanDeleteGeometry = False
    roi.CaseName = case.CaseName
    roi.ExaminationName = examinations.SelectedExamination
    roi.RoiType = Enum.Parse(clr.GetClrType(RoiType),ofRoi.Type)
    roi.Color = Media.Color.FromRgb(ofRoi.Color.R, ofRoi.Color.G, ofRoi.Color.B)
    
    rois.Add(roi)

from RoiManager.ViewModels import RoiSelectionViewModel
roiSelectionViewModel = RoiSelectionViewModel(rois, roiNameMappingTable)

from RoiManager.Views import MainWindow
mainWindow = MainWindow(roiSelectionViewModel)
mainWindow.ShowDialog();

print "RoiSelection CanExecute: ", roiSelectionViewModel.CanExecute

if not roiSelectionViewModel.CanExecute:
    print "Canceled"
    sys.exit()

roiNames = []
for r in rois:
    roiNames.append(r.Name)

for r in rois:
    roiGeometry = roiGeometries[r.Name]
    ofRoi = roiGeometry.OfRoi
    if r.IsDerived and r.CanUpdate:
        ofRoi.UpdateDerivedGeometry(Examination=selectedExamination, Algorithm="Auto")
    if r.IsDerived and r.CanUnderive:
        ofRoi.DeleteExpression()
    if r.HasGeometry and r.CanDeleteGeometry:
        roiGeometry.DeleteGeometry()
    if r.CanRename and r.NewName:
        if(not r.NewName in roiNames):
            ofRoi.Name = r.NewName
        else:
            message = 'Cannot Rename "{0}" because "{1}" already exists'.format(r.Name, r.NewName)
            print message
            MessageBox.Show(message)
    if r.CanDeleteRoi:
        ofRoi.DeleteRoi()
    if r.CanChangeColor:
        colorString = "{}, {}, {}".format(r.Color.R, r.Color.G, r.Color.B)
        ofRoi.Color = colorString
    if r.CanChangeRoiType:
        roiType = r.RoiType.ToString()
        ofRoi.Type = roiType
        if roiType == 'Gtv' or roiType == 'Ctv' or roiType == 'Ptv':
            ofRoi.OrganData.OrganType = 'Target' 
        elif roiType == 'Organ' or roiType == 'Avoidance':
            ofRoi.OrganData.OrganType = 'OrganAtRisk'
        elif roiType == 'Support':
            ofRoi.OrganData.OrganType = 'Other'
        else:
            ofRoi.OrganData.OrganType = 'Unknown'
pass
