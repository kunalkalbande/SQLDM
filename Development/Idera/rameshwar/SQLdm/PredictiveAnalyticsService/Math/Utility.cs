using System;
using System.Collections.Generic;
using System.Text;

namespace PredictiveAnalyticsService.Math
{
    internal static class Utility
    {
        private const double MAXLOG = 7.09782712893383996732E2;
        private const double SQRTH  = 7.07106781186547524401E-1;

        private static double[] T;
        private static double[] U;
        private static double[] P;
        private static double[] Q;
        private static double[] R;
        private static double[] S;

        #region Constructor 

        static Utility()
        {
            T = new double[]
            {
                9.60497373987051638749E0,
                9.00260197203842689217E1,
                2.23200534594684319226E3,
		        7.00332514112805075473E3,
		        5.55923013010394962768E4
            };

            U = new double[]
            {
                3.35617141647503099647E1,
			    5.21357949780152679795E2,
			    4.59432382970980127987E3,
			    2.26290000613890934246E4,
			    4.92673942608635921086E4
            };

            P = new double[]
		    {
			    2.46196981473530512524E-10,
			    5.64189564831068821977E-1,
			    7.46321056442269912687E0,
			    4.86371970985681366614E1,
			    1.96520832956077098242E2,
			    5.26445194995477358631E2,
			    9.34528527171957607540E2,
			    1.02755188689515710272E3,
			    5.57535335369399327526E2
		    };

		    Q = new double[]
		    {
			    1.32281951154744992508E1,
			    8.67072140885989742329E1,
			    3.54937778887819891062E2,
			    9.75708501743205489753E2,
			    1.82390916687909736289E3,
			    2.24633760818710981792E3,
			    1.65666309194161350182E3,
			    5.57535340817727675546E2
		    };
      
		    R = new double[]
		    {
			    5.64189583547755073984E-1,
			    1.27536670759978104416E0,
			    5.01905042251180477414E0,
			    6.16021097993053585195E0,
			    7.40974269950448939160E0,
			    2.97886665372100240670E0
		    };

            S = new double[]
		    {
			    2.26052863220117276590E0,
			    9.39603524938001434673E0,
			    1.20489539808096656605E1,
			    1.70814450747565897222E1,
			    9.60896809063285878198E0,
			    3.36907645100081516050E0
		    };
        }
        #endregion

        public static void Normalize(double[] x)
        {
            double sum = 0;

            for (int i = 0; i < x.Length; i++)
                sum += x[i];

            if (Double.IsNaN(sum))
                throw new ApplicationException("Unable to normalize array!");

            if (sum == 0)
                return;

            for (int i = 0; i < x.Length; i++)
                x[i] /= sum;
        }

        public static double NormalProbability(double a)
        {
            double x;
            double y;
            double z;

            x = a * SQRTH;
            z = System.Math.Abs(x);

            if (z < SQRTH)
                y = 0.5 + 0.5 * ErrorFunction(x);
            else
            {
                y = 0.5 * ErrorFunctionComplement(z);

                if (x > 0)
                    y = 1.0 - y;
            }

            return y;
        }

        private static double ErrorFunction(double x)
        {
            double y;
            double z;

            if (System.Math.Abs(x) > 1.0)
                return 1.0 - ErrorFunctionComplement(x);

            z = x * x;
            y = x * EvalPolynomial(z, T, 4) / EvalPolynomialC1(z, U, 5);

            return y;
        }

        private static double ErrorFunctionComplement(double a)
        {
            double x;
            double y;
            double z;
            double p;
            double q;

            if (a < 0.0) 
                x = -a;
            else 
                x = a;

            if (x < 1.0)
                return 1.0 - ErrorFunction(a);

            z = -a * a;

            if (z < -MAXLOG)
            {
                if (a < 0) 
                    return 2.0;
                else 
                    return 0.0;
            }

            z = System.Math.Exp(z);

            if (x < 8.0)
            {
                p = EvalPolynomial(x, P, 8);
                q = EvalPolynomialC1(x, Q, 8);
            }
            else
            {
                p = EvalPolynomial(x, R, 5);
                q = EvalPolynomialC1(x, S, 6);
            }

            y = (z * p) / q;

            if (a < 0) 
                y = 2.0 - y;

            if (y == 0.0)
            {
                if (a < 0) 
                    return 2.0;
                else 
                    return 0.0;
            }

            return y;
        }

        private static double EvalPolynomialC1(double x, double[] coef, int N) 
	    {      
		    double ans;
		    ans = x + coef[0];
      
		    for(int i = 1; i < N; i++) 
                ans = ans * x + coef[i];
      
		    return ans;
	    }

	    private static double EvalPolynomial( double x, double[] coef, int N ) 
	    {
		    double ans;
		    ans = coef[0];
      
		    for(int i = 1; i <= N; i++) 
                ans = ans * x + coef[i];
      
		    return ans;
	    }
    }
}
