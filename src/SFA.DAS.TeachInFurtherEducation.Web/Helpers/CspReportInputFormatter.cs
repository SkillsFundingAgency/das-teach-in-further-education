using Microsoft.AspNetCore.Mvc.Formatters;
using SFA.DAS.TeachInFurtherEducation.Web.Models;
using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SFA.DAS.TeachInFurtherEducation.Web.Helpers
{
    public class CspReportInputFormatter : TextInputFormatter
    {
        public CspReportInputFormatter()
        {
            SupportedMediaTypes.Add("application/csp-report");
            SupportedEncodings.Add(Encoding.UTF8);
            SupportedEncodings.Add(Encoding.Unicode);
        }

        protected override bool CanReadType(Type type)
        {
            return type == typeof(CspViolationReport);
        }

        public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context, Encoding encoding)
        {
            var request = context.HttpContext.Request;
            using var reader = new StreamReader(request.Body, encoding);
            var body = await reader.ReadToEndAsync();

            // Deserialize JSON from the request body
            var report = JsonSerializer.Deserialize<CspViolationReport>(body, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            return await InputFormatterResult.SuccessAsync(report);
        }
    }

}
