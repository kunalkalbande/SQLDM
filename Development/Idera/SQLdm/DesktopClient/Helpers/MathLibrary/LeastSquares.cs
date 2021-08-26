/*************************************************************************
Copyright (c) 2006-2007, Sergey Bochkanov (ALGLIB project).

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are
met:

- Redistributions of source code must retain the above copyright
  notice, this list of conditions and the following disclaimer.

- Redistributions in binary form must reproduce the above copyright
  notice, this list of conditions and the following disclaimer listed
  in this license in the documentation and/or other materials
  provided with the distribution.

- Neither the name of the copyright holders nor the names of its
  contributors may be used to endorse or promote products derived from
  this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
"AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*************************************************************************/

using System;

namespace Idera.SQLdm.DesktopClient.Helpers.MathLibrary
{
    public class LeastSquares
    {
        /*************************************************************************
        Weighted approximation by arbitrary function basis in a space of arbitrary
        dimension using linear least squares method.

        Input parameters:
            Y   -   array[0..N-1]
                    It contains a set  of  function  values  in  N  points.  Space
                    dimension  and  points  don't  matter.  Procedure  works  with
                    function values in these points and values of basis  functions
                    only.

            W   -   array[0..N-1]
                    It contains weights corresponding  to  function  values.  Each
                    summand in square sum of approximation deviations  from  given
                    values is multiplied by the square of corresponding weight.

            FMatrix-a table of basis functions values, array[0..N-1, 0..M-1].
                    FMatrix[I, J] - value of J-th basis function in I-th point.

            N   -   number of points used. N>=1.
            M   -   number of basis functions, M>=1.

        Output parameters:
            C   -   decomposition coefficients.
                    Array of real numbers whose index goes from 0 to M-1.
                    C[j] - j-th basis function coefficient.

          -- ALGLIB --
             Copyright by Bochkanov Sergey
        *************************************************************************/
        public static void BuildGeneralLeastSquares(ref double[] y,
            ref double[] w,
            ref double[,] fmatrix,
            int n,
            int m,
            ref double[] c)
        {
            int i = 0;
            int j = 0;
            double[,] a = new double[0, 0];
            double[,] q = new double[0, 0];
            double[,] vt = new double[0, 0];
            double[] b = new double[0];
            double[] tau = new double[0];
            double[,] b2 = new double[0, 0];
            double[] tauq = new double[0];
            double[] taup = new double[0];
            double[] d = new double[0];
            double[] e = new double[0];
            bool isuppera = new bool();
            int mi = 0;
            int ni = 0;
            double v = 0;
            int i_ = 0;
            int i1_ = 0;

            mi = n;
            ni = m;
            c = new double[ni - 1 + 1];

            //
            // Initialize design matrix.
            // Here we are making MI>=NI.
            //
            a = new double[ni + 1, Math.Max(mi, ni) + 1];
            b = new double[Math.Max(mi, ni) + 1];
            for (i = 1; i <= mi; i++)
            {
                b[i] = w[i - 1] * y[i - 1];
            }
            for (i = mi + 1; i <= ni; i++)
            {
                b[i] = 0;
            }
            for (j = 1; j <= ni; j++)
            {
                i1_ = (0) - (1);
                for (i_ = 1; i_ <= mi; i_++)
                {
                    a[j, i_] = fmatrix[i_ + i1_, j - 1];
                }
            }
            for (j = 1; j <= ni; j++)
            {
                for (i = mi + 1; i <= ni; i++)
                {
                    a[j, i] = 0;
                }
            }
            for (j = 1; j <= ni; j++)
            {
                for (i = 1; i <= mi; i++)
                {
                    a[j, i] = a[j, i] * w[i - 1];
                }
            }
            mi = Math.Max(mi, ni);

            //
            // LQ-decomposition of A'
            // B2 := Q*B
            //
            LQ.LQDecomposition(ref a, ni, mi, ref tau);
            LQ.UnpackqFromLQ(ref a, ni, mi, ref tau, ni, ref q);
            b2 = new double[1 + 1, ni + 1];
            for (j = 1; j <= ni; j++)
            {
                b2[1, j] = 0;
            }
            for (i = 1; i <= ni; i++)
            {
                v = 0.0;
                for (i_ = 1; i_ <= mi; i_++)
                {
                    v += b[i_] * q[i, i_];
                }
                b2[1, i] = v;
            }

            //
            // Back from A' to A
            // Making cols(A)=rows(A)
            //
            for (i = 1; i <= ni - 1; i++)
            {
                for (i_ = i + 1; i_ <= ni; i_++)
                {
                    a[i, i_] = a[i_, i];
                }
            }
            for (i = 2; i <= ni; i++)
            {
                for (j = 1; j <= i - 1; j++)
                {
                    a[i, j] = 0;
                }
            }

            //
            // Bidiagonal decomposition of A
            // A = Q * d2 * P'
            // B2 := (Q'*B2')'
            //
            BiDiagonal.ToBidiagonal(ref a, ni, ni, ref tauq, ref taup);
            BiDiagonal.MultiplyByqFromBidiagonal(ref a, ni, ni, ref tauq, ref b2, 1, ni, true, false);
            BiDiagonal.UnpackptFromBidiagonal(ref a, ni, ni, ref taup, ni, ref vt);
            BiDiagonal.UnpackDiagonalsFromBidiagonal(ref a, ni, ni, ref isuppera, ref d, ref e);

            //
            // Singular value decomposition of A
            // A = U * d * V'
            // B2 := (U'*B2')'
            //
            if (!BiDiagonalSvd.BiDiagonalSvdDecomposition(ref d, e, ni, isuppera, false, ref b2, 1, ref q, 0, ref vt, ni))
            {
                for (i = 0; i <= ni - 1; i++)
                {
                    c[i] = 0;
                }
                return;
            }

            //
            // B2 := (d^(-1) * B2')'
            //
            if (d[1] != 0)
            {
                for (i = 1; i <= ni; i++)
                {
                    if (d[i] > ComplexMath.MachineEpsilon * 10 * Math.Sqrt(ni) * d[1])
                    {
                        b2[1, i] = b2[1, i] / d[i];
                    }
                    else
                    {
                        b2[1, i] = 0;
                    }
                }
            }

            //
            // B := (V * B2')'
            //
            for (i = 1; i <= ni; i++)
            {
                b[i] = 0;
            }
            for (i = 1; i <= ni; i++)
            {
                v = b2[1, i];
                for (i_ = 1; i_ <= ni; i_++)
                {
                    b[i_] = b[i_] + v * vt[i, i_];
                }
            }

            //
            // Out
            //
            for (i = 1; i <= ni; i++)
            {
                c[i - 1] = b[i];
            }
        }


