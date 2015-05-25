using System;

namespace ClothoidAndTheOthers.Geometry
{
    public struct Vector
    {
        public static bool AreSequentiallyAntiClockwise(Vector v1, Vector v2, Vector v3)
        {
            var v1_v2 = (v1 ^ v2) > 0 ? 1 : -1;
            var v2_v3 = (v2 ^ v3) > 0 ? 1 : -1;
            var v3_v1 = (v3 ^ v1) > 0 ? 1 : -1;
            
            return v1_v2 + v2_v3 + v3_v1 > 0;
        }

        /// <summary>
        /// Ориентированная площадь параллелограмма
        /// </summary>
        public static double operator ^(Vector v1, Vector v2)
        {
            return v1.X*v2.Y - v1.Y*v2.X;
        }

        public static double operator *(Vector v1, Vector v2)
        {
            return v1.X*v2.X + v1.Y*v2.Y;
        }

        public static Vector operator *(Vector v, double a)
        {
            return new Vector(v.X * a, v.Y * a);
        }

        public static Vector operator /(Vector v, double a)
        {
            return new Vector(v.X / a, v.Y / a);
        }

        public static Vector operator -(Vector v)
        {
            return v.Reversed;
        }

        public static Vector operator +(Vector v1, Vector v2)
        {
            return new Vector(v1.X + v2.X, v1.Y + v2.Y);
        }

        public static Vector operator -(Vector v1, Vector v2)
        {
            return new Vector(v1.X - v2.X, v1.Y - v2.Y);
        }

        public Vector Reversed
        {
            get { return new Vector(-X, -Y);}
        }

        public readonly double X;
        
        public readonly double Y;

        public Vector(double x, double y)
        {
            X = x;
            Y = y;
        }

        public Vector(Point start, Point end)
        {
            X = end.X - start.X;
            Y = end.Y - start.Y;
        }

        public Vector(Point point): this (point.X, point.Y)
        {
        }

        public UnitVector Normal
        {
            get
            {
                return new Vector(-Y, X).Unit;
            }
        }

        public double Length
        {
            get
            {
                return Math.Sqrt(X*X + Y*Y);
            }
        }

        public UnitVector Unit
        {
            get 
            {
                var norm = 1/Length;
                return new UnitVector(X * norm, Y * norm);
            }
        }

        public double Norm
        {
            get
            {
                var x = Math.Abs(X);
                var y = Math.Abs(Y);

                if (x == 0.0)
                    return y;

                if (y == 0.0)
                    return x;

                if (x > y)
                    return x * Math.Sqrt(1.0 + (y / x) * (y / x));

                return y * Math.Sqrt(1.0 + (x / y) * (x / y));
            }
        }

        public double SqrNorm
        {
            get
            {
                double x = Math.Abs(X);
                double y = Math.Abs(Y);

                if (x == 0.0)
                    return y * y;
                if (y == 0.0)
                    return x * x;
                if (x > y)
                    return x * x * (1.0 + (y / x) * (y / x));

                return y * y * (1.0 + (x / y) * (x / y));
            }
        }

        public static Vector Zero
        {
            get { return new Vector(0, 0); }
        }

        public Vector Rotate(UnitVector angle)
        {
            return new Vector( 
                X * angle.X  - Y * angle.Y, 
                X * angle.Y  + Y * angle.X);
        }

        public override string ToString()
        {
            return "{" + X + "; " + Y + "}";
        }
    }
}
