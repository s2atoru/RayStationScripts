from connect import *

# for .NET and WPF
import clr
import wpf
clr.AddReference("PresentationFramework")
clr.AddReference("PresentationCore")

from System.Windows import MessageBox

import json
import math

def SizeOfIterator(iterator):
    return sum(1 for _ in iterator)

def GetStructureSet(case, examination):
    examinationName = examination.Name
    return case.PatientModel.StructureSets[examinationName] 

def GetOptimizationFunctionType(objectiveConstituentFunction):
    doseFunctionParameters = objectiveConstituentFunction.DoseFunctionParameters
    functionType = doseFunctionParameters.FunctionType if hasattr(doseFunctionParameters, 'FunctionType') else 'DoseFallOff'
    if functionType == 'UniformEud':
        functionType = 'TargetEud'

    return functionType

def GetPlanOptimizationForBeamSet(planOptimizations, beamSet):
    if SizeOfIterator(planOptimizations) ==  1:
        return planOptimizations[0]
    elif SizeOfIterator(planOptimizations) ==  2:
        dicomPlanLabel = beamSet.DicomPlanLabel
        for po in planOptimizations:
            for optimizedBeamSet in po.OptimizedBeamSets:
                if dicomPlanLabel == optimizedBeamSet.DicomPlanLabel:
                    return po

    return None

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

def GetRoiDetails(structureSet):
    roiGeometries = structureSet.RoiGeometries

    roiDetails = {}
    for roiGeometry in roiGeometries:
        #cast into python boolean for json
        roiDetails[roiGeometry.OfRoi.Name] = {'HasContours':bool(roiGeometry.HasContours()), 'Type':roiGeometry.OfRoi.Type}
        if bool(roiGeometry.HasContours()):
            roiDetails[roiGeometry.OfRoi.Name]['Volume'] = roiGeometry.GetRoiVolume()
        else:
            roiDetails[roiGeometry.OfRoi.Name]['Volume'] = 0
    return roiDetails

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

def ClinicalGoalDict(clinicalGoal):
    clinicalGoalDict = {};
    clinicalGoalDict['RoiName'] = clinicalGoal.RoiName
    clinicalGoalDict['GoalCriteria'] = clinicalGoal.GoalCriteria
    clinicalGoalDict['GoalType'] = clinicalGoal.GoalType
    clinicalGoalDict['AcceptanceLevel'] = clinicalGoal.AcceptanceLevel
    clinicalGoalDict['ParameterValue'] = clinicalGoal.ParameterValue
    clinicalGoalDict['IsComparativeGoal'] = clinicalGoal.IsComparativeGoal
    clinicalGoalDict['Priority'] = clinicalGoal.Priority
    return clinicalGoalDict

def AddClinicalGoal(plan, clinicalGoal):
    plan.TreatmentCourse.EvaluationSetup.AddClinicalGoal(RoiName=clinicalGoal.RoiName,
                                                         GoalCriteria=clinicalGoal.GoalCriteria,
                                                         GoalType=clinicalGoal.GoalType, 
                                                         AcceptanceLevel=clinicalGoal.AcceptanceLevel,
                                                         ParameterValue=clinicalGoal.ParameterValue,
                                                         IsComparativeGoal=clinicalGoal.IsComparativeGoal,
                                                         Priority=clinicalGoal.Priority)

