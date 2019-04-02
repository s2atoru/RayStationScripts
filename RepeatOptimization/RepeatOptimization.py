#from connect import *

import clr
import sys, math, wpf, os
from System.Collections.Generic import List;
from System.Windows import MessageBox

clr.AddReference("PresentationFramework")
clr.AddReference("PresentationCore")

#sys.path.append(r"C:\Users\satoru\Source\Repos\RayStationScripts\OptimizatoinSettings\bin\Debug")

RayStationScriptsPath = os.environ["USERPROFILE"] + r"\DeskTop\RayStationScripts" + "\\"
dllsPath = RayStationScriptsPath + "Dlls"
print(dllsPath)
sys.path.append(dllsPath)

scriptsPath = RayStationScriptsPath + "Scripts"
print(scriptsPath)
sys.path.append(scriptsPath)

clr.AddReference("OptimizationRepeater")

from OptimizationRepeater.Models import RepetitionParameters
from OptimizationRepeater.Models import OptimizationFunction
from OptimizationRepeater.Views import MainWindow

numberOfRepetitionTimes = 2
scaleDoseAfterEachIteration = True
scaleDoseAfterLastIteration = False
resetBeforeStartingOptimization = False

repetitionParameters = RepetitionParameters()

repetitionParameters.NumberOfRepetitionTimes = numberOfRepetitionTimes
repetitionParameters.ScaleDoseAfterEachIteration = scaleDoseAfterEachIteration
repetitionParameters.ScaleDoseAfterLastIteration = scaleDoseAfterLastIteration
repetitionParameters.ResetBeforeStartingOptimization = resetBeforeStartingOptimization

#plan = get_current("Plan")
#beamSet = get_current("BeamSet")
#optimizations = plan.PlanOptimizations
#planOptimizatoin = GetPlanOptimizationForBeamSet(optimizations, beamSet)
#objectiveConstituentFunctions = planOptimizatoin.Objective.ConstituentFunctions

optimizationFunctions = List[OptimizationFunction]()

optimizationFunction = OptimizationFunction()
optimizationFunction.Order = 0
optimizationFunction.RoiName = "Bladder"
optimizationFunction.FunctionType = "MaxEud"
optimizationFunction.DoseLevel = 3000
optimizationFunction.PercentVolume = 30
optimizationFunction.EudParameterA = 1.0
optimizationFunction.Weight = 1
optimizationFunction.BoostedWeight = 1
optimizationFunction.IsBoosted = False

optimizationFunctions.Add(optimizationFunction)

optimizationFunction = OptimizationFunction()
optimizationFunction.Order = 1
optimizationFunction.RoiName = "Bladder"
optimizationFunction.FunctionType = "MaxDose"
optimizationFunction.DoseLevel = 3000
optimizationFunction.Weight = 2
optimizationFunction.BoostedWeight = 3
optimizationFunction.IsBoosted = False

optimizationFunctions.Add(optimizationFunction)

optimizationFunction = OptimizationFunction()
optimizationFunction.Order = 2
optimizationFunction.RoiName = "PTV"
optimizationFunction.FunctionType = "UniformDose"
optimizationFunction.DoseLevel = 7600
optimizationFunction.Weight = 1
optimizationFunction.BoostedWeight = 100
optimizationFunction.IsBoosted = True

optimizationFunctions.Add(optimizationFunction)

#for i, f in enumerate(objectiveConstituentFunctions):
#    optimizatonFunction = OptimizationFunction()
#    SetOptimizationFunction(objectiveConstituentFunction, i, optimizatoinFunction)
#    optimizationFunctions.Add(optimizationFunction)

mainWindow = MainWindow(repetitionParameters, optimizationFunctions)
mainWindow.ShowDialog()

numberOfRepetitionTimes = repetitionParameters.NumberOfRepetitionTimes
scaleDoseAfterEachIteration = repetitionParameters.ScaleDoseAfterEachIteration
scaleDoseAfterLastIteration = repetitionParameters.ScaleDoseAfterLastIteration
resetBeforeStartingOptimization = repetitionParameters.ResetBeforeStartingOptimization
canExecute = repetitionParameters.CanExecute 

print numberOfRepetitionTimes, scaleDoseAfterEachIteration, scaleDoseAfterLastIteration, resetBeforeStartingOptimization, canExecute

for optimizationFunction in optimizationFunctions:
    print optimizationFunction.Order, optimizationFunction.Weight, optimizationFunction.IsBoosted, optimizationFunction.BoostedWeight, optimizationFunction 

from ScaleDose import ScaleDoseBeamSets

#ScaleDoseBeamSets(plan, beamSet)
#canExecute = False

if (canExecute):
    if(resetBeforeStartingOptimization):
        MessageBox.Show("Reset Optimization")
        #Execute Reset Optimization
        planOptimizatoin.ResetOptimization()
        
        UpdateObjectiveConstituentFunctionWeights(objectiveConstituentFunctions, optimizatonFunctions)

    for i in xrange(numberOfRepetitionTimes):

        print "Start optimization {0}".format(i+1)

        # Weight boosting at the last iteration
        if i + 1 == numberOfRepetitionTimes:
            print "Boost weights at last iteration"
            BoostObjectiveConstituentFunctionWeights(objectiveConstituentFunctions, optimizatonFunctions)

        # Execute Optimization
        planOptimizatoin.RunOptimization()

        if(i+1 == numberOfRepetitionTimes and scaleDoseAfterLastIteration):
            print "Scale dose after last iteration {0}".format(i+1)
            #Execute Scale dose
            ScaleDoseBeamSets(plan, beamSet)

        if(i+1 < numberOfRepetitionTimes and scaleDoseAfterEachIteration):
            print "Scale dose after iteration {0}".format(i+1)
            #Execute Scale dose
            ScaleDoseBeamSets(plan, beamSet)
pass
