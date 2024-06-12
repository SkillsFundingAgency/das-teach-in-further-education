using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace SFA.DAS.TeachInFurtherEducation.Contentful.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class ContentServiceException : Exception
    {
        public ContentServiceException()
        {
        }

#pragma warning disable S1133 // Deprecated code should be removed
        [Obsolete("The base constructor is marked obsolete", DiagnosticId = "SYSLIB0051")]
#pragma warning restore S1133 // Deprecated code should be removed
        protected ContentServiceException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public ContentServiceException(string? message) : base(message)
        {
        }

        public ContentServiceException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
