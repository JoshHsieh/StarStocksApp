// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;

namespace StarStocks.Core.Helpers
{
    public class OptionHelper
    {
        /// <summary>
        /// Interest rate, 利率??
        /// </summary>
        public double r { get; set; }

        /// <summary>
        /// 波動率 Volatility, IV
        /// </summary>
        public double sigma { get; set; }

        /// <summary>
        /// 行權價 Strike price
        /// </summary>
        public double K { get; set; }

        /// <summary>
        /// 合約到期日，例如一個月後到期，因為是double 型態所以這邊填入 30.0/ 365.0
        /// </summary>
        public double T { get; set; } // Expiry date

        /// <summary>
        /// 尚不知用途
        /// </summary>
        public double b { get; set; } // Cost of carry

        /// <summary>
        /// call or put
        /// </summary>
        public string otyp { get; set; } // Option type (call, put)

        public OptionHelper()
        {
            otyp = "C";
        }

        public OptionHelper(string OptionType, double StrikePrice, double Sigma, double Maturity)
        {
            this.otyp = OptionType;
            this.K = StrikePrice;
            this.sigma = Sigma;
            this.T = Maturity;
        }

        public OptionHelper(string OptionType, double StrikePrice, double Sigma, double Maturity, double Riskfree = 0, double CostofCarry = 0)
        {
            this.otyp = OptionType;
            this.K = StrikePrice;
            this.sigma = Sigma;
            this.T = Maturity;
            this.r = Riskfree;
            this.b = CostofCarry;
        }

        /// <summary>
        /// 計算 price
        /// </summary>
        /// <param name="U">underlying 標的物目前價格</param>
        /// <returns></returns>
        public double Price(double U)
        {
            if (otyp == "C")
            {
                //Console.WriteLine("call..{0}", U);
                return CallPrice(U);
            }

            else return PutPrice(U);
        }

        private double CallPrice(double U)
        {
            //Console.WriteLine("{0}, {1}, {2}, {3}, {4}", sigma, T, K, r, b);
            double tmp = sigma * Math.Sqrt(T);

            double d1 = (Math.Log(U / K) + (sigma * sigma) * 0.5 * T) / tmp;

            double d2 = d1 - tmp;

            return (U * Math.Exp(-b * T) * FinanceMathHelper.N(d1)) - (K * Math.Exp(-r * T) * FinanceMathHelper.N(d2));
        }

        private double PutPrice(double U)
        {
            double tmp = sigma * Math.Sqrt(T);

            double d1 = (Math.Log(U / K) + (sigma * sigma) * 0.5 * T) / tmp;

            double d2 = d1 - tmp;

            return (K * Math.Exp(-r * T) * FinanceMathHelper.N(-d2)) - (U * Math.Exp((-b) * T) * FinanceMathHelper.N(-d1));
        }


        /// <summary>
        /// 計算 Delta
        /// </summary>
        /// <param name="U"></param>
        /// <returns></returns>
        public double Delta(double U)
        {
            if (otyp == "C")
            {
                return CallDelta(U);
            }

            else return PutDelta(U);
        }

        private double CallDelta(double U)
        {
            double tmp = sigma * Math.Sqrt(T);
            double d1 = (Math.Log(U / K) + (sigma * sigma * 0.5) * T) / tmp;
            return Math.Exp(-b * T) * FinanceMathHelper.N(d1);
        }

        private double PutDelta(double U)
        {
            double tmp = sigma * Math.Sqrt(T);

            double d1 = (Math.Log(U / K) + (sigma * sigma * 0.5) * T) / tmp;

            return Math.Exp((-b) * T) * (FinanceMathHelper.N(d1) - 1);
        }

        /// <summary>
        /// Gamma
        /// </summary>
        /// <param name="U"></param>
        /// <returns></returns>
        public double Gamma(double U)
        {
            double tmp = sigma * Math.Sqrt(T);

            double d1 = (Math.Log(U / K) + (sigma * sigma) * 0.5 * T) / tmp;

            return (FinanceMathHelper.n(d1) * Math.Exp((-b) * T)) / (U * tmp);
        }

        /// <summary>
        /// theta
        /// </summary>
        /// <param name="U"></param>
        /// <returns></returns>
        public double Theta(double U)
        {
            if (otyp == "C")
            {
                return CallTheta(U);
            }
            else return PutTheta(U);
        }

        private double CallTheta(double U)
        {
            double tmp = sigma * Math.Sqrt(T);
            double d1 = (Math.Log(U / K) + (sigma * sigma) * 0.5 * T) / tmp;
            double d2 = d1 - tmp;
            double result = -U * FinanceMathHelper.n(d1) * sigma * Math.Exp(-b * T) / (2 * Math.Sqrt(T));
            result += b * U * FinanceMathHelper.N(d1) * Math.Exp(-b * T);
            result -= r * K * Math.Exp(-r * T) * FinanceMathHelper.N(d2);
            return result;
        }

