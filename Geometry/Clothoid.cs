using System;
using System.Collections.Generic;
using System.Diagnostics;
using ClothoidAndTheOthers.Mathematics;

namespace ClothoidAndTheOthers.Geometry
{
    //http://en.wikipedia.org/wiki/Euler_spiral

    public struct Clothoid
    {
        private static readonly double SqrtPi = Math.Sqrt(Math.PI);
        
        private static double GetX(double curvatureRate, double length)
        {
            if (length == 0)
                return 0; // even if curvatureRate is NaN
            
            return FresnelIntegrals.C(length*curvatureRate)/curvatureRate;
        }

        private static double GetY(double curvatureRate, double length)
        {
            if (length == 0)
                return 0; // even if curvatureRate is NaN

            return FresnelIntegrals.S(length * Math.Abs(curvatureRate)) / curvatureRate;
        }

        private static Point GetPoint(double curvatureRate, double length)
        {
            var c = 0.0;
            var s = 0.0;

            if (Math.Abs(length) >= double.Epsilon)
            {
                FresnelIntegrals.Evaluate(length*Math.Abs(curvatureRate), out c, out s);
                c = c/Math.Abs(curvatureRate);
                s = s/curvatureRate;
            }

            return new Point(c, s);
        }

        private static double GetAngle(double curvatureRate, double length)
        {
            if (length == 0)
                return 0; // even if curvatureRate is NaN
            
            return (curvatureRate * length) * (Math.Abs(curvatureRate) * length); 
            // Math.Abs(...) stands instead of "* Sign(...)"
        }

        private static UnitVector GetTangent(double curvatureRate, double length)
        {
            if (length == 0)
                return new UnitVector(1, 0); // even if curvatureRate is NaN

            var angle = GetAngle(curvatureRate, length);

            var x = Math.Cos(angle);
            var y = Math.Sqrt(1 - x*x) * Math.Sign(curvatureRate);

            return new UnitVector(x, y);
        }

        private static UnitVector GetNormal(double curvatureRate, double length)
        {
            return GetTangent(curvatureRate, length).Normal;
        }

        private static double GetCurvatureRate(double length, double radius)
        {
            if (double.IsNaN(length*radius))
                throw new ArgumentException();

            return 1 / Math.Sqrt(2 * Math.Abs(radius) * Math.Abs(length)) * Math.Sign(radius*length);
        }

        private static double GetRadius(double curvatureRate, double length)
        {
            return 1 / (2*length*curvatureRate*curvatureRate) * Math.Sign(curvatureRate);
        }

        private static Point GetCurvatureCenter(double curvatureRate, double length, double radius)
        {
            return GetPoint(curvatureRate, length) + GetNormal(curvatureRate, length)*radius;
        }

        public static Point GetCurvatureCenter(double curvatureRate, double length)
        {
            if (double.IsNaN(curvatureRate))
                throw new ArgumentException("curvatureRate can not be NaN");

            return GetPoint(curvatureRate, length) + GetNormal(curvatureRate, length) * GetRadius(curvatureRate, length);
        }

        private static Point CalcOrigin(double curvatureRate, double endLength, Circle endCircle, UnitVector originDirection)
        {
            var radiusV = (-GetNormal(curvatureRate, endLength)).Rotate(originDirection) * endCircle.Radius;
            var startToEndVector = ((Vector)GetPoint(curvatureRate, endLength)).Rotate(originDirection);

            return endCircle.Center + radiusV - startToEndVector;
        }

        
        public static double GetLength(double curvatureRate, double radius)
        {
            if (double.IsNaN(curvatureRate))
                return 0;
            
            return 1/(2*curvatureRate*curvatureRate*radius)*Math.Sign(curvatureRate);
        }
        
        
        /// <summary>
        /// Curvature rate of Clothoid, equals to 1/Sqrt(2*R*L) at each point of clothoid. 
        /// Negative value means that Radius is negative (clockwise) in positive Length parameter direction.
        /// </summary>
        public readonly double CurvatureRate;

