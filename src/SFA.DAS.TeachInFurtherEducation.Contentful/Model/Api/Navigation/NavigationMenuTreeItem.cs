using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.TeachInFurtherEducation.Web.Models
{
    [ExcludeFromCodeCoverage]
    public sealed class NavigationMenuTreeItem
    {
        public string Id { get; init; } = string.Empty;
        public string Title { get; init; } = string.Empty;
        public string Url { get; init; } = string.Empty;
        public bool OpenInNewTab { get; init; }
        public bool Enabled { get; init; }
        public int SortOrder { get; init; }
        public List<NavigationMenuTreeItem> Children { get; init; } = new();
    }
}
