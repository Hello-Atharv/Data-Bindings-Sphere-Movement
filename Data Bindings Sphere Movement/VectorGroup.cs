using System;
using System.Collections.Generic;
using System.Text;

namespace DataBindingsSphereMovement
{
    class VectorGroup
    {

        public Vector AddVectors(bool changeVector, Vector vector1, Vector vector2)
        {
            Vector result;

            if (changeVector)
            {
                result = vector1;
            }
            else
            {
                result = new Vector(0, 0);
            }

            result.XValue = vector1.XValue + vector2.XValue;
            result.YValue = vector1.YValue + vector2.YValue;

            return result;
        }

        public Vector ScalarMultiply(bool changeVector, Vector vector, double scalar)
        {
            Vector result;

            if (changeVector)
            {
                result = vector;
            }
            else
            {
                result = new Vector(0, 0);
            }

            

            result.XValue = vector.XValue * scalar;
            result.YValue = vector.YValue * scalar;

            return result;
        }

        public Vector SubtractVectors(bool changeVector, Vector vector1, Vector vector2)
        {
            
            Vector result = AddVectors(changeVector, vector1, ScalarMultiply(false, vector2, -1));

            return result;
        }

        public double Magnitude(Vector vector)
        {
            double result = Math.Sqrt(Math.Pow(vector.XValue, 2) + Math.Pow(vector.YValue, 2));

            return result;
        }

        public double DotProduct(Vector vector1, Vector vector2)
        {
            double result = vector1.XValue * vector2.XValue + vector1.YValue * vector2.YValue;

            return result;
        }

        public double SepDistance(Vector vector1, Vector vector2)
        {
            Vector vector = SubtractVectors(false, vector1, vector2);

            double result = Magnitude(vector);

            return result;
        }

        public Vector ChangeBasis(Vector vector, Vector basisIVector, Vector basisJVector)
        {
            Vector iHat = ConvertToUnitVector(basisIVector);
            Vector jHat = ConvertToUnitVector(basisJVector);

            double xVal = (iHat.XValue * vector.XValue) + (jHat.XValue * vector.YValue);
            double yVal = (iHat.YValue * vector.XValue) + (jHat.YValue * vector.YValue);

            Vector result = new Vector(xVal, yVal);

            return result;
        }

        public Vector ConvertToUnitVector(Vector vector)
        {
            double magnitude = Magnitude(vector);

            Vector result = new Vector((vector.XValue/magnitude), (vector.YValue/magnitude));

            return result;
        }

        public Vector Orthogonal(Vector vector)
        {
            Vector result = new Vector(vector.YValue, vector.XValue*-1);

            return result;
        }

        public Vector InvertFromBasis(Vector vector, Vector basisIVector, Vector basisJVector)
        {
            Vector iHat = ConvertToUnitVector(basisIVector);
            Vector jHat = ConvertToUnitVector(basisJVector);

            double xVal = (jHat.YValue * vector.XValue) + (-1 * jHat.XValue * vector.YValue);
            double yVal = (-1 * iHat.YValue * vector.XValue) + (iHat.XValue * vector.YValue);

            Vector result = new Vector (xVal, yVal);

            return result;
        }

    }
}
