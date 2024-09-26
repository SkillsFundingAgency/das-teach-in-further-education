using Contentful.Core.Models;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.TeachInFurtherEducation.Contentful.Model.Interim
{

    [ExcludeFromCodeCoverage]
    public class HeaderFullImage : Common, IContent
    {
        public required string Title { get; set; }

        public required Asset HeaderImage { get; set; }
    }
}
