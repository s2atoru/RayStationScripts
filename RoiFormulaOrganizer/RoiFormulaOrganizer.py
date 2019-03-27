#from connect import *

import clr
import wpf
clr.AddReference("PresentationFramework")
clr.AddReference("PresentationCore")

from System.Collections.Generic import List
from System.Windows import MessageBox

import sys, os
import json

clr.AddReference("RoiFormulaMaker")
from RoiFormulaMaker.Views import MainWindow

RayStationScriptsPath = os.environ["USERPROFILE"] + r"\DeskTop\RayStationScripts" + "\\"

dllsPath = RayStationScriptsPath + "Dlls"
print(dllsPath)
sys.path.append(dllsPath)

scriptsPath = RayStationScriptsPath + "Scripts"
print(scriptsPath)
sys.path.append(scriptsPath)

roiFormulasPath = RayStationScriptsPath + "RoiFormulas"

#case = get_current("Case")
#examination = get_current("Examination")
#structureSet = GetStructureSet(case, examination)
#rois = GetRois(structureSet)
#structureNames = List[str](rois.keys())

structureNames = List[str](['PTV', 'Rectum', 'Bladder', 'FemoralHeads'])
structureDesigns = List[object]()

mainWindow = MainWindow(structureNames, structureDesigns, roiFormulasPath)
mainWindow.ShowDialog();

#Python JSON encoder and decoder
#https://docs.python.org/2.7/library/json.html
roiFormulas = []
for s in structureDesigns:
    #print(s,s.ToJson())
    #To dictionary
    roiFormulas.append(json.loads(s.ToJson()))

fileName = 'test.json'
filePath = os.path.join(roiFormulasPath, fileName)

with open(filePath, mode='w') as f:
    f.write(json.dumps({'Description':'test ROI formulas', 'RoiFormulas' : roiFormulas}))

for rf in roiFormulas:
    formulaType = rf['FormulaType']
    if(formulaType  == 'MarginAddedRoi'):
        structureName = rf['StructureName']
        baseStructureName = rf['BaseStructureName']
        margin = rf['Margin']

        marginInCm = margin/10.

        print formulaType , structureName, baseStructureName, marginInCm
        #MakeMarginAddedRoi(case, examination, structureName, baseStructureName, marginInCm, isDerived=True, color='Yellow', roiType='Control')
    
    elif(formulaType  == 'RingRoi'):
        structureName = rf['StructureName']
        baseStructureName = rf['BaseStructureName']
        innerMargin = rf['InnerMargin']
        outerMargin = rf['OuterMargin']

        innerMarginInCm =  innerMargin/10.
        outerMarginInCm = outerMargin/10.

        print formulaType , structureName, baseStructureName, outerMarginInCm, innerMarginInCm
        #MakeRingRoi(case, examination, structureName, baseStructureName, outerMarginInCm, innerMarginInCm, isDerived=True, color='Yellow', roiType='Control')

    elif(formulaType  == 'RoiSubtractedRoi'):
        structureName = rf['StructureName']
        baseStructureName = rf['BaseStructureName']
        subtractedRoiName = rf['SubtractedRoiName']
        margin = rf['Margin']

        marginInCm = margin/10.  

        print formulaType , structureName, baseStructureName, subtractedRoiName, marginInCm
        #MakeRoiSubtractedRoi(case, examination, structureName, baseStructureName, subtractedRoiName, marginInCm, isDerived=True, color='Yellow', roiType='Control')
pass