from connect import *

import clr
import wpf
clr.AddReference("PresentationFramework")
clr.AddReference("PresentationCore")

from System.Collections.Generic import List, Dictionary
from System.Windows import MessageBox

import sys, os
import json

def GetRois(structureSet):
    roiGeometries = structureSet.RoiGeometries

    #DotNet Dictionary
    #rois = List[List[str]]()
    #for roiGeometry in roiGeometries:
    #    rois.Add(List[str]([roiGeometry.OfRoi.Name, str(roiGeometry.HasContours())]))

    rois = {}
    for roiGeometry in roiGeometries:
        #cast into python boolean for json
        rois[roiGeometry.OfRoi.Name] = bool(roiGeometry.HasContours())

    return rois

def GetRoi(roiName, rois, case, color, roiType, TissueName=None, RbeCellTypeName=None, RoiMaterial=None):
    if(roiName in rois):
        roi = case.PatientModel.RegionsOfInterest[roiName]
        if( not (roi.DerivedRoiExpression is None)):
            roi.DeleteExpression()
    else:
        roi = case.PatientModel.CreateRoi(Name=roiName, Color=color, Type=roiType, TissueName=TissueName, RbeCellTypeName=RbeCellTypeName, RoiMaterial=RoiMaterial)
    
    return roi

def MarginDict(margins, marginType='Expand'):
    keys = ['Type', 'Superior', 'Inferior', 'Anterior', 'Posterior', 'Right', 'Left']
    values = [marginType] + margins

    return dict(zip(keys, values))

def ExpressionDict(operation, sourceRois, margins=[0]*6, marginType='Expand'):
    marginDict = MarginDict(margins, marginType)
    return { 'Operation': operation, 'SourceRoiNames': sourceRois, 'MarginSettings': marginDict }

def HaveAllRoisContours(roiNames, rois):
    
    if(roiNames is None):
        return False

    if(len(roiNames) == 0):
        return False

    for roiName in roiNames:
        if(not (roiName in rois)):
            return False
        if(not rois[roiName]):
            return False

    return True

def MakeRoisSubtractedRoi(case, examination, resultRoiName, sourceRoiNames, subtractedRoiNames, outerMargins = [0] * 6, innerMargins=[0] * 6, resultMargins=[0] * 6, isDerived=True, color='Yellow', roiType='Control'):
    
    structureSet = GetStructureSet(case, examination)

    if(resultRoiName in sourceRoiNames or resultRoiName in subtractedRoiNames):
        isDerived = False

    rois = GetRois(structureSet)
    roi = GetRoi(resultRoiName, rois, case, color, roiType)

    #if(resultRoiName in rois):
    #    roi = case.PatientModel.RegionsOfInterest[resultRoiName]
    #    if( not (roi.DerivedRoiExpression is None)):
    #        roi.DeleteExpression()
    #else:
    #    roi = case.PatientModel.CreateRoi(Name=resultRoiName, Color=color, Type=roiType, TissueName=None, RbeCellTypeName=None, RoiMaterial=None)

    #marginSettingsA = MarginDict(outerMargins)
    #marginSettingsB = MarginDict(innerMargins)
    marginSettingsResult = MarginDict(resultMargins)

    expressionA = ExpressionDict('Union', sourceRoiNames, outerMargins)
    expressionB = ExpressionDict('Union', subtractedRoiNames, innerMargins)
    resultOperation = 'Subtraction'

    roiNames = sourceRoiNames + subtractedRoiNames
    haveAllRoisContours = HaveAllRoisContours(roiNames, rois)

    if(isDerived):
        roi.SetAlgebraExpression(ExpressionA=expressionA, ExpressionB=expressionB, ResultOperation=resultOperation, ResultMarginSettings=marginSettingsResult)
        if(haveAllRoisContours):
            roi.UpdateDerivedGeometry(Examination=examination, Algorithm='Auto')
            return True
        else:
            print 'MakeRoisSubtractedRoi for {0}: Not updated derived geometry because all ROIs do not have contours'.format(resultRoiName)
    else:
        if(haveAllRoisContours):
            roi.CreateAlgebraGeometry(Examination=examination, Algorithm='Auto', ExpressionA=expressionA, ExpressionB=expressionB, ResultOperation=resultOperation, ResultMarginSettings=marginSettingsResult)
            return True
        else:
            print 'MakeRoisSubtractedRoi for {0}: Not created geometry because all ROIs do not have contours'.format(resultRoiName)

    return False

