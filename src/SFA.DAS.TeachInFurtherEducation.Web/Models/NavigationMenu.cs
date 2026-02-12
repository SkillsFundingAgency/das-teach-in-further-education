using System.Collections.Generic;

namespace SFA.DAS.TeachInFurtherEducation.Web.Models
{

    public class NavigationMenu
    {
        public string? Title { get; set; }
        // TODO - test defaults
        public int SortOrder => 1;
        public List<NavigationMenuItem> MenuItems { get; set; } = new List<NavigationMenuItem>();
    }
}