using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilityBillCompilerFunction.BillCompiler
{
    public class TenantRecord
    {
        public string? Tenant { get; set; }
        public DateTime? TenantStartDate { get; set; }
        public DateTime? TenantEndDate { get; set; }

        public int TotalActiveDays(UtilityRecord bill)
        {
            if (bill.UtilStartDate == null || bill.UtilEndDate == null)
                throw new Exception($"Invalid start or end date for Bill: {Tenant}");

            if (bill.UtilEndDate < TenantStartDate)
                return 0;

            DateTime? startRef = TenantStartDate > bill.UtilStartDate ? TenantStartDate : bill.UtilStartDate;

            DateTime? endRef = DateTime.MinValue;
            if (TenantEndDate == null)
                endRef = bill.UtilEndDate;
            else
                endRef = TenantEndDate < bill.UtilEndDate ? TenantEndDate : bill.UtilEndDate;

            if (startRef == null || endRef == null)
                throw new Exception($"Invalid start or end date for Tenant: {Tenant}");
            else
            {
                TimeSpan duration = (DateTime)endRef - (DateTime)startRef;
                return Math.Max(duration.Days + 1, 0);
            }
        }
    }
}