def MakeUnionRoi(case, examination, resultRoiName, sourceRoiNames, margins=[0] * 6, marginType='Expand', isDerived=True, color='Yellow', roiType='Control'):
    
    structureSet = GetStructureSet(case, examination)

    if(resultRoiName in sourceRoiNames):
        isDerived = False

    rois = GetRois(structureSet)
    roi = GetRoi(resultRoiName, rois, case, color, roiType)

    expressionA = ExpressionDict('Union', sourceRoiNames, margins)
    expressionB = ExpressionDict('Union', [])
    resultOperation = 'None'
    marginSettingsResult = MarginDict([0]*6)

    roiNames = sourceRoiNames
    haveAllRoisContours = HaveAllRoisContours(roiNames, rois)

    if(isDerived):
        roi.SetAlgebraExpression(ExpressionA=expressionA, ExpressionB=expressionB, ResultOperation=resultOperation, ResultMarginSettings=marginSettingsResult)
        if(haveAllRoisContours):
            roi.UpdateDerivedGeometry(Examination=examination, Algorithm='Auto')
            return True
        else:
            print 'MakeUnionRoi for {0}: Not updated derived geometry because all ROIs do not have contours'.format(resultRoiName)
    else:
        if(haveAllRoisContours):
            roi.CreateAlgebraGeometry(Examination=examination, Algorithm='Auto', ExpressionA=expressionA, ExpressionB=expressionB, ResultOperation=resultOperation, ResultMarginSettings=marginSettingsResult)
            return True
        else:
            print 'MakeUnionRoi for {0}: Not created geometry because all ROIs do not have contours'.format(resultRoiName)

    return False

def MakeIntersectionRoi(case, examination, resultRoiName, sourceRoiNames, margins=[0] * 6, marginType='Expand', isDerived=True, color='Yellow', roiType='Control'):
    
    structureSet = GetStructureSet(case, examination)

    if(resultRoiName in sourceRoiNames):
        isDerived = False

    rois = GetRois(structureSet)
    roi = GetRoi(resultRoiName, rois, case, color, roiType)

    expressionA = ExpressionDict('Intersection', sourceRoiNames, margins)
    expressionB = ExpressionDict('Intersection', [])
    resultOperation = 'None'
    marginSettingsResult = MarginDict([0]*6)

    roiNames = sourceRoiNames
    haveAllRoisContours = HaveAllRoisContours(roiNames, rois)

    if(isDerived):
        roi.SetAlgebraExpression(ExpressionA=expressionA, ExpressionB=expressionB, ResultOperation=resultOperation, ResultMarginSettings=marginSettingsResult)
        if(haveAllRoisContours):
            roi.UpdateDerivedGeometry(Examination=examination, Algorithm='Auto')
            return True
        else:
            print 'MakeIntersectionRoi for {0}: Not updated derived geometry because all ROIs do not have contours'.format(resultRoiName)
    else:
        if(haveAllRoisContours):
            roi.CreateAlgebraGeometry(Examination=examination, Algorithm='Auto', ExpressionA=expressionA, ExpressionB=expressionB, ResultOperation=resultOperation, ResultMarginSettings=marginSettingsResult)
            return True
        else:
            print 'MakeIntersectionRoi for {0}: Not created geometry because all ROIs do not have contours'.format(resultRoiName)

    return False

