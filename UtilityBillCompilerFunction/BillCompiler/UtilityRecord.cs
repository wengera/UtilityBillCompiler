using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace UtilityBillCompilerFunction.BillCompiler
{
    public class UtilityRecord
    {
        public string? Utility { get; set; }
        public DateTime? UtilStartDate { get; set; }
        public DateTime? UtilEndDate { get; set; }
        public decimal? Amount { get; set; } = 0m;

        public int TotalActiveDaysBetweenTenants(IEnumerable<TenantRecord> tenants)
        {
            var total = 0;
            foreach (var tenant in tenants)
                total += tenant.TotalActiveDays(this);

            return total;
        }

    }
}
