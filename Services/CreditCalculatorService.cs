using System;

namespace CreditSimulator.Services
{
    public static class CreditCalculatorService
    {
        public const double AdminFee = 200000;

        public static double GetFactor(int tenor)
        {
            return tenor switch
            {
                3 => 0.368,
                6 => 0.208,
                9 => 0.1475,
                12 => 0.121,
                _ => throw new ArgumentException("Tenor not supported")
            };
        }

        public static (double Principal, double Installment, double TotalCredit, double Margin, double GrandTotal) Calculate(double price, double dp, int tenor)
        {
            if (dp < (price * 0.20))
                throw new ArgumentException("DP must be at least 20% of the price");

            double principal = price - dp;
            double factor = GetFactor(tenor);
            double installment = principal * factor;
            double totalCredit = installment * tenor;
            double margin = totalCredit - principal;
            double grandTotal = dp + AdminFee + totalCredit;

            return (principal, installment, totalCredit, margin, grandTotal);
        }

        public static double CalculateEarlyPayoff(double principal, int targetTenor, double currentInstallment, int monthsPaid)
        {
            double targetFactor = GetFactor(targetTenor);
            return (principal * targetFactor * targetTenor) - (currentInstallment * monthsPaid);
        }
    }
}