        /*************************************************************************
        Linear approximation using least squares method

        The subroutine calculates coefficients of  the  line  approximating  given
        function.

        Input parameters:
            X   -   array[0..N-1], it contains a set of abscissas.
            Y   -   array[0..N-1], function values.
            N   -   number of points, N>=1

        Output parameters:
            a, b-   coefficients of linear approximation a+b*t

          -- ALGLIB --
             Copyright by Bochkanov Sergey
        *************************************************************************/
        public static void BuildLinearLeastSquares(ref double[] x,
            ref double[] y,
            int n,
            ref double a,
            ref double b)
        {
            double pp = 0;
            double qq = 0;
            double pq = 0;
            double b1 = 0;
            double b2 = 0;
            double d1 = 0;
            double d2 = 0;
            double t1 = 0;
            double t2 = 0;
            double phi = 0;
            double c = 0;
            double s = 0;
            double m = 0;
            int i = 0;

            pp = n;
            qq = 0;
            pq = 0;
            b1 = 0;
            b2 = 0;
            for (i = 0; i <= n - 1; i++)
            {
                pq = pq + x[i];
                qq = qq + ComplexMath.Sqr(x[i]);
                b1 = b1 + y[i];
                b2 = b2 + x[i] * y[i];
            }
            phi = Math.Atan2(2 * pq, qq - pp) / 2;
            c = Math.Cos(phi);
            s = Math.Sin(phi);
            d1 = ComplexMath.Sqr(c) * pp + ComplexMath.Sqr(s) * qq - 2 * s * c * pq;
            d2 = ComplexMath.Sqr(s) * pp + ComplexMath.Sqr(c) * qq + 2 * s * c * pq;
            if (Math.Abs(d1) > Math.Abs(d2))
            {
                m = Math.Abs(d1);
            }
            else
            {
                m = Math.Abs(d2);
            }
            t1 = c * b1 - s * b2;
            t2 = s * b1 + c * b2;
            if (Math.Abs(d1) > m * ComplexMath.MachineEpsilon * 1000)
            {
                t1 = t1 / d1;
            }
            else
            {
                t1 = 0;
            }
            if (Math.Abs(d2) > m * ComplexMath.MachineEpsilon * 1000)
            {
                t2 = t2 / d2;
            }
            else
            {
                t2 = 0;
            }
            a = c * t1 + s * t2;
            b = -(s * t1) + c * t2;
        }


