using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace RoiCenterOfMass.Models
{
    public class RoiCenterOfMass : BindableBase
    {
        private string id;
        public string Id
        {
            get { return id; }
            set { SetProperty(ref id, value); }
        }

        private bool inUse = true;
        public bool InUse
        {
            get { return inUse; }
            set { SetProperty(ref inUse, value); }
        }

        private Point3D coordinates = new Point3D();
        public Point3D Coordinates
        {
            get { return coordinates; }
            set { SetProperty(ref coordinates, value); }
        }

        private Point3D isocenterCoordinates = new Point3D();
        public Point3D IsocenterCoordinates
        {
            get { return isocenterCoordinates; }
            set
            {
                SetProperty(ref isocenterCoordinates, value);
                DistanceFromIsocenter = GetDistanceFromPoint(isocenterCoordinates);
            }
        }

        private double distanceFromIsocenter;
        public double DistanceFromIsocenter
        {
            get { return distanceFromIsocenter; }
            set
            {
                SetProperty(ref distanceFromIsocenter, value);
            }
        }

        private Point3D centerOfMassCoordinates;
        public Point3D CenterOfMassCoordinates
        {
            get { return centerOfMassCoordinates; }
            set
            {
                SetProperty(ref centerOfMassCoordinates, value);
                DistanceFromCenterOfMass = GetDistanceFromPoint(CenterOfMassCoordinates);
            }
        }

        private double distanceFromCenterOfMass;
        public double DistanceFromCenterOfMass
        {
            get { return distanceFromCenterOfMass; }
            set { SetProperty(ref distanceFromCenterOfMass, value); }
        }

        private double GetDistanceFromPoint(Point3D point)
        {
            double r = Math.Pow(Coordinates.X - point.X, 2);
            r += Math.Pow(Coordinates.Y - point.Y, 2);
            r += Math.Pow(Coordinates.Z - point.Z, 2);

            r = Math.Sqrt(r);

            return r;
        }
    }
}
