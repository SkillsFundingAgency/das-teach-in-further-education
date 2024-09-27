using System;

namespace SFA.DAS.TeachInFurtherEducation.Web.Exceptions
{
    public class PageNotFoundException : Exception
    {
        public PageNotFoundException(string message) : base(message) { }
    }
}