        public readonly Point Origin;
        public readonly UnitVector OriginDirection;
        
        public readonly double StartLength;
        public readonly double EndLength;
        
        private static double CalcLength(double radius, double h, double tol)
        {
            Debug.Assert(radius > 0 && h > 0, "Arguments must be positive");

            Equation.Func func = 
                length =>
                    {
                        var rate = GetCurvatureRate(length, radius);
                        var angle = GetAngle(rate, length);
                        return h - (GetY(rate, length) + radius * Math.Cos(angle));
                    };

            //return Equation.SolveBisectional(func, 0, 2 * Math.PI * radius, tol);
            return Equation.SolveBrent(func, 0, 2 * Math.PI * radius, tol, 0);
        }

        private static double CalcLengthAtSmallCircle(double bigR, double smallR, double d, double tol)
        {
            if (smallR <= 0 || d <= 0)
                return double.NaN;
            
            Equation.Func func =
                smallLength =>
                    {
                        var rate = GetCurvatureRate(smallLength, smallR);
                        var bigLength = GetLength(rate, bigR);

                        return d - (GetCurvatureCenter(rate, bigLength, bigR)
                                        .DistanceTo(GetCurvatureCenter(rate, smallLength, smallR)));
                    };
            
            
            var result = double.NaN;
            // Equation func is periodic, so we are checking each period of two full revolutions of clothoid
            for (var i = 0; i < 4; i++)
            {
                result = Equation.SolveBrent(func, i * 2*Math.PI*smallR, (i+1)*2*Math.PI*smallR, tol, 0);
                if (!double.IsNaN(result))
                    return result;
            }

            return result;
        }

        
        public static bool FitConnectedCircle(Clothoid clothoid, Point point, out Circle circle, out double clothoidLength)
        {
            const double tol = 1e-12;

            circle = new Circle(Point.NaP, double.NaN);
            clothoidLength = double.NaN;

            if (double.IsNaN(clothoid.CurvatureRate))
            {
                return false;
            }

            var newXAxis = new Line(clothoid.Origin, clothoid.OriginDirection);
            var vx = newXAxis.GetProjectction(point) - clothoid.Origin;
            var pt = new Point(vx.Length * Math.Sign(vx * newXAxis.Direction), newXAxis.DistanceTo(point));

            if ((pt.X * pt.Y) * clothoid.CurvatureRate <= 0.0)
            {
                return false;
            }

            Equation.Func func = length => Math.Abs(GetRadius(clothoid.CurvatureRate, length)) -
                                           (GetCurvatureCenter(clothoid.CurvatureRate, length) - pt).Length;

            var sign = Math.Sign(clothoid.CurvatureRate*pt.Y);
            var boundL = SqrtPi/Math.Abs(clothoid.CurvatureRate)*sign;

            clothoidLength = Equation.SolveBisectional(func, 0 + 1e-12 * sign, boundL - 1e-12*sign, tol);
            // Brent method does not work bacause func changes it's 2nd derivative sign

            if (double.IsNaN(clothoidLength))
                return false;

            var r = GetRadius(clothoid.CurvatureRate, clothoidLength);
            var c = clothoid.GetPoint(clothoidLength) + clothoid.GetNormal(clothoidLength) * r;

            circle = new Circle(c, r);
            return true;
        }


        public static double CalcTargetDistance(double radius, double arcLength)
        {
            var rate = GetCurvatureRate(arcLength, radius);
            var y = GetY(rate, arcLength);
            var normal = GetNormal(rate, arcLength);
            return y + normal.Y * radius;
        }