        /*************************************************************************
        Weighted cubic spline approximation using linear least squares

        Input parameters:
            X   -   array[0..N-1], abscissas
            Y   -   array[0..N-1], function values
            W   -   array[0..N-1], weights.
            A, B-   interval to build splines in.
            N   -   number of points used. N>=1.
            M   -   number of basic splines, M>=2.

        Output parameters:
            CTbl-   coefficients table to be used by SplineInterpolation function.
          -- ALGLIB --
             Copyright by Bochkanov Sergey
        *************************************************************************/
        public static void BuildSplineLeastSquares(ref double[] x,
            ref double[] y,
            ref double[] w,
            double a,
            double b,
            int n,
            int m,
            ref double[] ctbl)
        {
            int i = 0;
            int j = 0;
            double[,] ma = new double[0, 0];
            double[,] q = new double[0, 0];
            double[,] vt = new double[0, 0];
            double[] mb = new double[0];
            double[] tau = new double[0];
            double[,] b2 = new double[0, 0];
            double[] tauq = new double[0];
            double[] taup = new double[0];
            double[] d = new double[0];
            double[] e = new double[0];
            bool isuppera = new bool();
            int mi = 0;
            int ni = 0;
            double v = 0;
            double[] sx = new double[0];
            double[] sy = new double[0];
            int i_ = 0;

            System.Diagnostics.Debug.Assert(m >= 2, "BuildSplineLeastSquares: M is too small!");
            mi = n;
            ni = m;
            sx = new double[ni - 1 + 1];
            sy = new double[ni - 1 + 1];

            //
            // Initializing design matrix
            // Here we are making MI>=NI
            //
            ma = new double[ni + 1, Math.Max(mi, ni) + 1];
            mb = new double[Math.Max(mi, ni) + 1];
            for (j = 0; j <= ni - 1; j++)
            {
                sx[j] = a + (b - a) * j / (ni - 1);
            }
            for (j = 0; j <= ni - 1; j++)
            {
                for (i = 0; i <= ni - 1; i++)
                {
                    sy[i] = 0;
                }
                sy[j] = 1;
                Spline3.BuildCubicSpline(sx, sy, ni, 0, 0.0, 0, 0.0, ref ctbl);
                for (i = 0; i <= mi - 1; i++)
                {
                    ma[j + 1, i + 1] = w[i] * Spline3.SplineInterpolation(ref ctbl, x[i]);
                }
            }
            for (j = 1; j <= ni; j++)
            {
                for (i = mi + 1; i <= ni; i++)
                {
                    ma[j, i] = 0;
                }
            }

            //
            // Initializing right part
            //
            for (i = 0; i <= mi - 1; i++)
            {
                mb[i + 1] = w[i] * y[i];
            }
            for (i = mi + 1; i <= ni; i++)
            {
                mb[i] = 0;
            }
            mi = Math.Max(mi, ni);

            //
            // LQ-decomposition of A'
            // B2 := Q*B
            //
            LQ.LQDecomposition(ref ma, ni, mi, ref tau);
            LQ.UnpackqFromLQ(ref ma, ni, mi, ref tau, ni, ref q);
            b2 = new double[1 + 1, ni + 1];
            for (j = 1; j <= ni; j++)
            {
                b2[1, j] = 0;
            }
            for (i = 1; i <= ni; i++)
            {
                v = 0.0;
                for (i_ = 1; i_ <= mi; i_++)
                {
                    v += mb[i_] * q[i, i_];
                }
                b2[1, i] = v;
            }

            //
            // Back from A' to A
            // Making cols(A)=rows(A)
            //
            for (i = 1; i <= ni - 1; i++)
            {
                for (i_ = i + 1; i_ <= ni; i_++)
                {
                    ma[i, i_] = ma[i_, i];
                }
            }
            for (i = 2; i <= ni; i++)
            {
                for (j = 1; j <= i - 1; j++)
                {
                    ma[i, j] = 0;
                }
            }

            //
            // Bidiagonal decomposition of A
            // A = Q * d2 * P'
            // B2 := (Q'*B2')'
            //
            BiDiagonal.ToBidiagonal(ref ma, ni, ni, ref tauq, ref taup);
            BiDiagonal.MultiplyByqFromBidiagonal(ref ma, ni, ni, ref tauq, ref b2, 1, ni, true, false);
            BiDiagonal.UnpackptFromBidiagonal(ref ma, ni, ni, ref taup, ni, ref vt);
            BiDiagonal.UnpackDiagonalsFromBidiagonal(ref ma, ni, ni, ref isuppera, ref d, ref e);

            //
            // Singular value decomposition of A
            // A = U * d * V'
            // B2 := (U'*B2')'
            //
            if (!BiDiagonalSvd.BiDiagonalSvdDecomposition(ref d, e, ni, isuppera, false, ref b2, 1, ref q, 0, ref vt, ni))
            {
                for (i = 1; i <= ni; i++)
                {
                    d[i] = 0;
                    b2[1, i] = 0;
                    for (j = 1; j <= ni; j++)
                    {
                        if (i == j)
                        {
                            vt[i, j] = 1;
                        }
                        else
                        {
                            vt[i, j] = 0;
                        }
                    }
                }
                b2[1, 1] = 1;
            }

            //
            // B2 := (d^(-1) * B2')'
            //
            for (i = 1; i <= ni; i++)
            {
                if (d[i] > ComplexMath.MachineEpsilon * 10 * Math.Sqrt(ni) * d[1])
                {
                    b2[1, i] = b2[1, i] / d[i];
                }
                else
                {
                    b2[1, i] = 0;
                }
            }

            //
            // B := (V * B2')'
            //
            for (i = 1; i <= ni; i++)
            {
                mb[i] = 0;
            }
            for (i = 1; i <= ni; i++)
            {
                v = b2[1, i];
                for (i_ = 1; i_ <= ni; i_++)
                {
                    mb[i_] = mb[i_] + v * vt[i, i_];
                }
            }

            //
            // Forming result spline
            //
            for (i = 0; i <= ni - 1; i++)
            {
                sy[i] = mb[i + 1];
            }
            Spline3.BuildCubicSpline(sx, sy, ni, 0, 0.0, 0, 0.0, ref ctbl);
        }


