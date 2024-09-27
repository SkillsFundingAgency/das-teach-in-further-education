using SFA.DAS.TeachInFurtherEducation.Contentful.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.TeachInFurtherEducation.Contentful.Services
{
    [ExcludeFromCodeCoverage]
    public class AssetDownloader : IAssetDownloader
    {
        private readonly HttpClient _httpClient;

        public AssetDownloader(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<byte[]?> DownloadAssetContentAsync(string assetUrl)
        {
            if (!string.IsNullOrEmpty(assetUrl))
            {
                return await _httpClient.GetByteArrayAsync($"https:{assetUrl}");
            }

            return null;
        }
    }

}