        public static double CalcTargetDistance(double radius1, double radius2, double arcLength)
        {
            if (Math.Abs(radius1 - radius2) < double.Epsilon)
                return double.NaN;

            if (arcLength == 0.0)
                return Math.Abs(radius1 - radius2);

            var l1 = (arcLength*radius2)/(radius1 - radius2);
            var l2 = (arcLength*radius1)/(radius1 - radius2);
            
            var rate = GetCurvatureRate(l1, radius1);

            var center1 = GetPoint(rate, l1) + GetNormal(rate, l1)*radius1;
            var center2 = GetPoint(rate, l2) + GetNormal(rate, l2)*radius2;

            return (center2 - center1).Length;
        }

        /// <summary>
        /// Returnes less then zero if line and circle are intersecting, grater then zero if connecting clothoid is too long (turne angle > Pi)
        /// </summary>
        public static int Connect(Line line, Circle circle, bool fromLineToCircle, double tol, out Clothoid clothoid)
        {
            var h = line.DistanceTo(circle.Center);
            var offset = h - circle.Radius;

            double length;

            if (Math.Abs(offset) > tol)
            {
                if (offset*circle.Radius < 0)
                {
                    clothoid = new Clothoid();
                    return -1;
                }

                length = CalcLength(Math.Abs(circle.Radius), Math.Abs(h), tol);

                if (double.IsNaN(length))
                {
                    clothoid = new Clothoid();
                    return +1;
                }
            }
            else
            {
                length = 0;
            }

            var lengthAtCircle = fromLineToCircle ? length : -length;
            var curvatureRate = GetCurvatureRate(lengthAtCircle, circle.Radius);

            var originDirection = line.Direction;

            var origin = CalcOrigin(curvatureRate, lengthAtCircle, circle, originDirection);

            clothoid = fromLineToCircle
                           ? new Clothoid(curvatureRate, origin, originDirection, 0, lengthAtCircle)
                           : new Clothoid(curvatureRate, origin, originDirection, lengthAtCircle, 0);

            return 0;
        }

        /// <summary>
        /// Returnes less then zero if circles are intersecting, grater then zero if connecting clothoid is too long (turne angle > Pi)
        /// </summary>
        public static int Connect(Circle startCircle, Circle endCircle, double tol, out Clothoid clothoid)
        {
            var d = (startCircle.Center - endCircle.Center).Length;

            var offset = Math.Abs(endCircle.Radius - startCircle.Radius) - d;

            double rate;
            
            if (Math.Abs(offset) > tol)
            {
                if (offset * Math.Sign(endCircle.Radius*startCircle.Radius) < 0)
                {
                    clothoid = new Clothoid();
                    return -1;
                }

                var lengthDirection = Math.Sign(Math.Abs(startCircle.Radius) - Math.Abs(endCircle.Radius));

                var bigCircle = lengthDirection > 0 ? startCircle : endCircle;
                var smallCircle = lengthDirection > 0 ? endCircle : startCircle;

                var reversing = Math.Sign(smallCircle.Radius);

                var smallLength = CalcLengthAtSmallCircle(bigCircle.Radius * reversing, smallCircle.Radius * reversing, d, tol);

                if (double.IsNaN(smallLength))
                {
                    clothoid = new Clothoid();
                    return +1;
                }

                rate = GetCurvatureRate(smallLength, smallCircle.Radius) * lengthDirection;
            }
            else
            {
                rate = GetCurvatureRate(0, endCircle.Radius);
            }
            
            var endLength = GetLength(rate, endCircle.Radius);
            var startLength = GetLength(rate, startCircle.Radius);
            
            var vD = GetCurvatureCenter(rate, endLength, endCircle.Radius) -
                     GetCurvatureCenter(rate, startLength, startCircle.Radius);
            var v = endCircle.Center - startCircle.Center;
            
            var originDirection = UnitVector.GetAngle(vD, v);
            var origin = CalcOrigin(rate, endLength, endCircle, originDirection);

            clothoid = new Clothoid(rate, origin, originDirection, startLength, endLength);

            return 0;
        }

