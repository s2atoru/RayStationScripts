////////////////////////////////////////////////////////////////////////////////
// Helpers.cs
//
// Helper methods to manipulate courses etc.
//  
// Applies to: ESAPI v13, v13.5, v13.6.
//
// Copyright (c) 2015 Varian Medical Systems, Inc.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy 
// of this software and associated documentation files (the "Software"), to deal 
// in the Software without restriction, including without limitation the rights 
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
// copies of the Software, and to permit persons to whom the Software is 
// furnished to do so, subject to the following conditions:
//
//  The above copyright notice and this permission notice shall be included in 
//  all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN 
// THE SOFTWARE.
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Linq;
using System.Windows;

using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace Juntendo.MedPhys.Esapi
{
    public static class EsapiHelpers
    {
        public static bool CheckStructures(Patient patient)
        {
            if (patient.StructureSets.Any()) return true;
            const string message = "Patient does not have any structures.";
            const string title = "Invalid patient";
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }

        public static Course GetCourse(Patient patient, string courseId)
        {
            var res = patient.Courses.Where(c => c.Id == courseId);
            if (res.Any())
            {
                return res.Single();
            }
            else
            {
                throw new InvalidOperationException($"No corresponding Course: {courseId}");
            }
        }

        public static PlanSetup GetPlanSetup(Course course, string planId)
        {
            var res = course.PlanSetups.Where(p => p.Id == planId);
            if (res.Any())
            {
                return res.Single();
            }
            {
                return null;
            }
        }

        public static PlanSum GetPlanSum(Course course, string planSumId)
        {
            var res = course.PlanSums.Where(p => p.Id == planSumId);
            if (res.Any())
            {
                return res.Single();
            }
            {
                return null;
            }
        }

        public static PlanSum GetPlanSum(ScriptContext context, string planSumId)
        {
            var res = context.PlanSumsInScope.Where(p => p.Id == planSumId);
            if (res.Any())
            {
                return res.Single();
            }
            {
                return null;
            }
        }

        public static Structure GetStructure(StructureSet structureSet, string structureName)
        {
            var res = structureSet.Structures.Where(s => s.Id == structureName);
            if (res.Any())
            {
                return res.Single();
            }
            {
                return null;
            }
        }

        /// <summary>
        /// Get filled structure
        /// </summary>
        /// <param name="structureSet"> Structure set to search </param>
        /// <param name="structureName"> Structure name </param>
        /// <returns> Structure, the name of which is structureName. Return null if the structure is empty.</returns>
        public static Structure GetFilledStructure(StructureSet structureSet, string structureName)
        {
            var res = structureSet.Structures.Where(s => s.Id == structureName);
            if (res.Any())
            {
                if (res.Single().IsEmpty)
                {
                    return null;
                }

                return res.Single();
            }
            else
            {
                return null;
            }
        }

        public static Structure GetBodyStructure(PlanSetup planSetup)
        {
            var res = planSetup.StructureSet.Structures.Where(s => s.Id == "BODY");
            if (res.Any())
            {
                return res.Single();
            }
            {
                throw new InvalidOperationException("No Body Structure");
            }
        }

        public static string EsapiDoseUnitString(Dose doseEsapi)
        {

            int xSize = doseEsapi.XSize;
            int ySize = doseEsapi.YSize;

            int[,] voxelValuesInPlane = new int[xSize, ySize];
            doseEsapi.GetVoxels(0, voxelValuesInPlane);
            int voxelValue = voxelValuesInPlane[0, 0];

            DoseValue doseValue = doseEsapi.VoxelToDoseValue(voxelValue);

            return doseValue.UnitAsString;
        }

        public static double[,,] EsapiDoseToDose3dArray(Dose doseEsapi)
        {
            int xSize = doseEsapi.XSize;
            int ySize = doseEsapi.YSize;
            int zSize = doseEsapi.ZSize;

            double[,,] dose3dArray = new double[zSize, ySize, xSize];

            int[,] voxelValuesInPlane = new int[xSize, ySize];
            for (int i = 0; i < zSize; i++)
            {
                doseEsapi.GetVoxels(i, voxelValuesInPlane);

                for (int j = 0; j < ySize; j++)
                {
                    for (int k = 0; k < xSize ; k++)
                    {
                        int voxelValue = voxelValuesInPlane[k, j];
                        var doseValue = doseEsapi.VoxelToDoseValue(voxelValue);
                        dose3dArray[i, j, k] = doseValue.Dose;
                    }
                }
            }

            return dose3dArray;
        }


        public static double EsapiIndexToCoordinate(int i, double xOrigin, double Res)
        {
            return xOrigin + Res * (double)i;
        }

        public static double EsapiDoseXIndexToDicomCoodinate(int i, Dose doseEsapi)
        {
            double xRes = doseEsapi.XRes;
            double xOrigin = doseEsapi.Origin[0];

            double x = EsapiIndexToCoordinate(i, xOrigin, xRes);

            return x;
        }

        public static double EsapiDoseYIndexToDicomCoodinate(int i, Dose doseEsapi)
        {
            double yRes = doseEsapi.YRes;
            double yOrigin = doseEsapi.Origin[1];

            double y = EsapiIndexToCoordinate(i, yOrigin, yRes);

            return y;
        }

        public static double EsapiDoseZIndexToDicomCoodinate(int i, Dose doseEsapi)
        {
            double zRes = doseEsapi.ZRes;
            double zOrigin = doseEsapi.Origin[2];

            double z = EsapiIndexToCoordinate(i, zOrigin, zRes);

            return z;
        }

        public static double[,,] EsapiImageToImage3dArray(Image imageEsapi)
        {
            int xSize = imageEsapi.XSize;
            int ySize = imageEsapi.YSize;
            int zSize = imageEsapi.ZSize;

            double[,,] image3dArray = new double[zSize, ySize, xSize];

            int[,] voxelValuesInPlan = new int[xSize, ySize];
            for (int i = 0; i < zSize; i++)
            {
                Console.WriteLine($"Image to Array Reading Slice# {i:d}");
                imageEsapi.GetVoxels(i, voxelValuesInPlan);

                for (int j = 0; j < ySize; j++)
                {
                    for (int k = 0; k < xSize; k++)
                    {
                        int voxelValue = voxelValuesInPlan[k, j];
                        var imageValue = imageEsapi.VoxelToDisplayValue(voxelValue);
                        image3dArray[i, j, k] = imageValue;
                    }
                }
            }

            return image3dArray;
        }

        public static int ProfileNumberOfIntervals(VVector start, VVector end, double res=1.0)
        {
            double distance = VVector.Distance(start, end);
            int num = ((int)(distance / res));

            return num;
        }

        public static VVector ProfileEndVVector(VVector start, VVector end, double res=1.0)
        {
            double distance = VVector.Distance(start, end);
            int num = ProfileNumberOfIntervals(start, end, res);
            VVector truncatedEnd = start + ((num * res) / distance) * (end - start);

            return truncatedEnd;
        }

        public static bool[] ProfileStructureMask(VVector start, VVector end,  Structure structure, double res=1.0)
        {
            int numberOfIntervals = ProfileNumberOfIntervals(start, end, res);
            int numberOfPoints = numberOfIntervals + 1;

            VVector pointVVector;

            bool[] structureMask = new bool[numberOfPoints];

            double distance = VVector.Distance(start, end);
            for (int i =0; i < numberOfPoints; i++)
            {
                pointVVector = start + ((i * res) / distance) * (end - start);
                structureMask[i] = structure.IsPointInsideSegment(pointVVector);
            }

            return structureMask;
        }

        public static double[]  ImageProfile(Image imageEsapi, VVector start, VVector end, double res=1.0)
        {
            int numberOfIntervals = ProfileNumberOfIntervals(start, end, res);
            double[] imageProfile = new double[numberOfIntervals+1];
            imageEsapi.GetImageProfile(start, end, imageProfile);

            return imageProfile;
        }

        public static double BeamPointDose(Beam beam, VVector pointDicom)
        {
            DoseValue dose = beam.Dose.GetDoseToPoint(pointDicom);
            double muPerGy = beam.MetersetPerGy;
            double mu = beam.Meterset.Value;
            double refDose = mu / muPerGy;
            double pointDose = refDose * dose.Dose;

            return pointDose;
        }

        /// <summary>
        /// EDW angle
        /// </summary>
        /// <param name="beam"> Beam in ESAPI </param>
        /// <returns> EDW angle in degree (If no EDW exists, return 0) </returns>
        public static double EDWAngle(Beam beam)
        {
            if(beam.Wedges.Count() == 0)
            {
                return 0.0;
            }

            var edwId = beam.Wedges.First().Id;

            double edwAngle = 0.0;
            if(edwId.Contains("EDW"))
            {
                edwAngle = double.Parse(edwId.Substring(3, 2));
            }
            else
            {
                edwAngle = 0.0;
            }

            return edwAngle;
        }

        /// <summary>
        /// Check EDW direction
        /// </summary>
        /// <param name="beam">Beam in ESAPI </param>
        /// <returns> 1: IN, -1: OUT, 0: NONE </returns>
        public static int EDWDirectionIndex(Beam beam)
        {
            if (beam.Wedges.Count() == 0)
            {
                return 0;
            }

            var edwId = beam.Wedges.First().Id;
            if(edwId.Contains("EDW"))
            {
                if(edwId.Contains("IN"))
                {
                    return 1;
                }
                else if (edwId.Contains("OUT"))
                {
                    return -1;
                }
            }

            return 0;
        }
    }
}