def SetOptimizationFunction(beamSet, planOptimization, objectiveConstituentFunction, order, optimizationFunction):

        planLabel = GetPlanLabelForConstituentFunction(beamSet, planOptimization, objectiveConstituentFunction)
        print planLabel
        optimizationFunction.PlanLabel = planLabel

        roiName = objectiveConstituentFunction.ForRegionOfInterest.Name
        optimizationFunction.RoiName = roiName

        functionType = GetOptimizationFunctionType(objectiveConstituentFunction)
        optimizationFunction.FunctionType = functionType

        parameters = objectiveConstituentFunction.DoseFunctionParameters
        
        #Common for all types
        weight = parameters.Weight
        lqModelParameters = parameters.LqModelParameters

        optimizationFunction.Order = order
        optimizationFunction.Weight = weight
        optimizationFunction.LqModelParameters = lqModelParameters

        doseLevel = parameters.DoseLevel if hasattr(parameters, 'DoseLevel') else 0
        percentVolume = parameters.PercentVolume if hasattr(parameters, 'PercentVolume') else 0
        eudParameterA = parameters.EudParameterA if hasattr(parameters, 'EudParameterA') else 1

        #Dose-fall off
        adaptToTargetDoseLevels = parameters.AdaptToTargetDoseLevels if hasattr(parameters, 'AdaptToTargetDoseLevels') else False
        highDoseLevel = parameters.HighDoseLevel if hasattr(parameters, 'HighDoseLevel') else 0
        lowDoseLevel = parameters.LowDoseLevel if hasattr(parameters, 'LowDoseLevel') else 0
        lowDoseDistance = parameters.LowDoseDistance if hasattr(parameters, 'LowDoseDistance') else 0

        if (functionType == 'MaxDose' or functionType == 'MinDose' or functionType == 'UniformDose'):
            optimizationFunction.DoseLevel = doseLevel
        elif (functionType == 'MaxDvh' or functionType == 'MinDvh'):
            optimizationFunction.DoseLevel = doseLevel
            optimizationFunction.PercentVolume = percentVolume
        elif (functionType == 'MaxEud' or functionType == 'MinEud' or functionType == 'TargetEud'):
            optimizationFunction.DoseLevel = doseLevel
            optimizationFunction.PercentVolume = percentVolume
            optimizationFunction.EudParameterA = eudParameterA
        elif (functionType == 'DoseFallOff'):
            optimizationFunction.HighDoseLevel = highDoseLevel
            optimizationFunction.LowDoseLevel = lowDoseLevel
            optimizationFunction.LowDoseDistance = lowDoseDistance
            optimizationFunction.AdaptToTargetDoseLevels = adaptToTargetDoseLevels
        else:
            optimizationFunction.FunctionType = 'NotImplemented'

def UpdateObjectiveConstituentFunctionWeights(objectiveConstituentFunctions, optimizationFunctions):
    for f in optimizationFunctions:
        order = f.Order
        objectiveConstituentFunctions[order].DoseFunctionParameters.Weight = f.Weight

def BoostObjectiveConstituentFunctionWeights(objectiveConstituentFunctions, optimizationFunctions):
    for f in optimizationFunctions:
        order = f.Order
        if f.IsBoosted:
            objectiveConstituentFunctions[order].DoseFunctionParameters.Weight = f.BoostedWeight

def GetPlanLabelForConstituentFunction(currentBeamSet, planOptimization, constituentFunction):
    if SizeOfIterator(planOptimization.OptimizedBeamSets) == 1:
        return currentBeamSet.DicomPlanLabel
    elif SizeOfIterator(planOptimization.OptimizedBeamSets) == 2:
        return constituentFunction.OfDoseDistribution.ForBeamSet.DicomPlanLabel if hasattr(constituentFunction.OfDoseDistribution, 'ForBeamSet') else 'Combined dose'
    else:
        return None

def SetMaxArcMu(beamSetting, maxArcMu):
    
    properties = beamSetting.ArcConversionPropertiesPerBeam

    print 'SetMaxArcMu: CreateDualArcs -> False and BurstGantrySpacing -> None'

    conformalArcStyle = properties.ConformalArcStyle  
    createDualArcs = False
    finalGantrySpacing = properties.FinalArcGantrySpacing
    maxArcDeliveryTime = properties.MaxArcDeliveryTime
    burstGantrySpacing = None
    presentMaxArcMU = properties.MaxArcMU

    if IsEqualNone(maxArcMu, presentMaxArcMU):
        return

    beamSetting.ArcConversionPropertiesPerBeam.EditArcBasedBeamOptimizationSettings(ConformalArcStyle=conformalArcStyle, CreateDualArcs=createDualArcs, FinalGantrySpacing=finalGantrySpacing, MaxArcDeliveryTime=maxArcDeliveryTime, BurstGantrySpacing=burstGantrySpacing, MaxArcMU=maxArcMu)

