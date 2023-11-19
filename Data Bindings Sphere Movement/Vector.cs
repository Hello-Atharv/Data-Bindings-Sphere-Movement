using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace DataBindingsSphereMovement
{
    class Vector : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        private double xValue;
        private double yValue;

        private Vector result;

        public Vector(double xValue, double yValue)
        {
            this.xValue = xValue;
            this.yValue = yValue;
        }

        public double XValue
        {
            get { return xValue; }
            set { xValue = value; OnPropertyChanged("XValue"); }
        }

        public double YValue
        {
            get { return yValue; }
            set { yValue = value; OnPropertyChanged("YValue"); }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public Vector AddVectors(bool changeVector, Vector vector)
        {

            if (changeVector)
            {
                result = this;
            }
            else
            {
                result = new Vector(0, 0);
            }

            result.XValue = XValue + vector.XValue;
            result.YValue = YValue + vector.YValue;

            return result;
        }

        public Vector ScalarMultiply(bool changeVector, double scalar)
        {

            if (changeVector)
            {
                result = this;
            }
            else
            {
                result = new Vector(0, 0);
            }

            result.XValue = XValue * scalar;
            result.YValue = YValue * scalar;

            return result;
        }

        public Vector SubtractVectors(bool changeVector, Vector vector)
        {
            if (changeVector)
            {
                result = this;
            }
            else
            {
                result = new Vector(0, 0);
            }

            result = AddVectors(changeVector, vector.ScalarMultiply(false, -1));

            return result;
        }

        public double Magnitude()
        {
            double result = Math.Sqrt(Math.Pow(XValue, 2) + Math.Pow(YValue, 2));

            return result;
        }

        public double DotProduct(Vector vector)
        {
            double result = XValue * vector.XValue + YValue * vector.YValue;

            return result;
        }

        public double SepDistance(Vector vector)
        {
            Vector sepVector = SubtractVectors(false, vector);

            double result = sepVector.Magnitude();

            return result;
        }

        public Vector ChangeBasis(Vector basisIVector, Vector basisJVector)
        {
            Vector iHat = basisIVector.ConvertToUnitVector();
            Vector jHat = basisJVector.ConvertToUnitVector();

            double xVal = (iHat.XValue * XValue) + (jHat.XValue * YValue);
            double yVal = (iHat.YValue * XValue) + (jHat.YValue * YValue);

            Vector result = new Vector(xVal, yVal);

            return result;
        }

        public Vector ConvertToUnitVector()
        {
            double magnitude = Magnitude();

            Vector result = new Vector((XValue / magnitude), (YValue / magnitude));

            return result;
        }

        public Vector Orthogonal()
        {
            Vector result = new Vector(YValue, XValue * -1);

            return result;
        }

        public Vector InvertFromBasis(Vector basisIVector, Vector basisJVector)
        {
            Vector iHat = basisIVector.ConvertToUnitVector();
            Vector jHat = basisJVector.ConvertToUnitVector();

            double xVal = (jHat.YValue * XValue) + (-1 * jHat.XValue * YValue);
            double yVal = (-1 * iHat.YValue * XValue) + (iHat.XValue * YValue);

            Vector result = new Vector(xVal, yVal);

            return result;
        }
    }
}
