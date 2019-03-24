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


import HelloWorld

HelloWorld.HelloWorld()

print('Hello world')


pass