def ClearAllClinicalGoals(plan):
    for cg in plan.TreatmentCourse.EvaluationSetup.EvaluationFunctions:
        plan.TreatmentCourse.EvaluationSetup.DeleteClinicalGoal(FunctionToRemove=cg)

# gridResolution should be in mm
def SetDoseGridResolution(plan, gridResolution):

    doseGrid = plan.GetDoseGrid()

    cornerX = doseGrid.Corner.x
    cornerY = doseGrid.Corner.y
    cornerZ = doseGrid.Corner.z

    voxelSizeX = doseGrid.VoxelSize.x
    voxelSizeY = doseGrid.VoxelSize.y
    voxelSizeZ = doseGrid.VoxelSize.z

    nrVoxelX = doseGrid.NrVoxels.x
    nrVoxelY = doseGrid.NrVoxels.y
    nrVoxelZ = doseGrid.NrVoxels.z

    print cornerX, cornerY, cornerZ, voxelSizeX, voxelSizeY, voxelSizeZ, nrVoxelX, nrVoxelY, nrVoxelY
    print gridResolution

    # mm to cm for gridResolution
    if not (gridResolution/10. == voxelSizeX and gridResolution/10. == voxelSizeY and gridResolution/10. == voxelSizeZ):

        nrVoxelX = int(math.ceil(float(nrVoxelX-1)*(10.*float(voxelSizeX))/float(gridResolution))) + 1
        nrVoxelY = int(math.ceil(float(nrVoxelY-1)*(10.*float(voxelSizeY))/float(gridResolution))) + 1
        nrVoxelZ = int(math.ceil(float(nrVoxelZ-1)*(10.*float(voxelSizeZ))/float(gridResolution))) + 1

        plan.UpdateDoseGrid(Corner={ 'x': cornerX, 'y': cornerY, 'z': cornerZ }, VoxelSize={ 'x': gridResolution/10., 'y': gridResolution/10., 'z': gridResolution/10. }, NumberOfVoxels={ 'x': nrVoxelX, 'y': nrVoxelY, 'z': nrVoxelZ })
        plan.TreatmentCourse.TotalDose.UpdateDoseGridStructures()