def MakeMonoAlgebraRoi(case, examination, resultRoiName, sourceRoiNames, operation='Union', margins=[0] * 6, marginType='Expand', isDerived=True, color='Yellow', roiType='Control'):
    
    structureSet = GetStructureSet(case, examination)

    if(resultRoiName in sourceRoiNames):
        isDerived = False

    rois = GetRois(structureSet)
    roi = GetRoi(resultRoiName, rois, case, color, roiType)

    expressionA = ExpressionDict(operation, sourceRoiNames, margins, marginType)
    expressionB = ExpressionDict('Union', [])
    resultOperation = 'None'
    marginSettingsResult = MarginDict([0]*6)

    roiNames = sourceRoiNames
    haveAllRoisContours = HaveAllRoisContours(roiNames, rois)

    if(isDerived):
        roi.SetAlgebraExpression(ExpressionA=expressionA, ExpressionB=expressionB, ResultOperation=resultOperation, ResultMarginSettings=marginSettingsResult)
        if(haveAllRoisContours):
            roi.UpdateDerivedGeometry(Examination=examination, Algorithm='Auto')
            return True
        else:
            print 'MakeMonoAlgebraRoi for {0}: Not updated derived geometry because all ROIs do not have contours'.format(resultRoiName)
    else:
        if(haveAllRoisContours):
            roi.CreateAlgebraGeometry(Examination=examination, Algorithm='Auto', ExpressionA=expressionA, ExpressionB=expressionB, ResultOperation=resultOperation, ResultMarginSettings=marginSettingsResult)
            return True
        else:
            print 'MakeMonoAlgebraRoi for {0}: Not created geometry because all ROIs do not have contours'.format(resultRoiName)

    return False

def MakeBiAlgebraRoi(case, examination, resultRoiName, operation='Union', margins=[0]*6, marginType='Expand', sourceRoiNamesA=[], operationA='Union', marginsA=[0] * 6, marginTypeA='Expand', sourceRoiNamesB=[],  operationB='Union', marginsB=[0] * 6, marginTypeB='Expand', isDerived=True, color='Yellow', roiType='Control'):
    
    structureSet = GetStructureSet(case, examination)

    sourceRoiNames = sourceRoiNamesA + sourceRoiNamesB
    if(resultRoiName in sourceRoiNames):
        isDerived = False

    rois = GetRois(structureSet)
    roi = GetRoi(resultRoiName, rois, case, color, roiType)

    expressionA = ExpressionDict(operationA, sourceRoiNamesA, marginsA, marginTypeA)
    expressionB = ExpressionDict(operationB, sourceRoiNamesB, marginsB, marginTypeB)
    resultOperation = operation
    marginSettingsResult = MarginDict(margins, marginType)

    roiNames = sourceRoiNames
    haveAllRoisContours = HaveAllRoisContours(roiNames, rois)

    if(isDerived):
        roi.SetAlgebraExpression(ExpressionA=expressionA, ExpressionB=expressionB, ResultOperation=resultOperation, ResultMarginSettings=marginSettingsResult)
        if(haveAllRoisContours):
            roi.UpdateDerivedGeometry(Examination=examination, Algorithm='Auto')
            return True
        else:
            print 'MakeBilgebraRoi for {0}: Not updated derived geometry because all ROIs do not have contours'.format(resultRoiName)
    else:
        if(haveAllRoisContours):
            roi.CreateAlgebraGeometry(Examination=examination, Algorithm='Auto', ExpressionA=expressionA, ExpressionB=expressionB, ResultOperation=resultOperation, ResultMarginSettings=marginSettingsResult)
            return True
        else:
            print 'MakeBiAlgebraRoi for {0}: Not created geometry because all ROIs do not have contours'.format(resultRoiName)

    return False

