using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.TeachInFurtherEducation.Contentful.Model.Content.Navigation;
using SFA.DAS.TeachInFurtherEducation.Contentful.Services.Navigation;
using SFA.DAS.TeachInFurtherEducation.Web.Models;

namespace SFA.DAS.TeachInFurtherEducation.Contentful.Services.Interfaces
{
    internal interface INavigationTreeBuilder
    {
        IReadOnlyList<NavigationMenuTreeItem> BuildNavigationTree(IReadOnlyList<NavigationMenuItem> items);
    }
}
