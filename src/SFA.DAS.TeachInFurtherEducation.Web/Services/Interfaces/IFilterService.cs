using SFA.DAS.TeachInFurtherEducation.Contentful.Model.Content;
using SFA.DAS.TeachInFurtherEducation.Web.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.TeachInFurtherEducation.Web.Services.Interfaces
{

    public interface IFilterService
    {
        Task<HomeModel> RemapFilters(string filters, bool isPreview = false);

    }

}