def MakeMarginRoi(case, examination, resultRoiName, sourceRoiName, margin, marginType='Expand', isDerived=True, color='Yellow', roiType='Control'):
    
    structureSet = GetStructureSet(case, examination)

    if(resultRoiName == sourceRoiName):
        isDerived = False

    rois = GetRois(structureSet)
    roi = GetRoi(resultRoiName, rois, case, color, roiType)

    marginSettings = MarginDict([0]*6, marginType)

    roiNames = [sourceRoiName]
    haveAllRoisContours = HaveAllRoisContours(roiNames, rois)

    if(isDerived):
        roi.SetMarginExpression(SourceRoiName=sourceRoiName, MarginSettings=marginSettings)
        if(haveAllRoisContours):
            roi.UpdateDerivedGeometry(Examination=examination, Algorithm='Auto')
            return True
        else:
            print 'MakeMarginRoi for {0}: Not updated derived geometry because the source ROI does not have contours'.format(resultRoiName)
    else:
        if(haveAllRoisContours):
            roi.CreateMarginGeometry(SourceRoiName=sourceRoiName, MarginSettings=marginSettings)
            return True
        else:
            print 'MakeMarginRoi for {0}: Not created geometry because the source ROI does not have contours'.format(resultRoiName)

    return False

def MakeWallRoi(case, examination, resultRoiName, sourceRoiName, outwardDistance, inwardDistance=0.0, isDerived=True, color='Yellow', roiType='Control'):
    
    structureSet = GetStructureSet(case, examination)

    if(resultRoiName == sourceRoiName):
        isDerived = False

    rois = GetRois(structureSet)
    roi = GetRoi(resultRoiName, rois, case, color, roiType)

    roiNames = [sourceRoiName]
    haveAllRoisContours = HaveAllRoisContours(roiNames, rois)

    if(isDerived):
        roi.SetWallExpression(SourceRoiName=sourceRoiName, OutwardDistance=outwardDistance, InwardDistance=inwardDistance)
        if(haveAllRoisContours):
            roi.UpdateDerivedGeometry(Examination=examination, Algorithm='Auto')
            return True
        else:
            print 'MakeWallRoi for {0}: Not updated derived geometry because the source ROI does not have contours'.format(resultRoiName)
    else:
        if(haveAllRoisContours):
            roi.CreateWallGeometry(SourceRoiName=sourceRoiName, OutwardDistance=outwardDistance, InwardDistance=inwardDistance)
            return True
        else:
            print 'MakeWallRoi for {0}: Not created geometry because the source ROI does not have contours'.format(resultRoiName)

    return False

def MakeRingRoi(case, examination, structureName, baseStructureName, outerMargin, innerMargin, isDerived=True, color='Yellow', roiType='Control'):
    
    with CompositeAction('Ring ROI Algebra ({0})'.format(structureName)):
    
        hasGeometry = MakeRoisSubtractedRoi(case, examination, structureName, [baseStructureName], [baseStructureName], [outerMargin] * 6, innerMargins=[innerMargin] * 6, resultMargins=[0] * 6, isDerived=isDerived, color=color, roiType=roiType)
    
      # CompositeAction ends 

    if(hasGeometry):
        return True
    else:
        message = 'MakeRingRoi for {0}: not created geometry'.format(structureName)
        MessageBox.Show(message)
        return False

def MakeRoiSubtractedRoi(case, examination, structureName, baseStructureName, subtractedRoiName, margin, isDerived=True, color='Yellow', roiType='Control'):
    
    with CompositeAction('ROI subtracted ROI Algebra ({0})'.format(structureName)):

        hasGeometry = MakeRoisSubtractedRoi(case, examination, structureName, [baseStructureName], [subtractedRoiName], [0] * 6, innerMargins=[margin] * 6, resultMargins=[0] * 6, isDerived=isDerived, color=color, roiType=roiType)
    
      # CompositeAction ends 

    if(hasGeometry):
        return True
    else:
        message = 'MakeRoiSubtracted for {0}: not created geometry'.format(structureName)
        MessageBox.Show(message)
        return False