# x, y, z should be given in the DICOM coordinate system and cm
def GetDoseValueDCM(x, y, z, doseGrid, doseData):

    cornerX = doseGrid.Corner.x
    cornerY = doseGrid.Corner.y
    cornerZ = doseGrid.Corner.z

    voxelSizeX = doseGrid.VoxelSize.x
    voxelSizeY = doseGrid.VoxelSize.y
    voxelSizeZ = doseGrid.VoxelSize.z

    nrVoxelX = doseGrid.NrVoxels.x
    nrVoxelY = doseGrid.NrVoxels.y
    nrVoxelZ = doseGrid.NrVoxels.z

    nx1 = int((x - cornerX)/voxelSizeX)
    if(nx1 < 0):
        nx1 = 0
    if(nx1 >= nrVoxelX-1):
        nx1 = nrVoxelX - 2
    nx2 = nx1+1

    ny1 = int((y - cornerY)/voxelSizeY)
    if(ny1 < 0):
        ny1 = 0
    if(ny1 >= nrVoxelY-1):
        ny1 = nrVoxelY - 2
    ny2 = ny1+1

    nz1 = int((z - cornerZ)/voxelSizeZ)
    if(nz1 < 0):
        nz1 = 0
    if(nz1 >= nrVoxelZ-1):
        nz1 = nrVoxelZ - 2
    nz2 = nz1+1

    f111 = doseData[nx1, ny1, nz1]
    f112 = doseData[nx1, ny1, nz2]
    f121 = doseData[nx1, ny2, nz1]
    f122 = doseData[nx1, ny2, nz2]
    f211 = doseData[nx2, ny1, nz1]
    f212 = doseData[nx2, ny1, nz2]
    f221 = doseData[nx2, ny2, nz1]
    f222 = doseData[nx2, ny2, nz2]

    x1 = GetCoordinate1d(nx1, cornerX, voxelSizeX)
    x2 = GetCoordinate1d(nx2, cornerX, voxelSizeX)
    y1 = GetCoordinate1d(ny1, cornerY, voxelSizeY)
    y2 = GetCoordinate1d(ny2, cornerY, voxelSizeY)
    z1 = GetCoordinate1d(nz1, cornerZ, voxelSizeZ)
    z2 = GetCoordinate1d(nz2, cornerZ, voxelSizeZ)

    denominator = (x2-x1)*(y2-y1)*(z2-z1)
    denominator = 1.0/denominator

    value = f111*(x2-x)*(y2-y)*(z2-z)
    value += f112*(x2-x)*(y2-y)*(z-z1)
    value += f121*(x2-x)*(y-y1)*(z2-z)
    value += f122*(x2-x)*(y-y1)*(z-z1)
    value += f211*(x-x1)*(y2-y)*(z2-z)
    value += f212*(x-x1)*(y2-y)*(z-z1)
    value += f221*(x-x1)*(y-y1)*(z2-z)
    value += f222*(x-x1)*(y-y1)*(z-z1)

    return value*denominator

def GetGridCoordinate1d(n, x0, size):
    return x0+float(n)*float(size)

def IsEqualNone(a1, a2):
    if a1 is None:
        if a2 is None:
            return True
        else:
            return False
    else:
        if a2 is None:
            return False
        elif a1 == a2:
            return True
        else:
            return False

if __name__ == '__main__':

    MessageBox.Show('Hello world')

    case = get_current("Case")
    examination = get_current("Examination")
    #plan = get_current("Plan")

     
    case = get_current("Case")
    examination = get_current("Examination")

    #examinationName = examination.Name
    #structureSet = case.PatientModel.StructureSets[examinationName]

    structureSet = GetStructureSet(case, examination)

    rois = GetRoiDetails(structureSet)

    for key, value in rois.items():
        print key, value['HasContours'], value['Type'],  value['Volume']

    #jsonString = json.JSONEncoder().encode(rois)
    #jsonString = json.dumps(rois)
    #print jsonString

    #resultRoiName = 'zPTV1-Rectum'
    #sourceRoiNames = ['PTV1']
    #subtractedRoiNames = ['Rectum']
    #outerMargins = [0.1]*6

    #MakeRoisSubtractedRoi(case, examination, resultRoiName, sourceRoiNames, subtractedRoiNames, outerMargins, innerMargins=[0.2] * 6, resultMargins=[0] * 6, isDerived=True, color="Yellow", roiType='Control')
    
    #structureName = 'zTestRing1_UD'
    #baseStructureName = 'PTV1'
    #MakeRingRoi(case, examination, structureName, baseStructureName, 1.5, 0.2, isDerived=False)

    #structureName = 'zTestRectum-PTV1_01_UD'
    #baseStructureName = 'Rectum'
    #subtractedRoiName = 'PTV1'
    #MakeRoiSubtractedRoi(case, examination, structureName, baseStructureName, subtractedRoiName, 0.1, isDerived=False)

    #structureName = 'zTestBladder_03_UD'
    #baseStructureName = 'Bladder'
    #MakeMarginAddedRoi(case, examination, structureName, baseStructureName, 0.3, isDerived=False)

