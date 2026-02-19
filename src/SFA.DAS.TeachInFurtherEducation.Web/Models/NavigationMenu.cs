using System.Collections.Generic;

namespace SFA.DAS.TeachInFurtherEducation.Web.Models
{
    public class NavigationMenu
    {
        public string Title { get; set; }
        public IList<NavigationMenuItem> MenuItems { get; set; } = new List<NavigationMenuItem>();
    }
}