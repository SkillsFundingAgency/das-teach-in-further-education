using System.Net.Http;
using System.Threading.Tasks;

namespace SFA.DAS.TeachInFurtherEducation.Web.Interfaces
{
    public interface IHttpClientWrapper
    {
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage request);
    }
}
