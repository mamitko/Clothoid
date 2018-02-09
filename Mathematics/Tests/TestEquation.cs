using System;
using NUnit.Framework;

namespace Clothoid.Mathematics.Tests
{
    [TestFixture]
    public class TestEquation
    {
        const double Eps = 1e-12;

        [Test]
        public void TestSolveBisection()
        {
            Equation.Func func = x => -Math.Sqrt(1.5 * 1.5 - (x - 1) * (x - 1)) + 1;
            Assert.IsTrue(Equation.SolveBisection(func, 0.5, 2.3, Eps) > 0);

            func = x => x - 1;
            Assert.AreEqual(1, Equation.SolveBisection(func, 0, 2, Eps), Eps);
            Assert.AreEqual(1, Equation.SolveBisection(func, 1, 2, Eps), Eps);
            Assert.IsNaN(Equation.SolveBisection(func, 3, 4, Eps));

            func = x => x*x - 1;
            Assert.AreEqual(1, Equation.SolveBisection(func, 0, 10, Eps), Eps);
            Assert.AreEqual(-1, Equation.SolveBisection(func, -10, 0, Eps), Eps);
            Assert.AreEqual(-1, Equation.SolveBisection(func, -10, -1 + double.Epsilon, Eps), Eps);
            Assert.AreEqual(-1, Equation.SolveBisection(func, -10, -1, Eps), Eps);
            Assert.IsNaN(Equation.SolveBisection(func, -10, 10, Eps));
        }

        [Test]
        public void TestSolveNewton()
        {
            Equation.Func func = x => x - 1;
            Assert.AreEqual(1, Equation.SolveNewton(func, 10, 0, Eps), Eps);
            Assert.AreEqual(1, Equation.SolveNewton(func, 0, 10, Eps), Eps);
            Assert.IsNaN(Equation.SolveNewton(func, -10, 0, Eps));

            func = x => x*x - 1;
            Assert.AreEqual(1, Equation.SolveNewton(func, 0, 10, Eps), Eps);
            Assert.AreEqual(-1, Equation.SolveNewton(func, -10, 0, Eps), Eps);
            Assert.AreEqual(-1, Equation.SolveNewton(func, -10, -1 + double.Epsilon, Eps), Eps);
            Assert.AreEqual(-1, Equation.SolveNewton(func, -10, -1, Eps), Eps);
            Assert.IsNaN(Equation.SolveNewton(func, -10, 10, Eps));
        }
    }
}
