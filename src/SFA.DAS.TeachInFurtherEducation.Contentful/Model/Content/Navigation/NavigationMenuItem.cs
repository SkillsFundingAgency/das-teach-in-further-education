using Contentful.Core.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.TeachInFurtherEducation.Contentful.Model.Content.Navigation
{
    [ExcludeFromCodeCoverage]
    public class NavigationMenuItem
    {
        public SystemProperties Sys { get; set; } = default!;
        public string Title {  get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public bool OpenInNewTab { get; set; }
        public bool Enabled { get; set; } 
        public int SortOrder { get; set; }
        public NavigationMenuItem? ParentItem { get; set; }
    }
}
