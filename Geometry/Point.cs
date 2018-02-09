using System;
using System.Diagnostics.Contracts;

namespace Clothoid.Geometry
{
    public struct Point : IEquatable<Point>
    {
        public const double Eps = 1e-12;

        /// <summary>
        /// Not a point
        /// </summary>
        public static Point NaP = new Point(double.NaN, double.NaN);
        
        public static Point operator + (Point point, Vector vector)
        {
            return new Point(point.X + vector.X, point.Y + vector.Y);
        }

        public static Point operator - (Point point, Vector vector)
        {
            return point + (-vector);
        }
        
        public static Vector operator - (Point a, Point b)
        {
            return new Vector(b, a);
        }

        public static explicit operator Vector(Point point)
        {
            return new Vector(point);
        }
        
        public static Point Zero = new Point(0, 0);
        
        public readonly double X;
        public readonly double Y;

        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }

        public bool IsNaP
        {
            get
            {
                return double.IsNaN(X) || double.IsNaN(Y);
            }
        }

        public double DistanceTo(Point another)
        {
            return Math.Sqrt((this.X - another.X)*(this.X - another.X) + (this.Y - another.Y)*(this.Y - another.Y));
        }

        /// <summary>
        /// Rotates point around (0; 0)
        /// </summary>
        public Point Rotate(UnitVector angle)
        {
            return new Point(
                X * angle.X  - Y * angle.Y, 
                X * angle.Y  + Y * angle.X);
        }

        public Point Rotate(double angle, Point center)
        {
            return Rotate(UnitVector.FromAngle(angle), center);
        }
        
        public Point Rotate(UnitVector angle, Point center)
        {
            var v = new Vector(center);

            return Move(-v).Rotate(angle).Move(v);
        }

        public Point Move(Vector shift)
        {
            return new Point(X + shift.X, Y + shift.Y);
        }

        public override string ToString()
        {
            return string.Format("({0}; {1})", X, Y);
        }

        #region Equality members

        public bool Equals(Point other)
        {
            return X.Equals(other.X) && Y.Equals(other.Y);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Point && Equals((Point) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X.GetHashCode() * 397) ^ Y.GetHashCode();
            }
        }

        #endregion
    }
}
