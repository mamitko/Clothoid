using System;
using System.Diagnostics;

namespace Clothoid.Geometry
{
    public struct Circle
    {
        public Point Center { get; }

        public readonly double Radius;

        public Circle(Point center, double radius)
        {
            Center = center;
            Radius = radius;
        }

        public Circle(double centerX, double centerY, double radius) : this(new Point(centerX, centerY), radius)
        {
        }

        public double Length => 2*Math.PI*Math.Abs(Radius);

        public UnitVector GetTangent(Point point)
        {
            return Radius > 0 
                ? (point - Center).Normal 
                : -(point - Center).Normal;
        }

        public Line GetTangentLine(UnitVector radiusAngle)
        {
            var dir = Radius > 0
                ? radiusAngle.Normal
                : -radiusAngle.Normal;
            
            return new Line(Center + radiusAngle * Math.Abs(Radius), dir);
        }

        public double DistanceTo(Point point)
        {
            return (Math.Abs(Radius) - Center.DistanceTo(point))*Math.Sign(Radius);
        }

        public bool Intersect(Line line, out Point pt1, out Point pt2)
        {
            if (line.AbsDistanceTo(Center) > Math.Abs(Radius))
            {
                pt1 = Point.NaP;
                pt2 = Point.NaP;
                return false;
            }
            
            var shift = -new Vector(Center);

            var l = line.Translate(shift);
            
            var r = Radius;
            var r_2 = r*r;
            
            var a = l.A;
            var b = l.B;
            var p = l.P;

            var a_2 = a*a;
            var a_4 = a*a*a*a;
            
            var b_2 = b*b;

            var p_2 = p*p;
            
            var sqrtD = Math.Sqrt(b_2*r_2*a_2 + a_4*r_2 - a_2*p_2);

            Debug.Assert(!double.IsNaN(sqrtD));

            if (a == 0)
            {
                var y = p*b + sqrtD;
                var x = Math.Sqrt(r*r - y*y);

                pt1 = new Point(x, y) - shift;
                pt2 = new Point(-x, y) - shift;
                return true;
            }

            pt1 = new Point(-(b*(p*b + sqrtD) - p)/a, p*b+sqrtD) - shift;
            pt2 = new Point(-(-b*(-p*b + sqrtD) - p)/a, -(-p*b + sqrtD)) - shift;
            
            return true;
        }
    }
}
