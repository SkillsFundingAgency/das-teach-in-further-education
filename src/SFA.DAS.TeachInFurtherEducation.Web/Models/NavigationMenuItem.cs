using System.Collections.Generic;

namespace SFA.DAS.TeachInFurtherEducation.Web.Models
{
    public class NavigationMenuItem
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public bool OpenInNewTab  { get; set; }
        public IList<NavigationMenuItem> Children { get; set; } = new List<NavigationMenuItem>();
        public bool HasChildren => Children != null && Children.Count > 0;
        public bool IsDropdown => HasChildren;
    }
}