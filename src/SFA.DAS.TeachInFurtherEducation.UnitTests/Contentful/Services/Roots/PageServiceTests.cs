//using AutoFixture;
//using SFA.DAS.TeachInFurtherEducation.Contentful.Model.Api;
//using System.Linq;
//using System.Threading.Tasks;
//using Xunit;
//using SFA.DAS.TeachInFurtherEducation.Contentful.Services.Roots;

//namespace SFA.DAS.TeachInFurtherEducation.UnitTests.Contentful.Services.Roots
//{
//    public class PageServiceTests : RootServiceTestBase<Page, PageService>
//    {
//        public PageService PageService { get; set; }

//        public PageServiceTests()
//        {
//            PageService = new PageService(HtmlRenderer, Logger);
//        }

//        [Fact]
//        public async Task GetAll_SameNumberOfPagesTest()
//        {
//            const int numberOfPages = 3;

//            ContentfulCollection.Items = Fixture.CreateMany<Page>(numberOfPages);

//            var pages = await PageService.GetAll(ContentfulClient);

//            Assert.NotNull(pages);
//            Assert.Equal(numberOfPages, pages.Count());
//        }

//        [Fact]
//        public async Task GetAll_PageTest()
//        {
//            const int numberOfPages = 1;

//            ContentfulCollection.Items = Fixture.CreateMany<Page>(numberOfPages);

//            var pages = await PageService.GetAll(ContentfulClient);

//            var actualPage = pages.FirstOrDefault();
//            Assert.NotNull(actualPage);

//            var expectedSourcePage = ContentfulCollection.Items.First();
//            Assert.Equal(expectedSourcePage.PageTitle, actualPage.Title);
//            Assert.Equal(expectedSourcePage.PageURL, actualPage.Url);
//            Assert.Equal(ExpectedContent.Value, actualPage.Content.Value);
//        }

//        [Fact]
//        public async Task GetAll_NullUrlsFilteredOutTest()
//        {
//            const int numberOfPages = 3;

//            var pages = Fixture.CreateMany<Page>(numberOfPages).ToArray();
//            pages[1].PageURL = null;
//            ContentfulCollection.Items = pages;

//            var pagesResult = await PageService.GetAll(ContentfulClient);

//            Assert.NotNull(pages);
//            Assert.Equal(numberOfPages-1, pagesResult.Count());
//        }

//        [Fact]
//        public async Task GetAll_EmptyUrlsFilteredOutTest()
//        {
//            const int numberOfPages = 3;

//            var pages = Fixture.CreateMany<Page>(numberOfPages).ToArray();
//            pages[0].PageURL = "";
//            pages[2].PageURL = "";
//            ContentfulCollection.Items = pages;

//            var pagesResult = await PageService.GetAll(ContentfulClient);

//            Assert.NotNull(pages);
//            Assert.Single(pagesResult);
//        }
//    }
//}