using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Optimization;
using OptimizatoinSettings.ViewModels;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Media.Media3D;


namespace RoiCenterOfMass.ViewModels
{
    public class RoiCenterOfMassesViewModel : BindableBaseWithErrorsContainer
    {
        public ObservableCollection<Models.RoiCenterOfMass> RoiCenterOfMasses { get; }

        public RoiCenterOfMassesViewModel(IEnumerable<Models.RoiCenterOfMass> roiCenterOfMasses)
        {
            RoiCenterOfMasses = new ObservableCollection<Models.RoiCenterOfMass>(roiCenterOfMasses);

            UpdateCenterOfMass();

            SetOptimizedIsocenterToCenterOfMass();

            OptimizeCommand = new DelegateCommand(() => OptimizeIsocenterPosition());
            OkCommand = new DelegateCommand(() => { CanExecute = true; });
            CancelCommand = new DelegateCommand(() => { CanExecute = false; });
            ChangeInUseCommand = new DelegateCommand(() => { UpdateCenterOfMass(); SetOptimizedIsocenterToCenterOfMass(); SetVisibility(); });
            SetVisibilityCommand = new DelegateCommand(() => { SetVisibility(); });
        }

        private bool isAllVisible = false;
        public bool IsAllVisible
        {
            get { return isAllVisible; }
            set { SetProperty(ref isAllVisible, value); }
        }

        private bool canExecute = false;
        public bool CanExecute
        {
            get { return canExecute; }
            set { SetProperty(ref canExecute, value); }
        }

        private double maximumDistance;
        public double MaximumDistance
        {
            get { return maximumDistance; }
            set { SetProperty(ref maximumDistance, value); }
        }

        private double maximumDistanceForCenterOfMass;
        public double MaximumDistanceForCenterOfMass
        {
            get { return maximumDistanceForCenterOfMass; }
            set { SetProperty(ref maximumDistanceForCenterOfMass, value); }
        }

        private Point3D centerOfMass;
        public Point3D CenterOfMass
        {
            get { return centerOfMass; }
            set { SetProperty(ref centerOfMass, value); }
        }

        private double opitimizedIsocenterX;
        public double OptimizedIsocenterX
        {
            get { return opitimizedIsocenterX; }
            set
            {
                SetProperty(ref opitimizedIsocenterX, value);
                UpdateOptimizedIsocenter();
            }
        }

        private double opitimizedIsocenterY;
        public double OptimizedIsocenterY
        {
            get { return opitimizedIsocenterY; }
            set
            {
                SetProperty(ref opitimizedIsocenterY, value);
                UpdateOptimizedIsocenter();
            }
        }

        private double opitimizedIsocenterZ;
        public double OptimizedIsocenterZ
        {
            get { return opitimizedIsocenterZ; }
            set
            {
                SetProperty(ref opitimizedIsocenterZ, value);
                UpdateOptimizedIsocenter();
            }
        }

        private double convergenceTolerance = 1.0e-5;
        public double ConvergenceTorelance
        {
            get { return convergenceTolerance; }
            set { SetProperty(ref convergenceTolerance, value); }
        }

        private int maximumIterations = 1000;
        public int MaximumIterations
        {
            get { return maximumIterations; }
            set { SetProperty(ref maximumIterations, value); }
        }

        private MinimizationResult minimizationResult;
        public MinimizationResult MinimizationResult
        {
            get { return minimizationResult; }
            set { SetProperty(ref minimizationResult, value); }
        }

        public int MinimizationResultNumberOfIterations { get; private set; }
        public string MinimizationResultReasonForExit { get; private set; }

        public DelegateCommand OptimizeCommand { get; private set; }
        public DelegateCommand OkCommand { get; private set; }
        public DelegateCommand CancelCommand { get; private set; }
        public DelegateCommand ChangeInUseCommand { get; private set; }
        public DelegateCommand SetVisibilityCommand { get; private set; }

        private void UpdateCenterOfMass()
        {
            int n = 0;
            Point3D c = new Point3D(0.0, 0.0, 0.0);
            foreach (var p in RoiCenterOfMasses)
            {
                if (p.InUse)
                {
                    n += 1;
                    c.X += p.Coordinates.X;
                    c.Y += p.Coordinates.Y;
                    c.Z += p.Coordinates.Z;
                }
            }
            c.X /= n;
            c.Y /= n;
            c.Z /= n;

            CenterOfMass = c;

            foreach (var p in RoiCenterOfMasses)
            {
                if (p.InUse)
                {
                    p.CenterOfMassCoordinates = c;
                }
            }

            MaximumDistanceForCenterOfMass = MaximumDistanceFromRoiCenterOfMasses(CenterOfMass);
        }

