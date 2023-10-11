using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace UtilityBillCompilerFunction.BillCompiler
{
    public class OutputRecord
    {
        public string JobId { get; set; }
        public UtilityRecord Utility {get; set;}
        public TenantRecord Tenant { get; set; }
        public Decimal Amount { get; set; }
        public int ActiveDays { get; set; }
        public Decimal PercentActive { get; set; }

        public OutputRecord(string jobId, UtilityRecord utility, TenantRecord tenant, decimal amount, int activeDays, decimal percentActive)
        {
            JobId = jobId;
            Utility = utility;
            Tenant = tenant;
            Amount = amount;
            ActiveDays = activeDays;
            PercentActive = percentActive;
        }

        public static string Header => "Job Id,Job Processed,Utility,Utility Start Date,Utility End Date,Tenant,Amount Owed,Billable Days,Percent Active";
        
        public string ToCsv()
        {
            return $"{JobId},{DateTimeOffset.Now.ToString()},{Utility.Utility},{Utility.UtilStartDate?.ToString("MM-dd-yyyy")},{Utility.UtilEndDate?.ToString("MM-dd-yyyy")},{Tenant.Tenant},${Amount},{ActiveDays},%{PercentActive}";
        }

        public override string ToString() {

            return $"[{Tenant.Tenant}] ${Amount} | % {PercentActive} | Active Days {ActiveDays}";
        }
    }
}