        /*************************************************************************
        Polynomial approximation using least squares method

        The subroutine calculates coefficients  of  the  polynomial  approximating
        given function. It is recommended to use this function only if you need to
        obtain coefficients of approximation polynomial. If you have to build  and
        calculate polynomial approximation (NOT coefficients), it's better to  use
        BuildChebyshevLeastSquares      subroutine     in     combination     with
        CalculateChebyshevLeastSquares   subroutine.   The   result  of  Chebyshev
        polynomial approximation is equivalent to the result obtained using powers
        of X, but has higher  accuracy  due  to  better  numerical  properties  of
        Chebyshev polynomials.

        Input parameters:
            xValues             -   array[0..N-1], abscissas
            yValues             -   array[0..N-1], function values
            numberOfDataPoints  -   number of points, N>=1
            orderPolynomial     -   order of polynomial required, M>=0

        Output parameters:
            coefficients    -   approximating polynomial coefficients, array[0..M],
                    C[i]    - coefficient at X^i.

          -- ALGLIB --
             Copyright by Bochkanov Sergey
        *************************************************************************/
        public static void BuildPolynomialLeastSquares(ref double[] xValues,
            ref double[] yValues,
            int numberOfDataPoints,
            int orderPolynomial,
            ref double[] coefficients)
        {
            double[] ctbl = new double[0];
            double[] w = new double[0];
            double[] c1 = new double[0];
            double maxx = 0;
            double minx = 0;
            int i = 0;
            int j = 0;
            int k = 0;
            double e = 0;
            double d = 0;
            double l1 = 0;
            double l2 = 0;
            double[] z2 = new double[0];
            double[] z1 = new double[0];


            //
            // Initialize
            //
            maxx = xValues[0];
            minx = xValues[0];
            for (i = 1; i <= numberOfDataPoints - 1; i++)
            {
                if (xValues[i] > maxx)
                {
                    maxx = xValues[i];
                }
                if (xValues[i] < minx)
                {
                    minx = xValues[i];
                }
            }
            if (minx == maxx)
            {
                minx = minx - 0.5;
                maxx = maxx + 0.5;
            }
            w = new double[numberOfDataPoints - 1 + 1];
            for (i = 0; i <= numberOfDataPoints - 1; i++)
            {
                w[i] = 1;
            }

            //
            // Build Chebyshev approximation
            //
            BuildChebyshevLeastSquares(ref xValues, ref yValues, ref w, minx, maxx, numberOfDataPoints, orderPolynomial, ref ctbl);

            //
            // From Chebyshev to powers of X
            //
            c1 = new double[orderPolynomial + 1];
            for (i = 0; i <= orderPolynomial; i++)
            {
                c1[i] = 0;
            }
            d = 0;
            for (i = 0; i <= orderPolynomial; i++)
            {
                for (k = i; k <= orderPolynomial; k++)
                {
                    e = c1[k];
                    c1[k] = 0;
                    if (i <= 1 & k == i)
                    {
                        c1[k] = 1;
                    }
                    else
                    {
                        if (i != 0)
                        {
                            c1[k] = 2 * d;
                        }
                        if (k > i + 1)
                        {
                            c1[k] = c1[k] - c1[k - 2];
                        }
                    }
                    d = e;
                }
                d = c1[i];
                e = 0;
                k = i;
                while (k <= orderPolynomial)
                {
                    e = e + c1[k] * ctbl[k];
                    k = k + 2;
                }
                c1[i] = e;
            }

            //
            // Linear translation
            //
            l1 = 2 / (ctbl[orderPolynomial + 2] - ctbl[orderPolynomial + 1]);
            l2 = -(2 * ctbl[orderPolynomial + 1] / (ctbl[orderPolynomial + 2] - ctbl[orderPolynomial + 1])) - 1;
            coefficients = new double[orderPolynomial + 1];
            z2 = new double[orderPolynomial + 1];
            z1 = new double[orderPolynomial + 1];
            coefficients[0] = c1[0];
            z1[0] = 1;
            z2[0] = 1;
            for (i = 1; i <= orderPolynomial; i++)
            {
                z2[i] = 1;
                z1[i] = l2 * z1[i - 1];
                coefficients[0] = coefficients[0] + c1[i] * z1[i];
            }
            for (j = 1; j <= orderPolynomial; j++)
            {
                z2[0] = l1 * z2[0];
                coefficients[j] = c1[j] * z2[0];
                for (i = j + 1; i <= orderPolynomial; i++)
                {
                    k = i - j;
                    z2[k] = l1 * z2[k] + z2[k - 1];
                    coefficients[j] = coefficients[j] + c1[i] * z2[k] * z1[k];
                }
            }
        }