        public Clothoid(double curvaturerate, Point origin, UnitVector originDirection, double startLength, double endLength)
        {
            CurvatureRate = curvaturerate;
            Origin = origin;
            OriginDirection = originDirection;
            StartLength = startLength;
            EndLength = endLength;
        }



        public Point GetPoint(double length)
        {
            return (GetPoint(CurvatureRate, length)).Rotate(OriginDirection) + (Vector)Origin;
        }

        public UnitVector GetTangent(double length)
        {
            return GetTangent(CurvatureRate, length).Rotate(OriginDirection);
        }

        public UnitVector GetNormal(double length)
        {
            return GetNormal(CurvatureRate, length).Rotate(OriginDirection);
        }

        public double GetLength(double radius)
        {
            return GetLength(CurvatureRate, radius);
        }

        public Point StartPoint
        {
            get
            {
                return GetPoint(StartLength);
            }
        }

        public Point EndPoint
        {
            get
            {
                return GetPoint(EndLength);
            }
        }

        public double StartRadius
        {
            get { return GetRadius(CurvatureRate, StartLength); }
        }

        public double EndRadius
        {
            get { return GetRadius(CurvatureRate, EndLength); }
        }


        public double Length
        {
            get
            {
                return EndLength - StartLength;
            }
        }

        public UnitVector EndTangent
        {
            get
            {
                return GetTangent(EndLength);
            }
        }

        public UnitVector StartTangent
        {
            get
            {
                return GetTangent(StartLength);
            }
        }

        public bool Intersect(Line line, out Point point)
        {
            const double tol = 1e-12;
            
            var leftLength = Math.Min(StartLength, EndLength);
            var rightLength = Math.Max(StartLength, EndLength);

            var leftD = line.DistanceTo(GetPoint(leftLength));
            var rightD = line.DistanceTo(GetPoint(rightLength));

            if (leftD*rightD > 0)
            {
                point = Point.NaP;
                return false;
            }

            var prevMiddleLength = double.NaN;
            
            do
            {
                var middleLength = leftLength + (rightLength - leftLength)*Math.Abs(leftD)/(Math.Abs(leftD) + Math.Abs(rightD));
                var middlePoint = GetPoint(middleLength);
                var middleD = line.DistanceTo(middlePoint);

                if (Math.Abs(middleD) <= tol || Math.Abs(middleLength - prevMiddleLength) < tol)
                {
                    point = middlePoint;
                    return true;
                }
                
                if (middleD * rightD < 0)
                {
                    leftLength = middleLength;
                    leftD = middleD;
                }
                else
                    if (middleD * leftD < 0)
                    {
                        rightLength = middleLength;
                        rightD = middleD;
                    }
                    else
                        throw new NumericalMethodException();

                prevMiddleLength = middleLength;

            } while (true);
        }

        public IEnumerable<Point> GetPoints(double step, bool endPointGuaranteed)
        {
            var endL = Math.Max(EndLength, StartLength);

            var l = Math.Min(StartLength, EndLength);

            do
            {
                yield return GetPoint(l);
                
                l = l + step;

            } while (l < endL);

            if (endPointGuaranteed)
                yield return GetPoint(endL);
        }

        #region Equality
        public bool Equals(Clothoid other)
        {
            return other.CurvatureRate.Equals(CurvatureRate) && other.Origin.Equals(Origin) && other.OriginDirection.Equals(OriginDirection) && other.StartLength.Equals(StartLength) && other.EndLength.Equals(EndLength);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != typeof(Clothoid)) return false;
            return Equals((Clothoid)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = CurvatureRate.GetHashCode();
                result = (result * 397) ^ Origin.GetHashCode();
                result = (result * 397) ^ OriginDirection.GetHashCode();
                result = (result * 397) ^ StartLength.GetHashCode();
                result = (result * 397) ^ EndLength.GetHashCode();
                return result;
            }
        } 
        #endregion
    }
}
