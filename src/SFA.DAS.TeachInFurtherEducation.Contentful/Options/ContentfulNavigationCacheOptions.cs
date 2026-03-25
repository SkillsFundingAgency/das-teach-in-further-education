using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.TeachInFurtherEducation.Contentful.Options
{
    [ExcludeFromCodeCoverage]
    public sealed class ContentfulNavigationCacheOptions
    {
        public required int HeaderMenuCacheTTLMinutes { get; set; }
    }
}