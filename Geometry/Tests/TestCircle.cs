using System;
using NUnit.Framework;

namespace ClothoidAndTheOthers.Geometry.Tests
{
    [TestFixture]
    public class TestCircle
    {
        public const double Tol = 1e-6; // бОльшая точность не всегда получается при аналитическом расчет сеодинений-касаний
        private static readonly double _angleSinTol = Math.Sin(Math.PI / 180 / 60 / 60);

        internal static bool Contains(Circle circle, Point point)
        {
            return Math.Abs(point.DistanceTo(circle.Center) - Math.Abs(circle.Radius)) <= Tol * 50;
        }

        internal static bool Contains(Line line, Point point)
        {
            return Math.Abs(line.DistanceTo(point)) <= Tol * 10;
        }

        internal static bool AreSame(UnitVector u1, UnitVector u2)
        {
            var sin = u1 ^ u2;
            return Math.Abs(sin) < _angleSinTol && u1 * u2 > 0;
        }


        [Test]
        public void TestTangent()
        {
            var u = new Vector(1, -0.5).Unit;

            var c1 = new Circle(10, 20, 50);
            var c2 = new Circle(10 + 100, 20 - 100, 50);

            Assert.IsTrue(AreSame(c1.GetTagent(c1.Center + u * c1.Radius), c2.GetTagent(c2.Center + u * c2.Radius)));

            var c3 = new Circle(10 + 100000000, 20 - 10000000, 50);
            Assert.IsTrue(AreSame(c1.GetTagent(c1.Center + u * c1.Radius), c3.GetTagent(c3.Center + u * c3.Radius)));
        }

        [Test]
        public void TestGetTangentLine()
        {
            var circle = new Circle(10, 20, 10);

            var line = circle.GetTangentLine(new UnitVector(1, 0));
            Assert.AreEqual(circle.Radius, line.DistanceTo(circle.Center));

            line = circle.GetTangentLine(new UnitVector(0, 1));
            Assert.AreEqual(circle.Radius, line.DistanceTo(circle.Center), 1e-12);

            line = circle.GetTangentLine(new UnitVector(-1, 0));
            Assert.AreEqual(circle.Radius, line.DistanceTo(circle.Center), 1e-12);

            line = circle.GetTangentLine(new UnitVector(0, -1));
            Assert.AreEqual(circle.Radius, line.DistanceTo(circle.Center), 1e-12);

            
            circle = new Circle(10, 20, -10);

            line = circle.GetTangentLine(new UnitVector(1, 0));
            Assert.AreEqual(circle.Radius, line.DistanceTo(circle.Center));

            line = circle.GetTangentLine(new UnitVector(0, 1));
            Assert.AreEqual(circle.Radius, line.DistanceTo(circle.Center), 1e-12);

            line = circle.GetTangentLine(new UnitVector(-1, 0));
            Assert.AreEqual(circle.Radius, line.DistanceTo(circle.Center), 1e-12);

            line = circle.GetTangentLine(new UnitVector(0, -1));
            Assert.AreEqual(circle.Radius, line.DistanceTo(circle.Center), 1e-12);
        }

        [Test]
        public void TestIntersect()
        {
            var circle = new Circle(10, 20, 10);
            var line = new Line(new Point(0, 20), new Point(20, 20));

            Point pt1;
            Point pt2;
            Assert.IsTrue(circle.Intersect(line, out pt1, out pt2));
            Assert.That(
                ((new Point(0, 20)).Equals(pt1) && (new Point(20, 20)).Equals(pt2)) || 
                ((new Point(0, 20)).Equals(pt2) && (new Point(20, 20)).Equals(pt1)));

            
            circle = new Circle(10, 20, 10);
            line = new Line(new Point(0, 30), new Point(20, 30));
            
            Assert.IsTrue(circle.Intersect(line, out pt1, out pt2));
            Assert.That(
                (new Point(10, 30)).Equals(pt1) && (new Point(10, 30)).Equals(pt2));

            
            circle = new Circle(10, 20, 10);
            line = new Line(new Point(0, 100), new Point(20, 100));

            Assert.IsFalse(circle.Intersect(line, out pt1, out pt2));

            circle = new Circle(10, 20, 10);
            line = new Line(new Point(0, 100), new Point(20, 110));

            Assert.IsFalse(circle.Intersect(line, out pt1, out pt2));


            circle = new Circle(10, 20, 10);
            line = new Line(new Point(0, 25), new Point(20, 23));

            Assert.IsTrue(circle.Intersect(line, out pt1, out pt2));
            Assert.AreEqual(Math.Abs(circle.Radius), pt1.DistanceTo(circle.Center), 1e-12);
            Assert.AreEqual(Math.Abs(circle.Radius), pt2.DistanceTo(circle.Center), 1e-12);
        }
    }
}
