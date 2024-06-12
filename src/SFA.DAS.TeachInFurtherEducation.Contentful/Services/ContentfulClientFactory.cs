using System;
using Contentful.Core;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.TeachInFurtherEducation.Contentful.Services.Interfaces;

namespace SFA.DAS.TeachInFurtherEducation.Contentful.Services
{
    public class ContentfulClientFactory : IContentfulClientFactory
    {
        public IContentfulClient? ContentfulClient { get; }
        public IContentfulClient? PreviewContentfulClient { get; }

        public ContentfulClientFactory(IEnumerable<IContentfulClient> contentfulClients)
        {
            if (!contentfulClients.Any())
                throw new ArgumentException("Enumeration empty", nameof(contentfulClients));

            ContentfulClient = contentfulClients.SingleOrDefault(c => !c.IsPreviewClient);
            PreviewContentfulClient = contentfulClients.SingleOrDefault(c => c.IsPreviewClient);
        }
    }
}
