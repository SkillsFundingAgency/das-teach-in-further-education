// CspReportInputFormatterTests.cs
using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SFA.DAS.TeachInFurtherEducation.Web.Helpers;
using SFA.DAS.TeachInFurtherEducation.Web.Models;
using Xunit;

namespace SFA.DAS.TeachInFurtherEducation.Web.Tests.Helpers
{
    public class CspReportInputFormatterTests
    {
        private readonly CspReportInputFormatter _formatter;

        public CspReportInputFormatterTests()
        {
            // Initialize the formatter
            _formatter = new CspReportInputFormatter();
        }

        #region Constructor Tests

        [Fact]
        public void Constructor_AddsSupportedMediaTypes()
        {
            // Assert
            Assert.Contains("application/csp-report", _formatter.SupportedMediaTypes);
        }

        [Fact]
        public void Constructor_AddsSupportedEncodings()
        {
            // Assert
            Assert.Contains(Encoding.UTF8, _formatter.SupportedEncodings);
            Assert.Contains(Encoding.Unicode, _formatter.SupportedEncodings);
        }

        #endregion

        #region CanRead Tests

        [Fact]
        public void CanRead_ReturnsTrue_ForSupportedMediaType_AndSupportedModelType()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Request.ContentType = "application/csp-report";

            var modelMetadataProvider = new EmptyModelMetadataProvider();
            var modelMetadata = modelMetadataProvider.GetMetadataForType(typeof(CspViolationReport));

            var modelState = new ModelStateDictionary();

            // Create a reader factory (dummy, as CanRead does not use it)
            Func<Stream, Encoding, TextReader> readerFactory = (stream, encoding) => new StreamReader(stream, encoding);

            var inputFormatterContext = new InputFormatterContext(
                httpContext,
                "modelName",
                modelState,
                modelMetadata,
                readerFactory);

            // Act
            var canRead = _formatter.CanRead(inputFormatterContext);

            // Assert
            Assert.True(canRead);
        }

        [Fact]
        public void CanRead_ReturnsFalse_WhenUnsupportedMediaType()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Request.ContentType = "application/json"; // Unsupported media type

            var modelMetadataProvider = new EmptyModelMetadataProvider();
            var modelMetadata = modelMetadataProvider.GetMetadataForType(typeof(CspViolationReport));

            var modelState = new ModelStateDictionary();

            Func<Stream, Encoding, TextReader> readerFactory = (stream, encoding) => new StreamReader(stream, encoding);

            var inputFormatterContext = new InputFormatterContext(
                httpContext,
                "modelName",
                modelState,
                modelMetadata,
                readerFactory);

            // Act
            var canRead = _formatter.CanRead(inputFormatterContext);