def MakeMarginAddedRoi(case, examination, structureName, baseStructureNames, margin, isDerived=True, color='Yellow', roiType='Control'):
    
    resultRoiName = structureName
    sourceRoiNames = baseStructureNames
    if(resultRoiName in sourceRoiNames):
        isDerived = False

    structureSet = GetStructureSet(case, examination)
    
    rois = GetRois(structureSet)
    roi = GetRoi(resultRoiName, rois, case, color, roiType)

    marginSettingsResult = MarginDict([margin]*6)

    hasSourceRoiContours = HaveAllRoisContours(sourceRoiNames, rois)

    with CompositeAction('Margin Added ROI ({0})'.format(structureName)):

        if (len(sourceRoiNames) == 1):
            if(isDerived):
                roi.SetMarginExpression(SourceRoiName=sourceRoiNames[0], MarginSettings=marginSettingsResult)
                if(hasSourceRoiContours):
                    roi.UpdateDerivedGeometry(Examination=examination, Algorithm='Auto')
                    return True
                else:
                    print 'MakeMarginAddedRoi for {0}: Not updated derived geometry because baseStructure does not have contours'.format(resultRoiName)
            else:
                if(hasSourceRoiContours):
                    roi.CreateMarginGeometry(Examination=examination, SourceRoiName=sourceRoiName[0], MarginSettings=marginSettingsResult)
                    return True
                else:
                    print 'MakeMarginAddedRoi for {0}: Not created geometry because baseStructure does not have contours'.format(resultRoiName)
        elif (len(sourceRoiNames) > 1):
            hasGeometry = MakeUnionRoi(case, examination, resultRoiName, sourceRoiNames, [margin]*6)
            if(hasGeometry):
                return True
        else:
            print 'MakeMarginAddedRoi for {0}: Do nothing because baseStructureNames is empty'

      # CompositeAction ends     

    message = 'MakeMarginAddedRoi for {0}: not created geometry'.format(structureName)
    MessageBox.Show(message)

    return False

def MakeOverlappedRois(case, examination, structureName, baseStructureNames, margin, isDerived=True, color='Yellow', roiType='Control'):
    
    resultRoiName = structureName
    sourceRoiNames = baseStructureNames
    if(resultRoiName in sourceRoiNames):
        isDerived = False

    structureSet = GetStructureSet(case, examination)
    
    rois = GetRois(structureSet)
    roi = GetRoi(resultRoiName, rois, case, color, roiType)

    marginSettingsResult = MarginDict([margin]*6)

    hasSourceRoiContours = HaveAllRoisContours(sourceRoiNames, rois)

    with CompositeAction('Overlapped ROIs ({0})'.format(structureName)):

        if (len(sourceRoiNames) == 1):
            if(isDerived):
                roi.SetMarginExpression(SourceRoiName=sourceRoiNames[0], MarginSettings=marginSettingsResult)
                if(hasSourceRoiContours):
                    roi.UpdateDerivedGeometry(Examination=examination, Algorithm='Auto')
                    return True
                else:
                    print 'MakeOverlappedRois for {0}: Not updated derived geometry because baseStructure does not have contours'.format(resultRoiName)
            else:
                if(hasSourceRoiContours):
                    roi.CreateMarginGeometry(Examination=examination, SourceRoiName=sourceRoiName[0], MarginSettings=marginSettingsResult)
                    return True
                else:
                    print 'MakeOverlappedRois for {0}: Not created geometry because baseStructure does not have contours'.format(resultRoiName)
        elif (len(sourceRoiNames) > 1):
            hasGeometry = MakeIntersectionRoi(case, examination, resultRoiName, sourceRoiNames, [margin]*6)
            if(hasGeometry):
                return True
        else:
            print 'MakeOverlappedRois for {0}: Do nothing because baseStructureNames is empty'

      # CompositeAction ends     

    message = 'MakeOverlappedRois for {0}: not created geometry'.format(structureName)
    MessageBox.Show(message)

    return False

