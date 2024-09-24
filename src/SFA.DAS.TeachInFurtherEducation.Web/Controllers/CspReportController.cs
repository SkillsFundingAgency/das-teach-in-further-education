using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.TeachInFurtherEducation.Web.Models;
using SFA.DAS.TeachInFurtherEducation.Web.Services;
using SFA.DAS.TeachInFurtherEducation.Web.Services.Interfaces;
using System;

namespace SFA.DAS.TeachInFurtherEducation.Web.Controllers
{


    [ApiController]
    [Route("api/csp-violations")]
    public class CspReportController : ControllerBase
    {
        private readonly ILogger<CspReportController> _logger;
        private readonly ICspReportService _cspReportService;

        public CspReportController(ICspReportService cspReportService, ILogger<CspReportController> logger)
        {
            _cspReportService = cspReportService;
            _logger = logger;
        }

        [HttpPost]
        public IActionResult Post([FromBody] CspViolationReport report)
        {
            _logger.LogInformation("CSP Violation Report Received");

            // Get the current domain from the request (environment-specific)
            var currentHost = $"{Request.Scheme}://{Request.Host}";

            // Validate the Referer or Origin header against the current host
            var referer = Request.Headers["Referer"].ToString();
            var origin = Request.Headers["Origin"].ToString();

            // Make sure the referer or origin is from the same domain as the current request
            if (!referer.StartsWith(currentHost, StringComparison.OrdinalIgnoreCase) &&
                !origin.StartsWith(currentHost, StringComparison.OrdinalIgnoreCase))
            {
                return Unauthorized(); // Reject the request if the referer or origin is invalid
            }

            // Log and process the CSP report
            _cspReportService.LogReport(report);

            return Ok();
        }
    }

}
