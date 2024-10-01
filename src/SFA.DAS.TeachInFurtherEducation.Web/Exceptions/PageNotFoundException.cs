using System;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.TeachInFurtherEducation.Web.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class PageNotFoundException : Exception
    {
        public PageNotFoundException(string message) : base(message) { }
    }
}
