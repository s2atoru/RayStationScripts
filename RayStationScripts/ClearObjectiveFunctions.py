from connect import *

dllsPath = os.environ["USERPROFILE"]
dllsPath = dllsPath + r"\Desktop\RayStationScripts\Dlls"
print(dllsPath)
sys.path.append(dllsPath)

RayStationScriptsPath = os.environ["USERPROFILE"] + r"\DeskTop\RayStationScripts" + "\\"
scriptsPath = RayStationScriptsPath + "Scripts"
print(scriptsPath)
sys.path.append(scriptsPath)

from Helpers import ClearBeamSetObjectiveFunction

plan = get_current('Plan')
beamSet = get_current('BeamSet')

ClearBeamSetObjectiveFunction(plan, beamSet)
