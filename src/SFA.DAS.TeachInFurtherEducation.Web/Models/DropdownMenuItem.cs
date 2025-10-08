using SFA.DAS.TeachInFurtherEducation.Contentful.Model.Content;
using SFA.DAS.TeachInFurtherEducation.Contentful.Model.Interim;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace SFA.DAS.TeachInFurtherEducation.Web.Models
{
    [ExcludeFromCodeCoverage]
    public class DropdownMenuItem
    {
        public string? Title { get; set; }
        public string? Url { get; set; }
        public List<DropdownMenuItem> Children { get; set; } = new List<DropdownMenuItem>();
        public bool HasChildren => Children?.Any() == true;
    }
}