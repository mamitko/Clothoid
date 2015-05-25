using System;

namespace ClothoidAndTheOthers.Geometry
{
    public struct Line
    {
        public readonly double A;
        public readonly double B;
        public readonly double P;
        

        public Line(double a, double b, double p)
        {
            A = a;
            B = b;
            P = p;
        }

        public Line(Point start, Point end)
        {
            if ((end - start).Length < double.Epsilon)
                throw new GeometryException("Точки слишком близко или совпадают");
            
            var normal = (end - start).Normal;
            
            A = normal.X;
            B = normal.Y;
            P = A*start.X + B*start.Y;
        }

        public Line(double x1, double y1, double x2, double y2): this (new Point(x1, y1), new Point(x2, y2))
        {
        }

        public Line(Point point, UnitVector direction)
        {
            A = -direction.Y;
            B = direction.X;
            P = A*point.X + B*point.Y;
        }

        public UnitVector Direction
        {
            get
            {
                return new UnitVector(B, -A);
            }
        }

        public UnitVector Normal
        {
            get { return new UnitVector(A, B); }
        }

        public Line Reversed
        {
            get { return new Line(-A, -B, -P); }
        }
        
        public double DistanceTo(Point point)
        {
            return A*point.X + B*point.Y - P;
        }
        
        public double AbsDistanceTo(Point point)
        {
            return Math.Abs(DistanceTo(point));
        }

        public Line Move(Vector vector)
        {
            return new Line(A, B, P + A * vector.X + B * vector.Y);
        }

        public Line Move(double shift)
        {
            return new Line(A, B, P + shift);
        }

        public Point Intersect(Line other)
        {
            Point result;
            if (!Intersect(other, out result))
                throw new StraightsAreParallelException();

            return result;
        }
        
        public bool Intersect(Line other, out Point point)
        {
            var shift = new Vector(A*P, B*P) + new Vector(1, 1);
            
            var l1 = this.Move(-shift);
            var l2 = other.Move(-shift);

            var divider = (l1.A * l2.B - l1.B * l2.A);

            if (Math.Abs(divider) <= 1e-12)
            {
                point = Point.NaP;
                return false;
            }

            point = new Point(
                -(l1.B * l2.P - l1.P * l2.B) / divider,
                (l1.A * l2.P - l2.A * l1.P) / divider) + shift;
            return true;
        }

        public Point GetProjectction(Point point)
        {
            return Intersect(new Line(point, Normal));
        }
    }
}
