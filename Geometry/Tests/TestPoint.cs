using System;
using NUnit.Framework;

namespace ClothoidAndTheOthers.Geometry.Tests
{
    [TestFixture]
    public class TestPoint
    {
        [Test]
        public void TestRotate()
        {
            var pt1 = new Point(1, 1);
            var pt2 = new Point(2, 2);

            var pt1_1 = pt1.Rotate(UnitVector.FromAngle(Math.PI/4), Point.Zero);
            var pt2_1 = pt2.Rotate(UnitVector.FromAngle(Math.PI/4), Point.Zero);

            var a1 = (pt2_1 - pt1_1).Unit.ToAngle();

            var pt1_2 = pt1.Rotate(UnitVector.FromAngle(Math.PI / 8), Point.Zero);
            var pt2_2 = pt2.Rotate(UnitVector.FromAngle(Math.PI / 8), Point.Zero);
            pt2_2 = pt2_2.Rotate(UnitVector.FromAngle(Math.PI / 8), pt1_2);

            var a2 = (pt2_2 - pt1_2).Unit.ToAngle();

            Assert.AreEqual(a1, a2, 1e-12);
        }
        
    }
}