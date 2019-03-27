from connect import *

# for .NET and WPF
import clr
import wpf
clr.AddReference("PresentationFramework")
clr.AddReference("PresentationCore")

from System.Windows import MessageBox

import json

def GetStructureSet(case, examination):
    examinationName = examination.Name
    return case.PatientModel.StructureSets[examinationName] 

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

def GetRoi(roiName, rois, color, roiType, TissueName=None, RbeCellTypeName=None, RoiMaterial=None):
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
        if(rois[roiName]):
            return True

    return False

def MakeRoisSubtractedRoi(case, examination, resultRoiName, sourceRoiNames, subtractedRoiNames, outerMargins = [0] * 6, innerMargins=[0] * 6, resultMargins=[0] * 6, isDerived=True, color="Yellow", roiType='Control'):
    
    structureSet = GetStructureSet(case, examination)

    if(resultRoiName in sourceRoiNames or resultRoiName in subtractedRoiNames):
        isDerived = False

    rois = GetRois(structureSet)
    roi = GetRoi(resultRoiName, rois, color, roiType)

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

def MakeRingRoi(case, examination, structureName, baseStructureName, outerMargin, innerMargin, isDerived=True, color="Yellow", roiType='Control'):
    
    with CompositeAction('Ring ROI Algebra ({0})'.format(structureName)):
    
        hasGeometry = MakeRoisSubtractedRoi(case, examination, structureName, [baseStructureName], [baseStructureName], [outerMargin] * 6, innerMargins=[innerMargin] * 6, resultMargins=[0] * 6, isDerived=isDerived, color=color, roiType=roiType)
    
      # CompositeAction ends 

    if(hasGeometry):
        return True
    else:
        message = 'MakeRingRoi for {0}: not created geometry'.format(structureName)
        MessageBox.Show(message)
        return False

def MakeTargetSubtractedRoi(case, examination, structureName, baseStructureName, subtractedTargetName, margin, isDerived=True, color="Yellow", roiType='Control'):
    
    with CompositeAction('Target subtracted ROI Algebra ({0})'.format(structureName)):

        hasGeometry = MakeRoisSubtractedRoi(case, examination, structureName, [baseStructureName], [subtractedTargetName], [0] * 6, innerMargins=[margin] * 6, resultMargins=[0] * 6, isDerived=isDerived, color=color, roiType=roiType)
    
      # CompositeAction ends 

    if(hasGeometry):
        return True
    else:
        message = 'MakeTargetSubtracted for {0}: not created geometry'.format(structureName)
        MessageBox.Show(message)
        return False

def MakeMarginAddedRoi(case, examination, structureName, baseStructureName, margin, isDerived=True, color="Yellow", roiType='Control'):
    
    resultRoiName = structureName
    sourceRoiName = baseStructureName
    if(resultRoiName == sourceRoiName):
        isDerived = False

    structureSet = GetStructureSet(case, examination)
    
    rois = GetRois(structureSet)
    roi = GetRoi(resultRoiName, rois, color, roiType)

    marginSettingsResult = MarginDict([margin]*6)

    hasSourceRoiContours = HaveAllRoisContours([sourceRoiName], rois)

    with CompositeAction('Margin Added ROI ({0})'.format(structureName)):
        if(isDerived):
            roi.SetMarginExpression(SourceRoiName=sourceRoiName, MarginSettings=marginSettingsResult)
            if(hasSourceRoiContours):
                roi.UpdateDerivedGeometry(Examination=examination, Algorithm='Auto')
                return True
            else:
                print 'MakeMarginAddedRoi for {0}: Not updated derived geometry because baseStructure does not have contours'.format(resultRoiName)
        else:
            if(hasSourceRoiContours):
                roi.CreateMarginGeometry(Examination=examination, SourceRoiName=sourceRoiName, MarginSettings=marginSettingsResult)
                return True
            else:
                print 'MakeMarginAddedRoi for {0}: Not created geometry because baseStructure does not have contours'.format(resultRoiName)

      # CompositeAction ends     

    message = 'MakeMarginAddedRoi for {0}: not created geometry'.format(structureName)
    MessageBox.Show(message)

    return False

if __name__ == '__main__':

    MessageBox.Show('Hello world')

    case = get_current("Case")
    examination = get_current("Examination")
    plan = get_current("Plan")

     
    case = get_current("Case")
    examination = get_current("Examination")

    #examinationName = examination.Name
    #structureSet = case.PatientModel.StructureSets[examinationName]

    structureSet = GetStructureSet(case, examination)

    rois = GetRois(structureSet)

    for key, value in rois.items():
        print key, value

    #jsonString = json.JSONEncoder().encode(rois)
    jsonString = json.dumps(rois)
    print jsonString

    resultRoiName = 'zPTV1-Rectum'
    sourceRoiNames = ['PTV1']
    subtractedRoiNames = ['Rectum']
    outerMargins = [0.1]*6

    #MakeRoisSubtractedRoi(case, examination, resultRoiName, sourceRoiNames, subtractedRoiNames, outerMargins, innerMargins=[0.2] * 6, resultMargins=[0] * 6, isDerived=True, color="Yellow", roiType='Control')
    
    structureName = 'zTestRing1_UD'
    baseStructureName = 'PTV1'
    MakeRingRoi(case, examination, structureName, baseStructureName, 1.5, 0.2, isDerived=False)

    structureName = 'zTestRectum-PTV1_01_UD'
    baseStructureName = 'Rectum'
    subtractedTargetName = 'PTV1'
    MakeTargetSubtractedRoi(case, examination, structureName, baseStructureName, subtractedTargetName, 0.1, isDerived=False)

    structureName = 'zTestBladder_03_UD'
    baseStructureName = 'Bladder'
    MakeMarginAddedRoi(case, examination, structureName, baseStructureName, 0.3, isDerived=False)


