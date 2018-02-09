using System;

namespace Clothoid.Geometry
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
            // todo looks like it's not OK to check with such a tiny epsilon
            if ((end - start).Length < double.Epsilon)
                throw new GeometryException("The points are too close, or coincide.");
            
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

        public UnitVector Direction => new UnitVector(B, -A);

        public UnitVector Normal => new UnitVector(A, B);

        public Line Reversed => new Line(-A, -B, -P);

        public double DistanceTo(Point point)
        {
            return A*point.X + B*point.Y - P;
        }
        
        public double AbsDistanceTo(Point point)
        {
            return Math.Abs(DistanceTo(point));
        }

        public Line Translate(Vector vector)
        {
            return new Line(A, B, P + A * vector.X + B * vector.Y);
        }

        public Line Translate(double shift)
        {
            return new Line(A, B, P + shift);
        }

        public bool Intersect(Line other, out Point point)
        {
            var shift = new Vector(A*P, B*P) + new Vector(1, 1);
            var l1 = this.Translate(-shift);
            var l2 = other.Translate(-shift);

            var divider = l1.A * l2.B - l1.B * l2.A;

            if (Math.Abs(divider) <= Point.Eps)
            {
                point = Point.NaP;
                return false;
            }

            point = new Point(
                        x: -(l1.B * l2.P - l1.P * l2.B) / divider + shift.X,
                        y: (l1.A * l2.P - l2.A * l1.P) / divider + shift.Y);

            return true;
        }

        public Point GetProjection(Point point)
        {
            var projectile = new Line(point, Normal);

            Point projection;
            return Intersect(projectile, out projection) ? projection : Point.NaP;
        }
    }
}
