using SFA.DAS.TeachInFurtherEducation.Web.Data.Models;
using System.Threading.Tasks;

namespace SFA.DAS.TeachInFurtherEducation.Web.Services.Interfaces
{
    public interface IGeoLocationProvider
    {
        Task<LocationModel?> GetLocationByPostcode(string postcode);
    }
}
