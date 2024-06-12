using System;
using System.Threading.Tasks;
using SFA.DAS.TeachInFurtherEducation.Contentful.Model.Content.Interfaces;

namespace SFA.DAS.TeachInFurtherEducation.Contentful.Services.Interfaces
{
    public interface IContentService
    {
        event EventHandler<EventArgs>? ContentUpdated;
        event EventHandler<EventArgs>? PreviewContentUpdated;

        IContent Content { get; }
        IContent? PreviewContent { get; }

        Task<IContent> Update();
        Task<IContent> UpdatePreview();
    }
}
