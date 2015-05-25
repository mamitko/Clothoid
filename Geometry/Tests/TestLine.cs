using NUnit.Framework;

namespace ClothoidAndTheOthers.Geometry.Tests
{
    [TestFixture]
    public class TestLine
    {
        [Test]
        public void TestDistance()
        {
            var a = new Point(10, 10);
            var b = new Point(20, 15);
            
            var line = new Line(a, b);

            Assert.AreEqual(0, line.DistanceTo(a), 1e-12);
            Assert.AreEqual(0, line.DistanceTo(b), 1e-12);

            Assert.IsTrue( line.DistanceTo( new Point(10, 11) ) > 0 );
            Assert.IsTrue(line.DistanceTo(new Point(10, 9)) < 0);
        }

        [Test]
        public void TestUnitDirection()
        {
            var a = new Point(10, 10);
            var b = new Point(20, 15);

            var line = new Line(a, b);

            var u = (b-a).Unit;

            Assert.AreEqual(1, line.Direction*u, 1e-12);
        }

        [Test]
        public void TestDirctionalCtor()
        {
            var pt = new Point(10, 20);
            var pt2 = new Point(-100, 200);

            var dir = (pt2 - pt).Unit;

            var line = new Line(pt, dir);

            Assert.AreEqual(0, line.DistanceTo(pt), 1e-12);
            Assert.AreEqual(0, line.DistanceTo(pt2), 1e-12);
            Assert.That(line.DistanceTo(new Point(0, 0)) > 0);
        }
    }
}
