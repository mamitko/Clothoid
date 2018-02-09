using System;
using System.Diagnostics;

namespace Clothoid.Mathematics
{
    internal static class Equation
    {
        public delegate double Func(double argument);

        /// <summary>
        /// Finds root of equation "func(x) = 0"
        /// </summary>
        internal static double SolveSimply(Func func, double start, double startStep, double argumentAccuracy)
        {
            var step = startStep;

            var arg = start;
            var value = Math.Abs(func(arg));

            while (step > argumentAccuracy)
            {
                var newValue = Math.Abs(func(arg + step));
                if (newValue < value)
                {
                    arg = arg + step;
                    value = newValue;
                }
                else
                {
                    newValue = Math.Abs(func(arg - step));
                    if (newValue < value)
                    {
                        arg = arg - step;
                        value = newValue;
                    }
                    else
                    {
                        step = step / 2;
                    }
                }
            }

            return arg;
        }
        
        internal static double SolveBisection(Func func, double rangeBound1, double rangeBound2, double funcValueAccuracy)
        {
            var l = Math.Min(rangeBound1, rangeBound2);
            var r = Math.Max(rangeBound1, rangeBound2);

            var lValue = func(l);
            var rValue = func(r);

            if (double.IsNaN(lValue) || double.IsNaN(rValue))
                throw new ArgumentException("Function must be defined (must not be NaN) at range bounds");

            if (rValue*lValue > 0)
                return double.NaN;

            if (lValue == 0)
                return l;

            if (rValue == 0)
                return r;

            double? mValueBefore = null;

            do
            {
                var m = (r + l)/2;
                var mValue = func(m);

                if (double.IsNaN(mValue))
                    throw new ArgumentException($"Function must be defined at all range (is NaN at {m})");

                if (Math.Abs(mValue) <= funcValueAccuracy)
                    return m;
                
                if (mValueBefore != null && mValue == mValueBefore.Value)
                    // Func calculation precision has been exceeded
                    return m;

                mValueBefore = mValue;

                if (lValue*mValue <= 0)
                {
                    r = m;
                    rValue = mValue;
                }

                if (rValue*mValue <= 0)
                {
                    l = m;
                    lValue = mValue;
                }

            } while (l != r);

            Debug.Fail("Root has not been found");

            return double.NaN;
        }

        public static double SolveNewton(Func func, double searchStartBound, double otherBound, double funcValueAccuracy)
        {
            const double StartRelativeDelta = 1e-6;

            if (funcValueAccuracy <= 0)
                throw new ArgumentException("funcValueAccuracy must be greater then zero");
            
            var startValue = func(searchStartBound);
            var otherBoundValue = func(otherBound);

            if (double.IsNaN(startValue) || double.IsNaN(otherBoundValue))
                throw new ArgumentException("Function must be defined (must not be NaN) at range bounds");

            if (startValue * otherBoundValue > 0)
                return double.NaN;

            if (Math.Abs(startValue) <= funcValueAccuracy)
                return searchStartBound;

            if (Math.Abs(otherBoundValue) <= funcValueAccuracy)
                return otherBound;

            var delta = StartRelativeDelta * Math.Sign(otherBound - searchStartBound);
            
            var x = searchStartBound;
            var value = startValue;

            while (true)
            {
                if (Math.Abs(value) <= funcValueAccuracy)
                    return x; 
                
                var offsetValue = func(x + delta);
                
                //todo check if offsetValue equals the value

                while (offsetValue*value < 0)
                {
                    if (delta == 0)
                        return x;

                    var chordZeroX = -(x * offsetValue - value * (x + delta)) / (-offsetValue + value);

                    delta = (chordZeroX - x) /2;
                    
                    offsetValue = func(x + delta);
                }

                var tangentZeroX = -(x * offsetValue - value * (x + delta)) / (-offsetValue + value);
                x = tangentZeroX;
                value = func(x);
            }
        }

        public static double SolveBrent(Func g, double left, double right, double tolerance, double target)
        {
            //http://deadline.3x.ro/brent_method.html
            //http://www.codeproject.com/KB/recipes/RootFinding.aspx?msg=3470182&display=Mobile
            
            const int MaxIterations = 50;

            if (tolerance <= 0.0)
            {
                string msg = $"Tolerance must be positive. Received {tolerance}.";
                throw new ArgumentOutOfRangeException(msg);
            }

            // Standardize the problem.  To solve g(x) = target,
            // solve f(x) = 0 where f(x) = g(x) - target.
            Func f = delegate(double x) { return g(x) - target; };

            // Implementation and notation based on Chapter 4 in
            // "Algorithms for Minimization without Derivatives"
            // by Richard Brent.

            double c, d, e, fa, fb, fc, tol, m, p, q, r, s;

            // set up aliases to match Brent's notation
            double a = left; double b = right; double t = tolerance;
            var iterationsUsed = 0;

            fa = f(a);
            fb = f(b);

            if (fa * fb > 0.0)
            {
                return double.NaN;
            }

        label_int:
            c = a; fc = fa; d = e = b - a;
        label_ext:
            if (Math.Abs(fc) < Math.Abs(fb))
            {
                a = b; b = c; c = a;
                fa = fb; fb = fc; fc = fa;
            }

            iterationsUsed++;

            tol = 2.0 * t * Math.Abs(b) + t;
            m = 0.5 * (c - b);
            if (Math.Abs(m) > tol && fb != 0.0) // exact comparison with 0 is OK here
            {
                // See if bisection is forced
                if (Math.Abs(e) < tol || Math.Abs(fa) <= Math.Abs(fb))
                {
                    d = e = m;
                }
                else
                {
                    s = fb / fa;
                    if (a == c)
                    {
                        // linear interpolation
                        p = 2.0 * m * s; q = 1.0 - s;
                    }
                    else
                    {
                        // Inverse quadratic interpolation
                        q = fa / fc; r = fb / fc;
                        p = s * (2.0 * m * q * (q - r) - (b - a) * (r - 1.0));
                        q = (q - 1.0) * (r - 1.0) * (s - 1.0);
                    }
                    if (p > 0.0)
                        q = -q;
                    else
                        p = -p;
                    s = e; e = d;
                    if (2.0 * p < 3.0 * m * q - Math.Abs(tol * q) && p < Math.Abs(0.5 * s * q))
                        d = p / q;
                    else
                        d = e = m;
                }
                a = b; fa = fb;
                if (Math.Abs(d) > tol)
                    b += d;
                else if (m > 0.0)
                    b += tol;
                else
                    b -= tol;
                if (iterationsUsed == MaxIterations)
                    return b;

                fb = f(b);
                if ((fb > 0.0 && fc > 0.0) || (fb <= 0.0 && fc <= 0.0))
                    goto label_int;
                else
                    goto label_ext;
            }
            else
                return b;
        }
    }
}