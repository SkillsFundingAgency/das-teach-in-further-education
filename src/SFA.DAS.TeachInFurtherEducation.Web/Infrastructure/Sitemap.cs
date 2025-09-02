using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using AspNetCore.SEOHelper.Sitemap;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using SFA.DAS.TeachInFurtherEducation.Contentful.Services.Interfaces;

namespace SFA.DAS.TeachInFurtherEducation.Web.Infrastructure
{
    //todo: code is untestable as is. need to refactor to introduce a level of indirection to CreateSitemapXML
    [ExcludeFromCodeCoverage]
    public class Sitemap : ISitemap
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IOptions<EndpointsOptions> _endpointsOptions;
        private readonly IContentService _contentService;

        public Sitemap(
            IWebHostEnvironment webHostEnvironment,
            IOptions<EndpointsOptions> endpointsOptions,
            IContentService contentService)
        {
            _webHostEnvironment = webHostEnvironment;
            _endpointsOptions = endpointsOptions;
            _contentService = contentService;
            contentService.ContentUpdated += OnContentUpdated;
        }

        private void OnContentUpdated(object? sender, EventArgs args)
        {
            Generate();
        }

        public void Generate()
        {
            if (!Uri.TryCreate(_endpointsOptions.Value.BaseURL, UriKind.Absolute, out Uri? baseUri))
                return;

            var nodes = new List<SitemapNode>();

            var content = _contentService.Content;
            
            nodes.AddRange(content.Pagess.Select(x => new SitemapNode
            {
                Priority = 1.0,
                Frequency = SitemapFrequency.Weekly,
                Url = new Uri(baseUri, string.Concat("", x.Url.StartsWith('/') ? x.Url : $"/{x.Url}"))
                            .AbsoluteUri
            }));
            
            foreach (var n in nodes)  {
                n.Url = n.Url.Replace("page/", "");
            }

            SitemapNode? home = nodes.Find(x => x.Url.EndsWith($"/{RouteNames.Home}"));
            if (home != null)
                home.Url = home.Url.Replace("home", "");

            new SitemapDocument().CreateSitemapXML(nodes, _webHostEnvironment.ContentRootPath);
        }
    }
}