        private double PutTheta(double U)
        {
            double tmp = sigma * Math.Sqrt(T);
            double d1 = (Math.Log(U / K) + (sigma * sigma) * 0.5 * T) / tmp;
            double d2 = d1 - tmp;
            double result = -U * FinanceMathHelper.n(d1) * sigma * Math.Exp(-b * T) / (2 * Math.Sqrt(T));
            result -= b * U * FinanceMathHelper.N(-d1) * Math.Exp(-b * T);
            result += r * K * Math.Exp(-r * T) * FinanceMathHelper.N(-d2);
            return result;
        }

        /// <summary>
        /// vega
        /// </summary>
        /// <param name="U"></param>
        /// <returns></returns>
        public double Vega(double U)
        {
            double tmp = sigma * Math.Sqrt(T);
            double d1 = (Math.Log(U / K) + (sigma * sigma) * 0.5 * T) / tmp;
            return U * Math.Sqrt(T) * FinanceMathHelper.n(d1) * Math.Exp(-b * T) / 100;
        }

        /// <summary>
        /// rho
        /// </summary>
        /// <param name="U"></param>
        /// <returns></returns>
        public double Rho(double U)
        {
            if (otyp == "C")
            {
                return CallRho(U);
            }
            else return PutRho(U);
        }

        private double CallRho(double U)
        {
            double tmp = sigma * Math.Sqrt(T);
            double d1 = (Math.Log(U / K) + (b + (sigma * sigma) * 0.5) * T) / tmp;
            double d2 = d1 - tmp;
            return K * T * Math.Exp(-r * T) * FinanceMathHelper.N(d2) / 100;
        }
        private double PutRho(double U)
        {
            double tmp = sigma * Math.Sqrt(T);
            double d1 = (Math.Log(U / K) + (b + (sigma * sigma) * 0.5) * T) / tmp;
            double d2 = d1 - tmp;
            return -K * T * Math.Exp(-r * T) * FinanceMathHelper.N(-d2) / 100;
        }

        /// <summary>
        /// Cost of Carry
        /// </summary>
        /// <param name="U"></param>
        /// <returns></returns>
        public double Coc(double U)
        {
            return U * Math.Exp(b * T) - U;
        }

        /// <summary>
        /// calculate gamma exposure by call or put
        /// uses the C# Math.PI field rather than a constant as in the C++ implementaion
        /// the value of Pi is 3.14159265358979323846
        /// https://gist.github.com/achvaicer/598242286181f5c501498a645e96f8ac
        /// </summary>
        /// <param name="callPutFlag"></param>
        /// <param name="s">underlying stock price</param>
        /// <param name="k">strike price</param>
        /// <param name="t">Years to maturity</param>
        /// <param name="r">Risk-free rate</param>
        /// <param name="v">Volatility</param>
        /// <returns></returns>
        public double CalculateGex(string callPutFlag, double s, double k, double t, double r, double v)
        {
            double d1 = 0.0;
            double d2 = 0.0;
            double dBlackScholes = 0.0;

            d1 = (Math.Log(s / k) + (r + v * v / 2.0) * t) / (v * Math.Sqrt(t));
            d2 = d1 - v * Math.Sqrt(t);

            if (callPutFlag.ToLower() == "c")
            {
                dBlackScholes = s * Cnd(d1) - k * Math.Exp(-r * T) * Cnd(d2);
            }
            else if (callPutFlag.ToLower() == "p")
            {
                dBlackScholes = k * Math.Exp(-r * T) * Cnd(-d2) - s * Cnd(-d1);
            }

            return dBlackScholes;
        }

        public double Cnd(double X)
        {
            double L = 0.0;
            double K = 0.0;
            double dCND = 0.0;
            const double a1 = 0.31938153;
            const double a2 = -0.356563782;
            const double a3 = 1.781477937;
            const double a4 = -1.821255978;
            const double a5 = 1.330274429;
            L = Math.Abs(X);
            K = 1.0 / (1.0 + 0.2316419 * L);
            dCND = 1.0 - 1.0 / Math.Sqrt(2 * Convert.ToDouble(Math.PI.ToString())) *
                Math.Exp(-L * L / 2.0) * (a1 * K + a2 * K * K + a3 * Math.Pow(K, 3.0) +
                a4 * Math.Pow(K, 4.0) + a5 * Math.Pow(K, 5.0));

            if (X < 0)
            {
                return 1.0 - dCND;
            }
            else
            {
                return dCND;
            }
        }
    }
}
