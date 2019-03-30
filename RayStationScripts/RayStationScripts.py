import clr
clr.AddReference("PresentationFramework")
clr.AddReference("PresentationCore")

import sys, os
import json

from System.Collections.Generic import List, Dictionary

RayStationScriptsPath = os.environ["USERPROFILE"] + r"\DeskTop\RayStationScripts" + "\\"

dllsPath = RayStationScriptsPath + "Dlls"
print(dllsPath)
sys.path.append(dllsPath)

scriptsPath = RayStationScriptsPath + "Scripts"
print(scriptsPath)
sys.path.append(scriptsPath)

print ','.join(sys.path)

clr.AddReference("RoiFormulaMaker")
from RoiFormulaMaker.Views import MainWindow
from RoiFormulaMaker.Models import RoiFormulas

structureFormulas = RoiFormulas()

structureNames = List[str](['PTV', 'Rectum', 'Bladder', 'FemoralHeads'])

roiFormulasPath = RayStationScriptsPath + "RoiFormulas"

structureDetails_dict = {"PTV": {"HasContours":True, "Type":"Ptv"}, "Rectum":{"HasContours":True, "Type":"Organ"},
                        "Bladder":{"HasContours":True, "Type":"Organ"}, "zRing":{"HasContours":False, "Type":"Control"}}

structureDetails = Dictionary[str, Dictionary[str,object]]()

for key, value in structureDetails_dict.items():
    structureDetails.Add(key, Dictionary[str,object](value))

print structureDetails

mainWindow = MainWindow(structureDetails, structureFormulas, roiFormulasPath)
mainWindow.ShowDialog();

#Python JSON encoder and decoder
#https://docs.python.org/2.7/library/json.html
jsonList = []
for s in structureFormulas.Formulas:
    print(s,s.ToJson())
    #To dictionary
    jsonList.append(json.loads(s.ToJson()))

from StringIO import StringIO
io = StringIO()
#JSON list to JSON text
json.dump(jsonList, io)
print io.getvalue()

if structureFormulas.CanExecute == False:
    print "Canceled"
    sys.exit()


#ringRoiParameters = RingRoiParameters();
#marginAddedRoiParameters = MarginAddedRoiParameters();

#print marginAddedRoiParameters.ToJson();


#from Helpers import MarginDict

#marginDict = MarginDict([4]*8)

#print marginDict

#sys.path.append(r"C:\Users\satoru\Source\Repos\RayStationScripts\OptimizatoinSettings\bin\Debug")
#clr.AddReference("OptimizatoinSettings")

#from OptimizatoinSettings.Models import SettingParameters
#from OptimizatoinSettings.Views import MainWindow

#settingParameters = SettingParameters()

#settingParameters.MaxNumberOfIterations = 50

#print("{0}, {1}, {2}, {3}, {4}".format(settingParameters.MaxNumberOfIterations, settingParameters.IterationsInPreparationsPhase, settingParameters.ComputeFinalDose, settingParameters.IsValid, settingParameters.CanSetParameters))

#settingParameters.MaxNumberOfIterations = 10

#print("{0}, {1}, {2}, {3}, {4}".format(settingParameters.MaxNumberOfIterations, settingParameters.IterationsInPreparationsPhase, settingParameters.ComputeFinalDose, settingParameters.IsValid, settingParameters.CanSetParameters))

#mainWindow = MainWindow(settingParameters)

#mainWindow.ShowDialog()

#print("{0}, {1}, {2}, {3}, {4}".format(settingParameters.MaxNumberOfIterations, settingParameters.IterationsInPreparationsPhase, settingParameters.ComputeFinalDose, settingParameters.IsValid, settingParameters.CanSetParameters))

print('Hello world')
pass