            // Assert
            Assert.False(canRead);
        }

        [Fact]
        public void CanRead_ReturnsFalse_WhenUnsupportedModelType()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Request.ContentType = "application/csp-report";

            var modelMetadataProvider = new EmptyModelMetadataProvider();
            var modelMetadata = modelMetadataProvider.GetMetadataForType(typeof(string)); // Unsupported model type

            var modelState = new ModelStateDictionary();

            Func<Stream, Encoding, TextReader> readerFactory = (stream, encoding) => new StreamReader(stream, encoding);

            var inputFormatterContext = new InputFormatterContext(
                httpContext,
                "modelName",
                modelState,
                modelMetadata,
                readerFactory);

            // Act
            var canRead = _formatter.CanRead(inputFormatterContext);

            // Assert
            Assert.False(canRead);
        }

        [Fact]
        public void CanRead_ThrowsInvalidOperationException_WhenNoSupportedMediaTypes()
        {
            // Arrange
            // Create a formatter with no supported media types
            var formatter = new CspReportInputFormatterWithoutMediaTypes();

            var httpContext = new DefaultHttpContext();
            httpContext.Request.ContentType = "application/csp-report";

            var modelMetadataProvider = new EmptyModelMetadataProvider();
            var modelMetadata = modelMetadataProvider.GetMetadataForType(typeof(CspViolationReport));

            var modelState = new ModelStateDictionary();

            Func<Stream, Encoding, TextReader> readerFactory = (stream, encoding) => new StreamReader(stream, encoding);

            var inputFormatterContext = new InputFormatterContext(
                httpContext,
                "modelName",
                modelState,
                modelMetadata,
                readerFactory);

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => formatter.CanRead(inputFormatterContext));
            Assert.Contains("SupportedMediaTypes", exception.Message);
        }

        #endregion

        #region ReadRequestBodyAsync Tests

        [Fact]
        public async Task ReadRequestBodyAsync_ReturnsCspViolationReport_WhenJsonIsValid()
        {
            // Arrange
            var reportDetails = new CspReportDetails
            {
                DocumentUri = "https://example.com",
                Referrer = "https://referrer.com",
                ViolatedDirective = "script-src 'self'",
                EffectiveDirective = "script-src",
                OriginalPolicy = "script-src 'self' https://apis.example.com",
                Disposition = "enforce",
                BlockedUri = "https://malicious.com/script.js",
                StatusCode = 200,
                ScriptSample = "console.log('test');"
            };

            var cspReport = new CspViolationReport
            {
                CspReport = reportDetails
            };

            var json = JsonSerializer.Serialize(cspReport, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Body = stream;
            httpContext.Request.ContentType = "application/csp-report";

            var modelMetadataProvider = new EmptyModelMetadataProvider();
            var modelMetadata = modelMetadataProvider.GetMetadataForType(typeof(CspViolationReport));

            var modelState = new ModelStateDictionary();

            // Create a reader factory
            Func<Stream, Encoding, TextReader> readerFactory = (stream, encoding) => new StreamReader(stream, encoding);

            var inputFormatterContext = new InputFormatterContext(
                httpContext,
                "modelName",
                modelState,
                modelMetadata,
                readerFactory);

            // Act
            var result = await _formatter.ReadRequestBodyAsync(inputFormatterContext, Encoding.UTF8);

            // Assert
            Assert.True(result.IsModelSet);
            var deserializedReport = Assert.IsType<CspViolationReport>(result.Model);
            Assert.NotNull(deserializedReport.CspReport);
            Assert.Equal(reportDetails.DocumentUri, deserializedReport.CspReport.DocumentUri);
            Assert.Equal(reportDetails.Referrer, deserializedReport.CspReport.Referrer);
            Assert.Equal(reportDetails.ViolatedDirective, deserializedReport.CspReport.ViolatedDirective);
            Assert.Equal(reportDetails.EffectiveDirective, deserializedReport.CspReport.EffectiveDirective);
            Assert.Equal(reportDetails.OriginalPolicy, deserializedReport.CspReport.OriginalPolicy);
            Assert.Equal(reportDetails.Disposition, deserializedReport.CspReport.Disposition);
            Assert.Equal(reportDetails.BlockedUri, deserializedReport.CspReport.BlockedUri);
            Assert.Equal(reportDetails.StatusCode, deserializedReport.CspReport.StatusCode);
            Assert.Equal(reportDetails.ScriptSample, deserializedReport.CspReport.ScriptSample);
        }

        [Fact]
        public async Task ReadRequestBodyAsync_ThrowsJsonException_WhenJsonIsInvalid()
        {
            // Arrange
            var invalidJson = "{ invalid json }";

            var stream = new MemoryStream(Encoding.UTF8.GetBytes(invalidJson));
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Body = stream;
            httpContext.Request.ContentType = "application/csp-report";

            var modelMetadataProvider = new EmptyModelMetadataProvider();
            var modelMetadata = modelMetadataProvider.GetMetadataForType(typeof(CspViolationReport));

            var modelState = new ModelStateDictionary();

            // Create a reader factory
            Func<Stream, Encoding, TextReader> readerFactory = (stream, encoding) => new StreamReader(stream, encoding);

            var inputFormatterContext = new InputFormatterContext(
                httpContext,
                "modelName",
                modelState,
                modelMetadata,
                readerFactory);

            // Act & Assert
            await Assert.ThrowsAsync<JsonException>(async () =>
                await _formatter.ReadRequestBodyAsync(inputFormatterContext, Encoding.UTF8));
        }

        [Fact]
        public async Task ReadRequestBodyAsync_ReturnsNull_WhenBodyIsEmpty()
        {
            // Arrange
            var json = "";

            var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Body = stream;
            httpContext.Request.ContentType = "application/csp-report";

            var modelMetadataProvider = new EmptyModelMetadataProvider();
            var modelMetadata = modelMetadataProvider.GetMetadataForType(typeof(CspViolationReport));

            var modelState = new ModelStateDictionary();

            // Create a reader factory
            Func<Stream, Encoding, TextReader> readerFactory = (stream, encoding) => new StreamReader(stream, encoding);

            var inputFormatterContext = new InputFormatterContext(
                httpContext,
                "modelName",
                modelState,
                modelMetadata,
                readerFactory);

            // Act
            var result = await _formatter.ReadRequestBodyAsync(inputFormatterContext, Encoding.UTF8);

            // Assert
            Assert.True(result.IsModelSet);
            Assert.Null(result.Model);
        }

        [Fact]
        public async Task ReadRequestBodyAsync_ReturnsCspViolationReport_WithPartialData_WhenSomeFieldsAreMissing()
        {
            // Arrange
            // CspReportDetails missing some properties (e.g., DocumentUri)
            var reportDetails = new CspReportDetails
            {
                DocumentUri = null, // Missing
                Referrer = "https://referrer.com",
                ViolatedDirective = "script-src 'self'",
                EffectiveDirective = "script-src",
                OriginalPolicy = "script-src 'self' https://apis.example.com",
                Disposition = "enforce",
                BlockedUri = "https://malicious.com/script.js",
                StatusCode = 200,
                ScriptSample = null // Optional
            };

            var cspReport = new CspViolationReport
            {
                CspReport = reportDetails
            };

            var json = JsonSerializer.Serialize(cspReport, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Body = stream;
            httpContext.Request.ContentType = "application/csp-report";

            var modelMetadataProvider = new EmptyModelMetadataProvider();
            var modelMetadata = modelMetadataProvider.GetMetadataForType(typeof(CspViolationReport));

            var modelState = new ModelStateDictionary();

            // Create a reader factory
            Func<Stream, Encoding, TextReader> readerFactory = (stream, encoding) => new StreamReader(stream, encoding);

            var inputFormatterContext = new InputFormatterContext(
                httpContext,
                "modelName",
                modelState,
                modelMetadata,
                readerFactory);

            // Act
            var result = await _formatter.ReadRequestBodyAsync(inputFormatterContext, Encoding.UTF8);

            // Assert
            Assert.True(result.IsModelSet);
            var deserializedReport = Assert.IsType<CspViolationReport>(result.Model);
            Assert.NotNull(deserializedReport.CspReport);
            Assert.Null(deserializedReport.CspReport.DocumentUri); // Confirm it's null
            Assert.Equal(reportDetails.Referrer, deserializedReport.CspReport.Referrer);
            Assert.Equal(reportDetails.ViolatedDirective, deserializedReport.CspReport.ViolatedDirective);
            Assert.Equal(reportDetails.EffectiveDirective, deserializedReport.CspReport.EffectiveDirective);
            Assert.Equal(reportDetails.OriginalPolicy, deserializedReport.CspReport.OriginalPolicy);
            Assert.Equal(reportDetails.Disposition, deserializedReport.CspReport.Disposition);
            Assert.Equal(reportDetails.BlockedUri, deserializedReport.CspReport.BlockedUri);
            Assert.Equal(reportDetails.StatusCode, deserializedReport.CspReport.StatusCode);
            Assert.Null(deserializedReport.CspReport.ScriptSample);
        }

        #endregion

        #region Helper Classes

        /// <summary>
        /// A custom formatter without any supported media types for testing purposes.
        /// </summary>
        private class CspReportInputFormatterWithoutMediaTypes : CspReportInputFormatter
        {
            public CspReportInputFormatterWithoutMediaTypes()
                : base()
            {
                // Clear supported media types
                SupportedMediaTypes.Clear();
            }
        }

        #endregion
    }
}