RayStationScriptsPath = os.environ["USERPROFILE"] + r"\DeskTop\RayStationScripts" + "\\"
#RayStationScriptsPath = r"\\10.208.223.10\RayStation\RayStationScripts" + "\\" 
#RayStationScriptsPath = r"R:\RayStationScripts" + "\\"

dllsPath = RayStationScriptsPath + "Dlls"
print(dllsPath)
sys.path.append(dllsPath)

#scriptsPath = RayStationScriptsPath + "Scripts"
scriptsPath = r"\\10.208.223.10\RayStation\RayStationScripts\Scripts"
print(scriptsPath)
sys.path.append(scriptsPath)

clr.AddReference("RoiFormulaMaker")
from RoiFormulaMaker.Views import MainWindow
from RoiFormulaMaker.Models import RoiFormulas

roiFormulasPath = RayStationScriptsPath + "RoiFormulas"
roiFormulasPath = r"\\10.208.223.10\RayStation\RayStationScripts\RoiFormulas"
#roiFormulasPath = r"R:\RayStationScripts\RoiFormulas"

from Helpers import GetStructureSet, GetRoiDetails
#from Helpers import MakeMarginAddedRoi, MakeOverlappedRois, MakeRingRoi, MakeRoisSubtractedRoi, MakeWallRoi

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
    structureName = rf['StructureName']

    if(formulaType  == 'MarginAddedRoi'):
        baseStructureNames = rf['BaseStructureNames']
        margin = rf['Margin']
        roiType = rf['StructureType']

        marginInCm = margin/10.

        print formulaType , structureName, baseStructureNames, marginInCm, roiType
        MakeMarginAddedRoi(case, examination, structureName, baseStructureNames, marginInCm, isDerived=True, color='Yellow', roiType=roiType)
    
    elif(formulaType  == 'OverlappedRoi'):
        baseStructureNames = rf['BaseStructureNames']
        margin = rf['Margin']
        roiType = rf['StructureType']

        marginInCm = margin/10.

        print formulaType , structureName, baseStructureNames, marginInCm, roiType
        MakeOverlappedRois(case, examination, structureName, baseStructureNames, marginInCm, isDerived=True, color='Yellow', roiType=roiType)

    elif(formulaType  == 'RingRoi'):
        baseStructureName = rf['BaseStructureName']
        innerMargin = rf['InnerMargin']
        outerMargin = rf['OuterMargin']
        roiType = rf['StructureType']

        innerMarginInCm =  innerMargin/10.
        outerMarginInCm = outerMargin/10.

        print formulaType , structureName, baseStructureName, outerMarginInCm, innerMarginInCm, roiType
        MakeRingRoi(case, examination, structureName, baseStructureName, outerMarginInCm, innerMarginInCm, isDerived=True, color='Yellow', roiType=roiType)

    elif(formulaType  == 'WallRoi'):
        baseStructureName = rf['BaseStructureName']
        innerMargin = rf['InnerMargin']
        outerMargin = rf['OuterMargin']
        roiType = rf['StructureType']

        innerMarginInCm =  innerMargin/10.
        outerMarginInCm = outerMargin/10.

        print formulaType , structureName, baseStructureName, outerMarginInCm, innerMarginInCm, roiType
        MakeWallRoi(case, examination, structureName, baseStructureName, outerMarginInCm, innerMarginInCm, isDerived=True, color='Yellow', roiType=roiType)

    elif(formulaType  == 'RoiSubtractedRoi'):
        baseStructureName = rf['BaseStructureName']
        subtractedRoiNames = rf['SubtractedRoiNames']
        margin = rf['Margin']
        roiType = rf['StructureType']

        marginInCm = margin/10.  

        print formulaType , structureName, baseStructureName, subtractedRoiNames, marginInCm, roiType
        MakeRoisSubtractedRoi(case, examination, structureName, [baseStructureName], subtractedRoiNames, outerMargins = [0] * 6, innerMargins=[marginInCm] * 6, resultMargins=[0] * 6, isDerived=True, color='Yellow', roiType=roiType)
pass


