#from connect import *

import clr
import sys, math, wpf, os
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
from OptimizationRepeater.Views import MainWindow

numberOfRepetitionTimes = 2
scaleDoseAfterEachIteration = True
scaleDoseAfterLastIteration = True
resetBeforeStartingOptimization = False

repetitionParameters = RepetitionParameters()

repetitionParameters.NumberOfRepetitionTimes = numberOfRepetitionTimes
repetitionParameters.ScaleDoseAfterEachIteration = scaleDoseAfterEachIteration
repetitionParameters.ScaleDoseAfterLastIteration = scaleDoseAfterLastIteration
repetitionParameters.ResetBeforeStartingOptimization = resetBeforeStartingOptimization

mainWindow = MainWindow(repetitionParameters)
mainWindow.ShowDialog()

numberOfRepetitionTimes = repetitionParameters.NumberOfRepetitionTimes
scaleDoseAfterEachIteration = repetitionParameters.ScaleDoseAfterEachIteration
scaleDoseAfterLastIteration = repetitionParameters.ScaleDoseAfterLastIteration
resetBeforeStartingOptimization = repetitionParameters.ResetBeforeStartingOptimization
canExecute = repetitionParameters.CanExecute 

print numberOfRepetitionTimes, scaleDoseAfterEachIteration, scaleDoseAfterLastIteration, resetBeforeStartingOptimization, canExecute

if (canExecute):
    if(resetBeforeStartingOptimization):
        MessageBox.Show("Reset Optimization")
        #Execute Reset Optimization
        
    for i in xrange(numberOfRepetitionTimes):

        print "Start optimization {0}".format(i+1)
        # Execute Optimization

        if(i+1 == numberOfRepetitionTimes and scaleDoseAfterLastIteration):
            print "Scale dose after last iteration {0}".format(i+1)
            #Execute Scale dose

        if(i+1 < numberOfRepetitionTimes and scaleDoseAfterEachIteration):
            print "Scale dose after iteration {0}".format(i+1)
            #Execute Scale dose


pass