        /*************************************************************************
        Chebyshev polynomial approximation using least squares method.

        The algorithm reduces interval [A, B] to the interval [-1,1], then  builds
        least squares approximation using Chebyshev polynomials.

        Input parameters:
            X   -   array[0..N-1], abscissas
            Y   -   array[0..N-1], function values
            W   -   array[0..N-1], weights
            A, B-   interval to build approximating polynomials in.
            N   -   number of points used. N>=1.
            M   -   order of polynomial, M>=0. This parameter is passed into
                    CalculateChebyshevLeastSquares function.

        Output parameters:
            CTbl - coefficient table. This parameter is passed into
                    CalculateChebyshevLeastSquares function.
          -- ALGLIB --
             Copyright by Bochkanov Sergey
        *************************************************************************/
        public static void BuildChebyshevLeastSquares(ref double[] x,
            ref double[] y,
            ref double[] w,
            double a,
            double b,
            int n,
            int m,
            ref double[] ctbl)
        {
            int i = 0;
            int j = 0;
            double[,] ma = new double[0, 0];
            double[,] q = new double[0, 0];
            double[,] vt = new double[0, 0];
            double[] mb = new double[0];
            double[] tau = new double[0];
            double[,] b2 = new double[0, 0];
            double[] tauq = new double[0];
            double[] taup = new double[0];
            double[] d = new double[0];
            double[] e = new double[0];
            bool isuppera = new bool();
            int mi = 0;
            int ni = 0;
            double v = 0;
            int i_ = 0;

            mi = n;
            ni = m + 1;

            //
            // Initializing design matrix
            // Here we are making MI>=NI
            //
            ma = new double[ni + 1, Math.Max(mi, ni) + 1];
            mb = new double[Math.Max(mi, ni) + 1];
            for (j = 1; j <= ni; j++)
            {
                for (i = 1; i <= mi; i++)
                {
                    v = 2 * (x[i - 1] - a) / (b - a) - 1;
                    if (j == 1)
                    {
                        ma[j, i] = 1.0;
                    }
                    if (j == 2)
                    {
                        ma[j, i] = v;
                    }
                    if (j > 2)
                    {
                        ma[j, i] = 2.0 * v * ma[j - 1, i] - ma[j - 2, i];
                    }
                }
            }
            for (j = 1; j <= ni; j++)
            {
                for (i = 1; i <= mi; i++)
                {
                    ma[j, i] = w[i - 1] * ma[j, i];
                }
            }
            for (j = 1; j <= ni; j++)
            {
                for (i = mi + 1; i <= ni; i++)
                {
                    ma[j, i] = 0;
                }
            }

            //
            // Initializing right part
            //
            for (i = 0; i <= mi - 1; i++)
            {
                mb[i + 1] = w[i] * y[i];
            }
            for (i = mi + 1; i <= ni; i++)
            {
                mb[i] = 0;
            }
            mi = Math.Max(mi, ni);

            //
            // LQ-decomposition of A'
            // B2 := Q*B
            //
            LQ.LQDecomposition(ref ma, ni, mi, ref tau);
            LQ.UnpackqFromLQ(ref ma, ni, mi, ref tau, ni, ref q);
            b2 = new double[1 + 1, ni + 1];
            for (j = 1; j <= ni; j++)
            {
                b2[1, j] = 0;
            }
            for (i = 1; i <= ni; i++)
            {
                v = 0.0;
                for (i_ = 1; i_ <= mi; i_++)
                {
                    v += mb[i_] * q[i, i_];
                }
                b2[1, i] = v;
            }

            //
            // Back from A' to A
            // Making cols(A)=rows(A)
            //
            for (i = 1; i <= ni - 1; i++)
            {
                for (i_ = i + 1; i_ <= ni; i_++)
                {
                    ma[i, i_] = ma[i_, i];
                }
            }
            for (i = 2; i <= ni; i++)
            {
                for (j = 1; j <= i - 1; j++)
                {
                    ma[i, j] = 0;
                }
            }

            //
            // Bidiagonal decomposition of A
            // A = Q * d2 * P'
            // B2 := (Q'*B2')'
            //
            BiDiagonal.ToBidiagonal(ref ma, ni, ni, ref tauq, ref taup);
            BiDiagonal.MultiplyByqFromBidiagonal(ref ma, ni, ni, ref tauq, ref b2, 1, ni, true, false);
            BiDiagonal.UnpackptFromBidiagonal(ref ma, ni, ni, ref taup, ni, ref vt);
            BiDiagonal.UnpackDiagonalsFromBidiagonal(ref ma, ni, ni, ref isuppera, ref d, ref e);

            //
            // Singular value decomposition of A
            // A = U * d * V'
            // B2 := (U'*B2')'
            //
            if (!BiDiagonalSvd.BiDiagonalSvdDecomposition(ref d, e, ni, isuppera, false, ref b2, 1, ref q, 0, ref vt, ni))
            {
                for (i = 1; i <= ni; i++)
                {
                    d[i] = 0;
                    b2[1, i] = 0;
                    for (j = 1; j <= ni; j++)
                    {
                        if (i == j)
                        {
                            vt[i, j] = 1;
                        }
                        else
                        {
                            vt[i, j] = 0;
                        }
                    }
                }
                b2[1, 1] = 1;
            }

            //
            // B2 := (d^(-1) * B2')'
            //
            for (i = 1; i <= ni; i++)
            {
                if (d[i] > ComplexMath.MachineEpsilon * 10 * Math.Sqrt(ni) * d[1])
                {
                    b2[1, i] = b2[1, i] / d[i];
                }
                else
                {
                    b2[1, i] = 0;
                }
            }

            //
            // B := (V * B2')'
            //
            for (i = 1; i <= ni; i++)
            {
                mb[i] = 0;
            }
            for (i = 1; i <= ni; i++)
            {
                v = b2[1, i];
                for (i_ = 1; i_ <= ni; i_++)
                {
                    mb[i_] = mb[i_] + v * vt[i, i_];
                }
            }

            //
            // Forming result
            //
            ctbl = new double[ni + 1 + 1];
            for (i = 1; i <= ni; i++)
            {
                ctbl[i - 1] = mb[i];
            }
            ctbl[ni] = a;
            ctbl[ni + 1] = b;
        }


