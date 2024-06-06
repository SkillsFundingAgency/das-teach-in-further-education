using Contentful.Core;

namespace SFA.DAS.TeachInFurtherEducation.Contentful.Services.Interfaces
{
    public interface IContentfulClientFactory
    {
        public IContentfulClient? ContentfulClient { get; }
        public IContentfulClient? PreviewContentfulClient { get; }
    }
}
