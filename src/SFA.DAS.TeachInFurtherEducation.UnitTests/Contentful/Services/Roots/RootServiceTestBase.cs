using AutoFixture;
using Contentful.Core.Models;
using Contentful.Core;
using System.Collections.Generic;
using FakeItEasy;
using Contentful.Core.Search;
using System.Threading;
using System;
using Microsoft.AspNetCore.Html;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace SFA.DAS.TeachInFurtherEducation.UnitTests.Contentful.Services.Roots
{
    public class RootServiceTestBase<TApiModel, TService>
    {
        public Fixture Fixture { get; }
        public Document Document { get; set; }
        public HtmlString ExpectedContent { get; set; }
        public IContentfulClient ContentfulClient { get; set; }
        public HtmlRenderer HtmlRenderer { get; set; }
        public ILogger<TService> Logger { get; set; }
        public ContentfulCollection<TApiModel> ContentfulCollection { get; set; }

        public RootServiceTestBase()
        {
            Fixture = new Fixture();

            Fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()

                .ForEach(b => Fixture.Behaviors.Remove(b));

            Fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            (Document, ExpectedContent) = SampleDocumentAndExpectedContent();

            Fixture.Inject(Document);

            ContentfulClient = A.Fake<IContentfulClient>();

            HtmlRenderer = A.Fake<HtmlRenderer>();
            Logger = A.Fake<ILogger<TService>>();

            ContentfulCollection = new ContentfulCollection<TApiModel> { Items = Array.Empty<TApiModel>() };
            SetupContentfulClientCall(ContentfulClient, ContentfulCollection);
        }

        public (Document, HtmlString) SampleDocumentAndExpectedContent(int differentiator = 0)
        {
            return (new Document
            {
                NodeType = "heading-2",
                Data = new GenericStructureData(),
                Content = new List<IContent>
                {
                    new Heading2
                    {
                        Content = new List<IContent> {new Text {Value = $"Gobble{differentiator}"}}
                    }
                }
            }, new HtmlString($"<h2>Gobble{differentiator}</h2>"));
        }

        public void SetupContentfulClientCall<TApiModelSetup>(
            IContentfulClient contentfulClient,
            ContentfulCollection<TApiModelSetup> returnCollection)
        {
            A.CallTo(() => contentfulClient.GetEntries(A<QueryBuilder<TApiModelSetup>>.Ignored, A<CancellationToken>.Ignored))
                .Returns(returnCollection);
        }
    }
}