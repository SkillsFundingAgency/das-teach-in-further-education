using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.TeachInFurtherEducation.Web.Models
{
    [ExcludeFromCodeCoverage]
    public sealed class NavigationMenuTree
    {
        public string MenuTitle { get; init; } = string.Empty;
        public IReadOnlyList<NavigationMenuTreeItem> Items { get; init; } = Array.Empty<NavigationMenuTreeItem>();
    }
}