        private void UpdateOptimizedIsocenter()
        {
            var optimizedIsocenter = new Point3D(OptimizedIsocenterX, OptimizedIsocenterY, OptimizedIsocenterZ);
            foreach (var p in RoiCenterOfMasses)
            {
                if (p.InUse)
                {
                    p.IsocenterCoordinates = optimizedIsocenter;
                }
            }

            MaximumDistance = MaximumDistanceFromRoiCenterOfMasses(optimizedIsocenter);
        }

        private void SetOptimizedIsocenterToCenterOfMass()
        {
            OptimizedIsocenterX = CenterOfMass.X;
            OptimizedIsocenterY = CenterOfMass.Y;
            OptimizedIsocenterZ = CenterOfMass.Z;

            foreach (var p in RoiCenterOfMasses)
            {
                if (p.InUse)
                {
                    p.IsocenterCoordinates = CenterOfMass;
                }
            }

            MaximumDistance = MaximumDistanceForCenterOfMass;
        }

        public void OptimizeIsocenterPosition()
        {
            var V = Vector<double>.Build;

            double[] x0 = new double[3] { OptimizedIsocenterX, OptimizedIsocenterY, OptimizedIsocenterZ };

            var x0Vec = V.Dense(x0);

            Func<Vector<double>, double> func = (Vector<double> x) => MaximumDistanceFromRoiCenterOfMasses(x);
            var objectiveFunction = ObjectiveFunction.Value(func);
            var solver = new NelderMeadSimplex(ConvergenceTorelance, MaximumIterations);

            MinimizationResult = solver.FindMinimum(objectiveFunction, x0Vec);

            MinimizationResultNumberOfIterations = MinimizationResult.Iterations;
            MinimizationResultReasonForExit = MinimizationResult.ReasonForExit.ToString();

            var minPoint = MinimizationResult.MinimizingPoint;
            MaximumDistance = MaximumDistanceFromRoiCenterOfMasses(minPoint);
            OptimizedIsocenterX = minPoint[0];
            OptimizedIsocenterY = minPoint[1];
            OptimizedIsocenterZ = minPoint[2];
        }

        public double MaximumDistanceFromRoiCenterOfMasses(double[] x)
        {
            if (x == null)
            {
                throw new ArgumentNullException(nameof(x));
            }

            int numPoints = RoiCenterOfMasses.Count;

            double maxDistance = -1.0;
            for (int i = 0; i < numPoints; i++)
            {
                var p = RoiCenterOfMasses[i];
                if (!p.InUse) continue;

                double d = 0.0;
                d += Math.Pow(x[0] - p.Coordinates.X, 2);
                d += Math.Pow(x[1] - p.Coordinates.Y, 2);
                d += Math.Pow(x[2] - p.Coordinates.Z, 2);
                d = Math.Sqrt(d);

                if (d > maxDistance)
                {
                    maxDistance = d;
                }
            }

            return maxDistance;
        }
        public double MaximumDistanceFromRoiCenterOfMasses(Vector<double> x)
        {
            if (x == null)
            {
                throw new ArgumentNullException(nameof(x));
            }

            if (x.Count != 3)
            {
                throw new InvalidOperationException($"Dim of x ({x.Count}) should be 3");
            }

            int dim = 3;
            double[] point = new double[dim];
            for (int i = 0; i < dim; i++)
            {
                point[i] = x[i];
            }

            return MaximumDistanceFromRoiCenterOfMasses(point);
        }

        public double MaximumDistanceFromRoiCenterOfMasses(Point3D p)
        {
            if (p == null)
            {
                throw new ArgumentNullException(nameof(p));
            }

            double[] point = new double[3] { p.X, p.Y, p.Z };

            return MaximumDistanceFromRoiCenterOfMasses(point);
        }

        private void SetVisibility()
        {
            foreach (var r in RoiCenterOfMasses)
            {
                r.IsVisible = r.InUse | IsAllVisible;
            }
        }
    }
}
