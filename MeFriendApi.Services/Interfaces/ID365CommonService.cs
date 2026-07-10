using MeFriendApi.Domain.DTO;
using Microsoft.AspNetCore.Http;
using static MeFriendApi.Domain.Constants;

namespace MeFriendApi.Services.Interfaces
{
    public interface ID365CommonService
    {
        Task<string> GetAccessToken();
        Task<List<T>> GetDataFromBc<T>(string apiPath, string? filter = "", BcWebServiceProtocol? bcWebServiceProtocol = BcWebServiceProtocol.V2);
        Task<TResponse?> PostDataToBc<TRequest, TResponse>(string apiPath, TRequest payload, BcWebServiceProtocol? bcWebServiceProtocol = BcWebServiceProtocol.V1);
        Task<TResponse?> PatchDataToBc<TRequest, TResponse>( string apiPath, TRequest payload, string? etag = null, BcWebServiceProtocol? bcWebServiceProtocol = BcWebServiceProtocol.V1);
        Task DeleteDataFromBc( string apiPath, string? etag = null, BcWebServiceProtocol? bcWebServiceProtocol = BcWebServiceProtocol.V1);
        Task<BcAttachmentResponseDto?> UploadAttachmentsToBcAsync(
            string filePathWithItemId,
            IFormFile file,
            BcWebServiceProtocol? bcWebServiceProtocol = BcWebServiceProtocol.V1);
    }
}

