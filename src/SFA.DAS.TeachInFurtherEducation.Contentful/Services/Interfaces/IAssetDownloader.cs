using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.TeachInFurtherEducation.Contentful.Services.Interfaces
{
    public interface IAssetDownloader
    {
        Task<byte[]?> DownloadAssetContentAsync(string assetUrl);
    }
}
