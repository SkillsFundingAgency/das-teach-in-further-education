using System.Collections.Generic;
using System.Threading.Tasks;
using Contentful.Core;
using SFA.DAS.TeachInFurtherEducation.Contentful.Model.Content;

namespace SFA.DAS.TeachInFurtherEducation.Contentful.Services.Interfaces.Roots
{
    public interface IPageService
    {
        Task<IEnumerable<PageRenamed>> GetAll(IContentfulClient contentfulClient);
    }
}
