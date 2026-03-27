using SQLite;
using System;

namespace CreditSimulator.Models
{
    public class SimulationRecord
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        
        public string CustomerName { get; set; }
        public double ProductPrice { get; set; }
        public double DownPayment { get; set; }
        public double Principal { get; set; }
        public int TenorMonths { get; set; }
        public double MonthlyInstallment { get; set; }
        public double Margin { get; set; }
        public double AdminFee { get; set; }
        public double TotalCredit { get; set; }
        public double GrandTotal { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
