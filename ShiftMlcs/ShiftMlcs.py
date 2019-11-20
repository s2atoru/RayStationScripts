from connect import *

import clr

#clr.AddReference("PresentationFramework")
#clr.AddReference("PresentationCore")

import sys, math, wpf, os
from System.Collections.Generic import List
from System.Windows import MessageBox

RayStationScriptsPath = os.environ["USERPROFILE"] + r"\DeskTop\RayStationScripts" + "\\"

dllsPath = RayStationScriptsPath + "Dlls"
sys.path.append(dllsPath)

scriptsPath = RayStationScriptsPath + "Scripts"
sys.path.append(scriptsPath)

clr.AddReference("MlcShifter")

numberOfLeafPairs = 60
minimumLeafGap = 0.05

leafRetraction = 1.0

from MlcShifter.ViewModels import BeamViewModel
from MlcShifter.ViewModels import MlcShifterViewModel
from MlcShifter.Views import MainWindow

with CompositeAction('Shift MLCs'):

    beamViewModels = List[BeamViewModel]()
    beam_set = get_current("BeamSet")
    for beam in beam_set.Beams:
        beamId = beam.Name
        maximumGaps = List[System.Double]()
        for leafNumber in xrange(numberOfLeafPairs):
            maximumGaps.Add(0.0)

        minimumX1 = 45
        maximumX2 = -45
        minimumY1 = 45
        maximumY2 = -45

        for segment in beam.Segments:
            leafPositions = segment.LeafPositions
            
            x1 = segment.JawPositions[0]
            x2 = segment.JawPositions[1]
            y1 = segment.JawPositions[2]
            y2 = segment.JawPositions[3]

            if x1 < minimumX1:
                minimumX1 = x1
            if x2 > maximumX2:
                maximumX2 = x2
            if y1 < minimumY1:
                minimumY1 = y1
            if y2 > maximumY2:
                maximumY2 = y2

            for leafNumber in xrange(numberOfLeafPairs):
                leafGap = leafPositions[1][leafNumber] - leafPositions[0][leafNumber]
                if leafGap > maximumGaps[leafNumber]:
                    maximumGaps[leafNumber] = leafGap

        beamViewModel = BeamViewModel(beamId, maximumGaps, maximumX2)

        for m in beamViewModel.MlcShiftDetails:
            m.ShiftA = maximumX2 + leafRetraction
            m.ShiftB = maximumX2 + leafRetraction

        beamViewModels.Add(beamViewModel)

    mlcShifterViewModel = MlcShifterViewModel(beamViewModels)

    mainWindow = MainWindow(mlcShifterViewModel)
    mainWindow.ShowDialog();

    if mlcShifterViewModel.IsOk:

        isDifferentShift = mlcShifterViewModel.IsDifferentShift
        
        beams = beam_set.Beams
        for beamViewModel in mlcShifterViewModel.BeamViewModels:

            beamId = beamViewModel.BeamId
            beam = beams[beamId]

            for segment in beam.Segments:
                isMlcShifted = False
                leafPositions = segment.LeafPositions
                for mlcShiftDetail in beamViewModel.MlcShiftDetails:
                    if (not mlcShiftDetail.IsCheckedA) and (not mlcShiftDetail.IsCheckedB):
                        continue

                    isMlcShifted = True
                    leafNumber = mlcShiftDetail.LeafNumber
                    if isDifferentShift:
                        if mlcShiftDetail.IsCheckedA:
                            leafPositions[1][leafNumber-1] = mlcShiftDetail.ShiftA + minimumLeafGap
                        if mlcShiftDetail.IsCheckedB:
                            leafPositions[0][leafNumber-1] = mlcShiftDetail.ShiftB
                    else:
                        if mlcShiftDetail.IsCheckedB:
                            leafPositions[0][leafNumber-1] = mlcShiftDetail.ShiftB
                            leafPositions[1][leafNumber-1] = mlcShiftDetail.ShiftB + minimumLeafGap

                if isMlcShifted:
                    segment.LeafPositions = leafPositions
