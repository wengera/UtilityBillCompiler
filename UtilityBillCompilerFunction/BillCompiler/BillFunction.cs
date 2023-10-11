using System.IO;
using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using UtilityBillCompilerFunction.BillCompiler;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace UtilityBillCompilerFunction
{
    public class BillFunction
    {
        private readonly ILogger _logger;
        private readonly CsvManager _manager;

        public BillFunction(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<BillFunction>();
        }

        [Function("BillFunction")]
        public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            try
            {
                string csvOutput = "";
                using (var reader = new StreamReader(req.Body))
                {
                    var data = reader.ReadToEnd();
                    var manager = new CsvManager(data);
                    manager.RunJob();
                    csvOutput = manager.OutputToCsv();
                }

                var response = req.CreateResponse(HttpStatusCode.OK);
                response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
                response.WriteString(csvOutput);
                return response;
            }
            catch(Exception e)
            {
                _logger.LogError(e.ToString());
                var response = req.CreateResponse(HttpStatusCode.InternalServerError);
                response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
                response.WriteString("Internal Server Error - Failed to parse file");
                return response;
            }
        }
    }
}
