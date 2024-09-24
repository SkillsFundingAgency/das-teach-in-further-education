using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.TeachInFurtherEducation.Contentful.Model.Content.Interfaces;
using SFA.DAS.TeachInFurtherEducation.Contentful.Model.Interim;

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

        Page? GetPreviewPageByURL(string url);

        Page? GetPageByURL(string url);

        Task<List<Asset<byte[]>>> GetAssetsByTags(params string[] tags);

    }
}
