using System;

namespace ClothoidAndTheOthers.Geometry
{
    public struct UnitVector : IEquatable<UnitVector>
    {
        public static UnitVector operator - (UnitVector u)
        {
            return new UnitVector(-u.X, -u.Y);
        }

        public static double operator ^(UnitVector v1, UnitVector v2)
        {
            return v1.X * v2.Y - v1.Y * v2.X;
        }
        
        public static Vector operator * (UnitVector u, double d)
        {
            return new Vector(u.X * d, u.Y * d);
        }
        
        public static double operator * (UnitVector u1, UnitVector u2)
        {
            return u1.X*u2.X + u1.Y*u2.Y;
        }
        
        public static bool operator == (UnitVector u1, UnitVector u2)
        {
            return u1.X == u2.X && u1.Y == u2.Y;
        }

        public static bool operator != (UnitVector u1, UnitVector u2)
        {
            return !(u1 == u2);
        }

        public static Vector operator - (UnitVector u1, UnitVector u2)
        {
            return new Vector(u1.X - u2.X, u1.Y - u2.Y);
        }

        public static Vector operator +(UnitVector u1, UnitVector u2)
        {
            return new Vector(u1.X + u2.X, u1.Y + u2.Y);
        }

        public static implicit operator Vector (UnitVector u)
        {
            return new Vector(u.X, u.Y);
        }

        public readonly double X;
        public readonly double Y;

        public UnitVector(double x, double y)
        {
            if (Math.Abs(x * x + y * y - 1) > 1e-5)
                throw new ArgumentException();

            X = x;
            Y = y;
        }

        public UnitVector Normal
        {
            get
            {
                return new Vector(-Y, X).Unit;
            }
        }

        public bool IsValid
        {
            get { return Math.Abs(1 - (X*X + Y*Y)) < 1e-15; }
        }

        public UnitVector Rotate(double angle)
        {
            return Rotate(UnitVector.FromAngle(angle));
        }
        
        public UnitVector Rotate(UnitVector angle)
        {
            return new UnitVector(
                X * angle.X - Y * angle.Y,
                X * angle.Y + Y * angle.X);
        }
        
        public static UnitVector GetAngle(Vector start, Vector end)
        {
            var cos = (start*end)/(start.Length*end.Length);

            if (cos >= 1)
                // could be a little (at ~1e-15) greate then 1 deu to floating point numbers precision
                return new UnitVector(1, 0);

            var sin = Math.Sqrt(1 - cos*cos)*Math.Sign(start ^ end);
            return new UnitVector(cos, sin);
        }

        public static UnitVector GetAngle(UnitVector start, UnitVector end)
        {
            var cos = (start * end);
            var sin = Math.Sqrt(Math.Abs(1 - cos * cos)) * Math.Sign(start ^ end);
            return new UnitVector(cos, sin);
        }

        public static UnitVector FromDegrees(double degrees)
        {
            return FromAngle(degrees*Math.PI/180);
        }
        
        public static UnitVector FromAngle(double radians)
        {
            var x = Math.Cos(radians);
            var y = Math.Sin(radians);

            return new UnitVector(x, y);
        }

        /// <summary>
        /// Returns angle in radians in range 0..Pi*2
        /// </summary>
        public double To2PiAngle()
        {
            var a = Math.Atan2(Y, X);
            return a >= 0 ? a : a + Math.PI*2;
        }

        /// <summary>
        /// Returns angle in radians in range -Pi..Pi
        /// </summary>
        public double ToAngle()
        {
            return Math.Atan2(Y, X);
        }

        
        public bool Equals(UnitVector other)
        {
            return other.X == X && other.Y == Y;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != typeof (UnitVector)) return false;
            return Equals((UnitVector) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X.GetHashCode()*397) ^ Y.GetHashCode();
            }
        }

        public override string ToString()
        {
            return "{" + X + "; " + Y + "}";
        }
    }
}