        /*************************************************************************
        Weighted Chebyshev polynomial constrained least squares approximation.

        The algorithm reduces [A,B] to [-1,1] and builds the Chebyshev polynomials
        series by approximating a given function using the least squares method.

        Input parameters:
            X   -   abscissas, array[0..N-1]
            Y   -   function values, array[0..N-1]
            W   -   weights, array[0..N-1].  Each  item  in  the  squared  sum  of
                    deviations from given values is  multiplied  by  a  square  of
                    corresponding weight.
            A, B-   interval in which the approximating polynomials are built.
            N   -   number of points, N>0.
            XC, YC, DC-
                    constraints (see description below)., array[0..NC-1]
            NC  -   number of constraints. 0 <= NC < M+1.
            M   -   degree of polynomial, M>=0. This parameter is passed into  the
                    CalculateChebyshevLeastSquares subroutine.

        Output parameters:
            CTbl-   coefficient  table.  This  parameter  is   passed   into   the
                    CalculateChebyshevLeastSquares subroutine.

        Result:
            True, if the algorithm succeeded.
            False, if the internal singular value decomposition subroutine  hasn't
        converged or the given constraints could not be met  simultaneously  (e.g.
        P(0)=0 � P(0)=1).

        Specifying constraints:
            This subroutine can solve  the  problem  having  constrained  function
        values or its derivatives in several points. NC specifies  the  number  of
        constraints, DC - the type of constraints, XC and YC - constraints as such.
        Thus, for each i from 0 to NC-1 the following constraint is given:
            P(xc[i]) = yc[i],       if DC[i]=0
        or
            d/dx(P(xc[i])) = yc[i], if DC[i]=1
        (here P(x) is approximating polynomial).
            This version of the subroutine supports only either polynomial or  its
        derivative value constraints.  If  DC[i]  is  not  equal  to  0 and 1, the
        subroutine will be aborted. The number of constraints should be less  than
        the number of degrees of freedom of approximating  polynomial  -  M+1  (at
        that, it could be equal to 0).

          -- ALGLIB --
             Copyright by Bochkanov Sergey
        *************************************************************************/
        public static bool BuildChebyshevLeastSquaresConstrained(ref double[] x,
            ref double[] y,
            ref double[] w,
            double a,
            double b,
            int n,
            ref double[] xc,
            ref double[] yc,
            ref int[] dc,
            int nc,
            int m,
            ref double[] ctbl)
        {
            bool result = new bool();
            int i = 0;
            int j = 0;
            int reducedsize = 0;
            double[,] designmatrix = new double[0, 0];
            double[] rightpart = new double[0];
            double[,] cmatrix = new double[0, 0];
            double[,] c = new double[0, 0];
            double[,] u = new double[0, 0];
            double[,] vt = new double[0, 0];
            double[] d = new double[0];
            double[] cr = new double[0];
            double[] ws = new double[0];
            double[] tj = new double[0];
            double[] uj = new double[0];
            double[] dtj = new double[0];
            double[] tmp = new double[0];
            double[] tmp2 = new double[0];
            double[,] tmpmatrix = new double[0, 0];
            double v = 0;
            int i_ = 0;

            System.Diagnostics.Debug.Assert(n > 0);
            System.Diagnostics.Debug.Assert(m >= 0);
            System.Diagnostics.Debug.Assert(nc >= 0 & nc < m + 1);
            result = true;

            //
            // Initialize design matrix and right part.
            // Add fictional rows if needed to ensure that N>=M+1.
            //
            designmatrix = new double[Math.Max(n, m + 1) + 1, m + 1 + 1];
            rightpart = new double[Math.Max(n, m + 1) + 1];
            for (i = 1; i <= n; i++)
            {
                for (j = 1; j <= m + 1; j++)
                {
                    v = 2 * (x[i - 1] - a) / (b - a) - 1;
                    if (j == 1)
                    {
                        designmatrix[i, j] = 1.0;
                    }
                    if (j == 2)
                    {
                        designmatrix[i, j] = v;
                    }
                    if (j > 2)
                    {
                        designmatrix[i, j] = 2.0 * v * designmatrix[i, j - 1] - designmatrix[i, j - 2];
                    }
                }
            }
            for (i = 1; i <= n; i++)
            {
                for (j = 1; j <= m + 1; j++)
                {
                    designmatrix[i, j] = w[i - 1] * designmatrix[i, j];
                }
            }
            for (i = n + 1; i <= m + 1; i++)
            {
                for (j = 1; j <= m + 1; j++)
                {
                    designmatrix[i, j] = 0;
                }
            }
            for (i = 0; i <= n - 1; i++)
            {
                rightpart[i + 1] = w[i] * y[i];
            }
            for (i = n + 1; i <= m + 1; i++)
            {
                rightpart[i] = 0;
            }
            n = Math.Max(n, m + 1);

            //
            // Now N>=M+1 and we are ready to the next stage.
            // Handle constraints.
            // Represent feasible set of coefficients as x = C*t + d
            //
            c = new double[m + 1 + 1, m + 1 + 1];
            d = new double[m + 1 + 1];
            if (nc == 0)
            {

                //
                // No constraints
                //
                for (i = 1; i <= m + 1; i++)
                {
                    for (j = 1; j <= m + 1; j++)
                    {
                        c[i, j] = 0;
                    }
                    d[i] = 0;
                }
                for (i = 1; i <= m + 1; i++)
                {
                    c[i, i] = 1;
                }
                reducedsize = m + 1;
            }
            else
            {

                //
                // Constraints are present.
                // Fill constraints matrix CMatrix and solve CMatrix*x = cr.
                //
                cmatrix = new double[nc + 1, m + 1 + 1];
                cr = new double[nc + 1];
                tj = new double[m + 1];
                uj = new double[m + 1];
                dtj = new double[m + 1];
                for (i = 0; i <= nc - 1; i++)
                {
                    v = 2 * (xc[i] - a) / (b - a) - 1;
                    for (j = 0; j <= m; j++)
                    {
                        if (j == 0)
                        {
                            tj[j] = 1;
                            uj[j] = 1;
                            dtj[j] = 0;
                        }
                        if (j == 1)
                        {
                            tj[j] = v;
                            uj[j] = 2 * v;
                            dtj[j] = 1;
                        }
                        if (j > 1)
                        {
                            tj[j] = 2 * v * tj[j - 1] - tj[j - 2];
                            uj[j] = 2 * v * uj[j - 1] - uj[j - 2];
                            dtj[j] = j * uj[j - 1];
                        }
                        System.Diagnostics.Debug.Assert(dc[i] == 0 | dc[i] == 1);
                        if (dc[i] == 0)
                        {
                            cmatrix[i + 1, j + 1] = tj[j];
                        }
                        if (dc[i] == 1)
                        {
                            cmatrix[i + 1, j + 1] = dtj[j];
                        }
                    }
                    cr[i + 1] = yc[i];
                }

                //
                // Solve CMatrix*x = cr.
                // Fill C and d:
                // 1. SVD: CMatrix = U * WS * V^T
                // 2. C := V[1:M+1,NC+1:M+1]
                // 3. tmp := WS^-1 * U^T * cr
                // 4. d := V[1:M+1,1:NC] * tmp
                //
                if (!Svd.SvdDecomposition(cmatrix, nc, m + 1, 2, 2, 2, ref ws, ref u, ref vt))
                {
                    result = false;
                    return result;
                }
                if (ws[1] == 0 | ws[nc] <= ComplexMath.MachineEpsilon * 10 * Math.Sqrt(nc) * ws[1])
                {
                    result = false;
                    return result;
                }
                c = new double[m + 1 + 1, m + 1 - nc + 1];
                d = new double[m + 1 + 1];
                for (i = 1; i <= m + 1 - nc; i++)
                {
                    for (i_ = 1; i_ <= m + 1; i_++)
                    {
                        c[i_, i] = vt[nc + i, i_];
                    }
                }
                tmp = new double[nc + 1];
                for (i = 1; i <= nc; i++)
                {
                    v = 0.0;
                    for (i_ = 1; i_ <= nc; i_++)
                    {
                        v += u[i_, i] * cr[i_];
                    }
                    tmp[i] = v / ws[i];
                }
                for (i = 1; i <= m + 1; i++)
                {
                    d[i] = 0;
                }
                for (i = 1; i <= nc; i++)
                {
                    v = tmp[i];
                    for (i_ = 1; i_ <= m + 1; i_++)
                    {
                        d[i_] = d[i_] + v * vt[i, i_];
                    }
                }

                //
                // Reduce problem:
                // 1. RightPart := RightPart - DesignMatrix*d
                // 2. DesignMatrix := DesignMatrix*C
                //
                for (i = 1; i <= n; i++)
                {
                    v = 0.0;
                    for (i_ = 1; i_ <= m + 1; i_++)
                    {
                        v += designmatrix[i, i_] * d[i_];
                    }
                    rightpart[i] = rightpart[i] - v;
                }
                reducedsize = m + 1 - nc;
                tmpmatrix = new double[n + 1, reducedsize + 1];
                tmp = new double[n + 1];
                Blas.MatrixMatrixMultiply(ref designmatrix, 1, n, 1, m + 1, false, ref c, 1, m + 1, 1, reducedsize, false, 1.0, ref tmpmatrix, 1, n, 1, reducedsize, 0.0, ref tmp);
                Blas.CopyMatrix(ref tmpmatrix, 1, n, 1, reducedsize, ref designmatrix, 1, n, 1, reducedsize);
            }

            //
            // Solve reduced problem DesignMatrix*t = RightPart.
            //
            if (!Svd.SvdDecomposition(designmatrix, n, reducedsize, 2, 2, 2, ref ws, ref u, ref vt))
            {
                result = false;
                return result;
            }
            tmp = new double[reducedsize + 1];
            tmp2 = new double[reducedsize + 1];
            for (i = 1; i <= reducedsize; i++)
            {
                tmp[i] = 0;
            }
            for (i = 1; i <= n; i++)
            {
                v = rightpart[i];
                for (i_ = 1; i_ <= reducedsize; i_++)
                {
                    tmp[i_] = tmp[i_] + v * u[i, i_];
                }
            }
            for (i = 1; i <= reducedsize; i++)
            {
                if (ws[i] != 0 & ws[i] > ComplexMath.MachineEpsilon * 10 * Math.Sqrt(nc) * ws[1])
                {
                    tmp[i] = tmp[i] / ws[i];
                }
                else
                {
                    tmp[i] = 0;
                }
            }
            for (i = 1; i <= reducedsize; i++)
            {
                tmp2[i] = 0;
            }
            for (i = 1; i <= reducedsize; i++)
            {
                v = tmp[i];
                for (i_ = 1; i_ <= reducedsize; i_++)
                {
                    tmp2[i_] = tmp2[i_] + v * vt[i, i_];
                }
            }

            //
            // Solution is in the tmp2.
            // Transform it from t to x.
            //
            ctbl = new double[m + 2 + 1];
            for (i = 1; i <= m + 1; i++)
            {
                v = 0.0;
                for (i_ = 1; i_ <= reducedsize; i_++)
                {
                    v += c[i, i_] * tmp2[i_];
                }
                ctbl[i - 1] = v + d[i];
            }
            ctbl[m + 1] = a;
            ctbl[m + 2] = b;
            return result;
        }


