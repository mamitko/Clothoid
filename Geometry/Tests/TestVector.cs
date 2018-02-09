using System;
using NUnit.Framework;

namespace Clothoid.Geometry.Tests
{
    [TestFixture]
    public class TestVector
    {
        private const double Tol = Point.Eps;

        private static void AssertAreEqual(Vector v1, Vector v2)
        {
            Assert.AreEqual(v1.X, v2.X, Tol);
            Assert.AreEqual(v1.Y, v2.Y, Tol);
        }
        
        [Test]
        public void TestAreSequentiallyClockwise()
        {
            Assert.IsFalse(Vector.AreSequentiallyAntiClockwise(new Vector(10, 0), new Vector(1, -10), new Vector(-10, 1)));
            Assert.IsTrue(Vector.AreSequentiallyAntiClockwise(new Vector(-10, 1), new Vector(1, -10), new Vector(10, 0)));
            Assert.IsFalse(Vector.AreSequentiallyAntiClockwise(new Vector(10, 0), new Vector(10, -1), new Vector(0, -10)));
            Assert.IsTrue(Vector.AreSequentiallyAntiClockwise(new Vector(10, 0), new Vector(10, 1), new Vector(0, -10)));
            Assert.IsFalse(Vector.AreSequentiallyAntiClockwise(new Vector(10, 0), new Vector(10, 3), new Vector(10, 1)));
        }

        [Test]
        public void TestGetAngle()
        {
            Assert.AreEqual(new UnitVector(1, 0), UnitVector.GetAngle(new Vector(10, 1), new Vector(10, 1)));

            var u = UnitVector.GetAngle(new Vector(10, 1), new Vector(10, 2));
            Assert.IsTrue(u.X > 0 && u.Y > 0);

            u = UnitVector.GetAngle(new Vector(10, 1), new Vector(-10, 2));
            Assert.IsTrue(u.X < 0 && u.Y > 0);

            u = UnitVector.GetAngle(new Vector(10, 1), new Vector(-10, -2));
            Assert.IsTrue(u.X < 0 && u.Y < 0);

            u = UnitVector.GetAngle(new Vector(10, 1), new Vector(10, -2));
            Assert.IsTrue(u.X > 0 && u.Y < 0);

            u = UnitVector.GetAngle(new Vector(-10, 1), new Vector(-10, -1));
            Assert.IsTrue(u.X > 0 && u.Y > 0);
        }

        [Test]
        public void TestFromAngle()
        {
            AssertAreEqual(new UnitVector(1, 0), UnitVector.FromAngle(0));
            AssertAreEqual(new UnitVector(0, 1), UnitVector.FromAngle(Math.PI/2));
            AssertAreEqual(new UnitVector(-1, 0), UnitVector.FromAngle(Math.PI));
            AssertAreEqual(new UnitVector(0, -1), UnitVector.FromAngle((Math.PI / 2) * 3));
            AssertAreEqual(new UnitVector(1, 0), UnitVector.FromAngle(Math.PI * 2));
            AssertAreEqual(new UnitVector(0, 1), UnitVector.FromAngle(Math.PI * 2 + Math.PI/2));
            
            AssertAreEqual(new UnitVector(0, -1), UnitVector.FromAngle(-Math.PI / 2));
            AssertAreEqual(new UnitVector(-1, 0), UnitVector.FromAngle(-Math.PI));
            AssertAreEqual(new UnitVector(0, 1), UnitVector.FromAngle(-(Math.PI / 2) * 3));
            AssertAreEqual(new UnitVector(1, 0), UnitVector.FromAngle(-Math.PI * 2));
            AssertAreEqual(new UnitVector(0, -1), UnitVector.FromAngle(-Math.PI * 2 - Math.PI / 2));
        }

        [Test]
        public void TestSignedAngle()
        {
            var u = (new Vector(1, 0.5)).Unit;
            var a = u.ToAngle();
            AssertAreEqual(u, UnitVector.FromAngle(a));

            u = (new Vector(1, -0.5)).Unit;
            a = u.ToAngle();
            AssertAreEqual(u, UnitVector.FromAngle(a));

            u = (new Vector(-1, 0.5)).Unit;
            a = u.ToAngle();
            AssertAreEqual(u, UnitVector.FromAngle(a));

            u = (new Vector(-1, -0.5)).Unit;
            a = u.ToAngle();
            AssertAreEqual(u, UnitVector.FromAngle(a));
        }
    }
}
