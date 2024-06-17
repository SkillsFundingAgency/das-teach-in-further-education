using Contentful.Core.Models;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.TeachInFurtherEducation.Contentful.Model.Interim
{

    [ExcludeFromCodeCoverage]
    public class Preamble
    {

        public string? PreambleTitle { get; set; }

        public Document? PrimarySection { get; set; }

        public Document? SecondarySection { get; set; }

    }

}
