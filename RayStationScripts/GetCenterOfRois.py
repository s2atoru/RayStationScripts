from connect import *

plan = get_current("Plan")
ss = plan.GetStructureSet()
rois = ss.RoiGeometries

#roiNames = ['PTV1', 'PTV2', 'PTV3', 'PTV4', 'PTV5', 'PTV6']
#roiNames = ['PTV1', 'PTV2', 'PTV3']

roiNames = ['PTV', 'PTV2', 'PTV3']

xsum = 0
ysum = 0
zsum = 0
numRois = 0
for name in roiNames:
    roi = rois[name]
    center = roi.GetCenterOfRoi()
    print "{0}: {1:.2f}, {2:.2f}, {3:.2f}".format(name, center.x, center.y, center.z)
    numRois += 1
    xsum += center.x
    ysum += center.y
    zsum += center.z

print "Center of mass: {0:.2f}, {1:.2f}, {2:.2f}".format(xsum/numRois, ysum/numRois, zsum/numRois)
