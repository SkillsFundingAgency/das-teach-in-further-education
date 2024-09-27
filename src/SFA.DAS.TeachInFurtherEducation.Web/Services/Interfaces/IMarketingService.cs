using SFA.DAS.TeachInFurtherEducation.Web.Models;
using System.Threading.Tasks;

namespace SFA.DAS.TeachInFurtherEducation.Web.Services.Interfaces
{
    public interface IMarketingService
    {
        Task SubscribeUser(NewsLetterSubscriberModel subscriber);
    }
}