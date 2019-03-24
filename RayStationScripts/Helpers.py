from System.Windows import MessageBox

def MarginDict(margins, type='Expand'):
    keys = ['Type', 'Superior', 'Inferior', 'Anterior', 'Posterior', 'Right', 'Left']
    values = [type] + margins

    return dict(zip(keys, values))

def ExpressionDict(operation, sourceRois, margins, marginType='Expand'):
    marginDict = MarginDict(margins, marginType)
    return { 'Operation': operation, 'SourceRoiNames': sourceRois, 'MarginSettings': marginDict }

def GetRois(structureSet):
    return {'PTV': True, 'Rectum': True, 'Bladder':False}

def HaveAllRoisContours(rois, roiNames):
    
    if(roiNames == null):
        return False

    if(len(roiNames) == 0):
        return False

    for roiName in roiNames:
        if(not (roiName in rois)):
            return False
        if(rois[roiName]):
            return False

    return True

def MakeRoisSubtractedRoi(case, examination, structureSets, resultRoiName, sourceRoiNames, subtractedRoiNames, outerMargins, innerMargins=[0] * 6, resultMargins=[0] * 6, isDervied=True, color="Yellow", roiType='Control'):
    
    if(len(set(sourceRoiNames) & set(subtractedRoiNames)) > 0):
       print 'MakeRoisSubtractedRoi for {0}: sourceRoiNames and subtractedRoiNames include the same ROI name'.format(resultRoiName)
       return

    if(resultRoiName in sourceRoiName or resultRoiName in subtractedRoiName):
        isDerived = False

    rois = GetRois(structureSet)

    if(resultRoiName in rois):
        roi = case.PatientModel.RegionsOfInterest[resultRoiName]
        if(roi.AlgebraExpression != null):
            roi.DeleteExpression()
    else:
        roi = case.PatientModel.CreateRoi(Name=resultRoiName, Color=color, Type=roiType, TissueName=None, RbeCellTypeName=None, RoiMaterial=None)

    marginSettingsA = MarginDict(outerMargins)
    marginSettingsB = MarginDict(innerMargins)
    marginSettingsResult = MarginDict(resultMargins)

    expressionA = ExpressionDict('Union', sourceRoiNames, marginSettingsA)
    expressionB = ExpressionDict('Union', subtractedRoiNames, marginSettingsB)
    resultOperation = 'Subtraction'

    hasAllRoisContours = HasAllRoisContours(rois, roiNames)

    if(isDerived):
        roi.SetAlgebraExpression(ExpressionA=expressionA, ExpressionB=expressionB, ResultOperation=resultOperation, ResultMarginSettings=marginSettingsResult)
        if(hasAllRoisContours):
            roi.UpdateDerivedGeometry(Examination=examination, Algorithm='Auto')
            return True
        else:
            print 'MakeRoisSubtractedRoi for {0}: Not updated derived geometry because all ROIs do not have contours'.format(resultRoiName)
    else:
        if(hasAllRoisContours):
            roi.CreateAlgebraGeometry(Examination=examination, Algorithm='Auto', ExpressionA=expressionA, ExpressionB=expressionB, ResultOperation=resultOperation, ResultMarginSettings=marginSettingsResult)
            return True
        else:
            print 'MakeRoisSubtractedRoi for {0}: Not created geometry because all ROIs do not have contours'.format(resultRoiName)

    return False

def MakeRingRoi(case, examination, structureSets, structureName, baseStructureName, outerMargin, innerMargin, isDervied=True, color="Yellow", roiType='Control'):
    
    hasGeometry = MakeRoisSubtractedRoi(case, examination, structureSets, structureName, [baseStructureName], [baseStructureName], [outerMargin] * 6, innerMargins=[innerMargin] * 6, resultMargins=[0] * 6, isDervied=isDerived, color=color, roiType=roiType)
    
    if(not hasGeometry):
        message = "MakeRingRoi for {0}: not created geometry"
        MessageBox.Show(message)

def MakeTargetSubtractedRoi(case, examination, structureSets, structureName, baseStructureName, subtractedTargetName, margin, isDervied=True, color="Yellow", roiType='Control'):
    
    hasGeometry = MakeRoisSubtractedRoi(case, examination, structureSets, structureName, [baseStructureName], [subtractedTargetName], [0] * 6, innerMargins=[margin] * 6, resultMargins=[0] * 6, isDervied=isDerived, color=color, roiType=roiType)
    
    if(not hasGeometry):
        message = "MakeTargetSubtracted for {0}: not created geometry"
        MessageBox.Show(message)