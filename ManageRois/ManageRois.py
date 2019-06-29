#from connect import *

import clr

clr.AddReference("System.Drawing")
#clr.AddReference("PresentationFramework")
#clr.AddReference("PresentationCore")

import sys, math, wpf, os
from System.Collections.Generic import List, Dictionary
from System.Windows import MessageBox
from System.Windows import Media
from System import Drawing

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

from RoiManager.Models import Roi
rois = List[Roi]()

roi = Roi()
roi.Name = 'PTV'
roi.IsDerived = False
dependentRois = List[str]()
roi.DependentRois = dependentRois
roi.HasGeometry = True
roi.CanDeleteGeometry = False
roi.CanUnderive = False
roi.CanDeleteGeometry = False
roi.CanRename = False
roi.NewName = ''
roi.CaseName = 'C1'
roi.ExaminationName = 'CT1'

colorDrawing = Drawing.Color.FromArgb(255,0,0)
colorMedia = Media.Color.FromRgb(colorDrawing.R, colorDrawing.G, colorDrawing.B)
roi.Color = colorMedia

rois.Add(roi)

roi = Roi()
roi.Name = 'zPTV-CTV'
roi.IsDerived = True
dependentRois = List[str]()
dependentRois.Add('PTV')
dependentRois.Add('CTV')
roi.DependentRois = dependentRois
roi.HasGeometry = True
roi.CanDeleteGeometry = False
roi.CanUnderive = False
roi.CanDeleteGeometry = False
roi.CanRename = False
roi.NewName = ''
roi.CaseName = 'C1'
roi.ExaminationName = 'CT1'

colorDrawing = Drawing.Color.FromArgb(0,255,0)
colorMedia = Media.Color.FromRgb(colorDrawing.R, colorDrawing.G, colorDrawing.B)
roi.Color = colorMedia

rois.Add(roi)

from RoiManager.ViewModels import RoiSelectionViewModel
roiSelectionViewModel = RoiSelectionViewModel(rois, roiNameMappingTable)

from RoiManager.Views import MainWindow
mainWindow = MainWindow(roiSelectionViewModel)
mainWindow.ShowDialog();

for r in rois:
    print r
    colorString = "{} {} {}".format(r.Color.R, r.Color.G, r.Color.B)
    print "Color: {}".format(colorString)

pass
#from RoiManager.ViewModels import ExaminationSelectionViewModel

#case = get_current("Case")

#examinations = ExaminationSelectionViewModel()

#examinationNames = List[str]()

#for e in case.Examinations:
#    examinationNames.Add(e.Name)
#    print(e.Name)

#examinations.CanExecute = False
#examinations.ExaminationNames = examinationNames
#from RoiManager.Views import ExaminationSelectionView
#examinationSelectionWindow = ExaminationSelectionView(examinations)
#examinationSelectionWindow.ShowDialog();

#print "Selected Examination: ", examinations.SelectedExamination
#print "ExaminationSelection CanExecute: ", examinations.CanExecute
#if not examinations.CanExecute:
#    print "Canceled"
#    sys.exit()

#roiGeometries = case.PatientModel.StructureSets[examinations.SelectedExamination].RoiGeometries

#from RoiManager.Models import Roi
#rois = List[Roi]()

#for r in roiGeometries:
#    roi = Roi()
#    ofRoi = r.OfRoi
#    roi.Name = ofRoi.Name
#    if ofRoi.DerivedRoiExpression is not None:
#        roi.IsDerived = True
#        roi.DependentRois = r.GetDependentRois()
#    else:
#        roi.IsDerived = False
#        roi.DependentRois = List[str]()

#    roi.HasGeometry = r.HasContours()
#    roi.CanDeleteGeometry = False
#    roi.CanUnderive = False
#    roi.CanDeleteGeometry = False
#    roi.CanRename = False
#    roi.NewName = ''
#    roi.CaseName = case.CaseName
#    roi.ExaminationName = examinations.SelectedExamination
#    rois.Add(roi)

#from RoiManager.ViewModels import RoiSelectionViewModel
#roiSelectionViewModel = RoiSelectionViewModel(rois, roiNameMappingTable)

#from RoiManager.Views import MainWindow
#mainWindow = MainWindow(roiSelectionViewModel)
#mainWindow.ShowDialog();

#print "RoiSelection CanExecute: ", roiSelectionViewModel.CanExecute

#if not roiSelectionViewModel.CanExecute:
#    print "Canceled"
#    sys.exit()

#for r in rois:
#    roigeometry = roigeometries[r.name]
#    ofroi = roigeometry.ofroi
#    if r.isderived and r.canunderive:
#        ofroi.deleteexpression()
#    if r.hasgeometry and r.candeletegeometry:
#        roigeometry.deletegeometry()
#    if r.CanRename and r.NewName:
#        pass
#    if r.candeleteroi:
#        ofroi.deleteroi()
#pass
