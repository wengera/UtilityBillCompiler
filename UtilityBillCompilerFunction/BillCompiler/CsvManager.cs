using System;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;

namespace UtilityBillCompilerFunction.BillCompiler
{
    public class CsvManager
    {
        private int monthIndex = 0;

        public string Data;
        public Guid JobId;
        public IEnumerable<UtilityRecord> CsvRecords;
        public IEnumerable<TenantRecord> TenantCsvRecords;

        public List<OutputRecord> OutputRecords = new List<OutputRecord>();

        public CsvManager(string data)
        {
            Data = data;
            JobId = Guid.NewGuid();
        }

        #region CsvOperations
        private void ExtractRecords()
        {
            try
            {
                
                using (var reader = new StringReader(Data))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    CsvRecords = csv.GetRecords<UtilityRecord>().Where(r => !string.IsNullOrWhiteSpace(r.Utility)).ToList();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        private void ExtractTenantRecords()
        {
            try
            {
                using (var reader = new StringReader(Data))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    TenantCsvRecords = csv.GetRecords<TenantRecord>().Where(r => !string.IsNullOrWhiteSpace(r.Tenant)).ToList();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        #endregion

        public void RunJob()
        {
            ExtractRecords();
            ExtractTenantRecords();

            Console.WriteLine("");
            foreach (var record in CsvRecords)
            {
                Console.WriteLine($"{record.Utility}: ${record.Amount}");
                Console.WriteLine($"--------------------------------");
                decimal totalTenantDays = record.TotalActiveDaysBetweenTenants(TenantCsvRecords);
                decimal tenantSum = 0;
                foreach (var tenant in TenantCsvRecords)
                {
                    decimal tenantDays = tenant.TotalActiveDays(record);
                    decimal amountDue = Math.Round((record.Amount ?? 0) * (tenantDays / totalTenantDays), 2);
                    decimal percentActive = Math.Round(tenantDays / totalTenantDays * 100, 2);
                    tenantSum += amountDue;
                    var outRecord = new OutputRecord(JobId.ToString(), record, tenant, amountDue, (int)tenantDays, percentActive);
                    OutputRecords.Add(outRecord);
                    Console.WriteLine(outRecord.ToString());
                }
                Console.WriteLine($"Sum: {Math.Round(tenantSum, 2)}");
                Console.WriteLine($"--------------------------------\n");
            }
            OutputRecords = OutputRecords.OrderBy(r => r.Tenant.Tenant).ToList();
        }

        public string OutputToCsv()
        {
            var builder = new StringBuilder();

            builder.AppendLine(OutputRecord.Header);

            foreach (var record in OutputRecords)
                builder.AppendLine(record.ToCsv());

            return builder.ToString();
        }
    }
}
