using Contentful.Core.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.TeachInFurtherEducation.Contentful.Model.Content.Navigation
{
    [ExcludeFromCodeCoverage]
    public class NavigationMenu
    {
        public SystemProperties Sys { get; set; } = default!;
        public string Title { get; set; } = string.Empty;
        public List<NavigationMenuItem> MenuItems { get; set; } = new();
    }
}