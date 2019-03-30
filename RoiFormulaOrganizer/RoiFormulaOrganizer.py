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

roiFormulasPath = RayStationScriptsPath + "RoiFormulas"

from Helpers import GetStructureSet, GetRoiDetails
from Helpers import MakeMarginAddedRoi, MakeRingRoi, MakeRoiSubtractedRoi

case = get_current("Case")
examination = get_current("Examination")
structureSet = GetStructureSet(case, examination)
roiDetails = GetRoiDetails(structureSet)

#structureNames = List[str](rois.keys())
#structureNames = List[str](['PTV', 'Rectum', 'Bladder', 'FemoralHeads'])

#structureDetails_dict = {"PTV": {"HasContours":True, "Type":"Ptv"}, "Rectum":{"HasContours":True, "Type":"Organ"},
#                        "Bladder":{"HasContours":True, "Type":"Organ"}, "zRing":{"HasContours":False, "Type":"Control"}}

structureDetails = Dictionary[str, Dictionary[str,object]]()

for key, value in roiDetails.items():
    structureDetails.Add(key, Dictionary[str,object](value))

structureFormulas = RoiFormulas()

mainWindow = MainWindow(structureDetails, structureFormulas, roiFormulasPath)
mainWindow.ShowDialog();

if structureFormulas.CanExecute == False:
    print "Canceled"
    sys.exit()

#Python JSON encoder and decoder
#https://docs.python.org/2.7/library/json.html
roiFormulas = []
for s in structureFormulas.Formulas:
    #print(s,s.ToJson())
    #To dictionary
    roiFormulas.append(json.loads(s.ToJson()))

#fileName = 'test.json'
#filePath = os.path.join(roiFormulasPath, fileName)

#with open(filePath, mode='w') as f:
#    f.write(json.dumps({'Description':'test ROI formulas', 'RoiFormulas' : roiFormulas}))

for rf in roiFormulas:
    formulaType = rf['FormulaType']
    if(formulaType  == 'MarginAddedRoi'):
        structureName = rf['StructureName']
        baseStructureName = rf['BaseStructureName']
        margin = rf['Margin']
        roiType = rf['StructureType']

        marginInCm = margin/10.

        print formulaType , structureName, baseStructureName, marginInCm, roiType
        MakeMarginAddedRoi(case, examination, structureName, baseStructureName, marginInCm, isDerived=True, color='Yellow', roiType=roiType)
    
    elif(formulaType  == 'RingRoi'):
        structureName = rf['StructureName']
        baseStructureName = rf['BaseStructureName']
        innerMargin = rf['InnerMargin']
        outerMargin = rf['OuterMargin']
        roiType = rf['StructureType']

        innerMarginInCm =  innerMargin/10.
        outerMarginInCm = outerMargin/10.

        print formulaType , structureName, baseStructureName, outerMarginInCm, innerMarginInCm, roiType
        MakeRingRoi(case, examination, structureName, baseStructureName, outerMarginInCm, innerMarginInCm, isDerived=True, color='Yellow', roiType=roiType)

    elif(formulaType  == 'RoiSubtractedRoi'):
        structureName = rf['StructureName']
        baseStructureName = rf['BaseStructureName']
        subtractedRoiName = rf['SubtractedRoiName']
        margin = rf['Margin']
        roiType = rf['StructureType']

        marginInCm = margin/10.  

        print formulaType , structureName, baseStructureName, subtractedRoiName, marginInCm, roiType
        MakeRoiSubtractedRoi(case, examination, structureName, baseStructureName, subtractedRoiName, marginInCm, isDerived=True, color='Yellow', roiType=roiType)
pass

