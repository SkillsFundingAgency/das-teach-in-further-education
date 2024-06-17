#if bootstrapping
// this class can be used to provide a valid GeneratedContent class when changing the code generator

using System;
using System.Collections.Generic;
using SFA.DAS.TeachInFurtherEducation.Contentful.Model.Content;
using SFA.DAS.TeachInFurtherEducation.Contentful.Model.Content.Interfaces;

namespace SFA.DAS.TeachInFurtherEducation.Contentful.Content
{
    public class GeneratedContent : IContent
    {
        public IEnumerable<Page> Pages { get; } = Array.Empty<Page>();
        public IEnumerable<CaseStudyPage> CaseStudyPages { get; } = Array.Empty<CaseStudyPage>();
        public IEnumerable<Scheme> Schemes { get; } = Array.Empty<Scheme>();
        public Filter MotivationsFilter { get; } = new Filter(null!, null!, null!);
        public Filter PayFilter { get; } = new Filter(null!, null!, null!);
        public Filter SchemeLengthFilter { get; } = new Filter(null!, null!, null!);
    }
}
#endif