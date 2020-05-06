from connect import *

leafNumbers = [26, 27, 32, 33]
newLeafPosition = 5
newLeafGap = 0.05
beam_set = get_current("BeamSet")
with CompositeAction('Shift MLCs 26, 27, 32, 33'):
    for beam in beam_set.Beams:
        for segment in beam.Segments:
            leafPositions = segment.LeafPositions
            for leafNumber in leafNumbers:
                newLeafGap = leafPositions[1][leafNumber] - leafPositions[0][leafNumber]
                print newLeafGap, leafPositions[0][leafNumber], leafPositions[1][leafNumber]
                leafPositions[0][leafNumber] = newLeafPosition
                leafPositions[1][leafNumber] = newLeafPosition + newLeafGap
        
            segment.LeafPositions = leafPositions
