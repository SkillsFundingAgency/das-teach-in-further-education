using System.Collections.Generic;

namespace SFA.DAS.TeachInFurtherEducation.Web.Models
{
    public class NavigationMenuItem
    {
        public string? Title { get; set; }
        public string? Url { get; set; }

        // TODOS: test defaults
        public bool OpenInNewTab  => false;
        public bool isActive => false;
        public int SortOrder => 1;

        // TODO: ParentItem: Reference (One reference to NavigationMenuItem) - For hierarchy
        // public ref ParentItem<NavigationMenuItem> { get; set; } = new <NavigationMenuItem>();
        
        // TODO: ChildItems: References (Many references to NavigationMenuItem) - Nested children
        public List<NavigationMenuItem> Children { get; set; } = new List<NavigationMenuItem>();
    }
}