        /*************************************************************************
        Calculation of a Chebyshev  polynomial  obtained   during  least  squares
        approximaion at the given point.

        Input parameters:
            M   -   order of polynomial (parameter of the
                    BuildChebyshevLeastSquares function).
            A   -   coefficient table.
                    A[0..M] contains coefficients of the i-th Chebyshev polynomial.
                    A[M+1] contains left boundary of approximation interval.
                    A[M+2] contains right boundary of approximation interval.
            X   -   point to perform calculations in.

        The result is the value at the given point.

        It should be noted that array A contains coefficients  of  the  Chebyshev
        polynomials defined on interval [-1,1].   Argument  is  reduced  to  this
        interval before calculating polynomial value.
          -- ALGLIB --
             Copyright by Bochkanov Sergey
        *************************************************************************/
        public static double CalculateChebyshevLeastAquares(int m,
            ref double[] a,
            double x)
        {
            double result = 0;
            double b1 = 0;
            double b2 = 0;
            int i = 0;

            x = 2 * (x - a[m + 1]) / (a[m + 2] - a[m + 1]) - 1;
            b1 = 0;
            b2 = 0;
            i = m;
            do
            {
                result = 2 * x * b1 - b2 + a[i];
                b2 = b1;
                b1 = result;
                i = i - 1;
            }
            while (i >= 0);
            result = result - x * b2;
            return result;
        }
    }
}