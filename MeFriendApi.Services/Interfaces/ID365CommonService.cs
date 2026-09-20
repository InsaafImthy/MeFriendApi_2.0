using MeFriendApi.Domain.DTO;
using Microsoft.AspNetCore.Http;
using static MeFriendApi.Domain.Constants;

namespace MeFriendApi.Services.Interfaces
{
    public interface ID365CommonService
    {
        Task<string> GetAccessToken();
        Task<MeFriendApi.Domain.Dto.Paging.PagedResult<T>> GetPagedDataFromBc<T>(
            string apiPath,
            int? pageSize = null,
            string? continuationToken = null,
            string? queryString = null,
            BcWebServiceProtocol? bcWebServiceProtocol = BcWebServiceProtocol.V2);
        Task<T?> GetSingleDataFromBc<T>(
            string apiPath,
            string? queryString = null,
            BcWebServiceProtocol? bcWebServiceProtocol = BcWebServiceProtocol.V2);
        Task<TResponse?> PostDataToBc<TRequest, TResponse>(string apiPath, TRequest payload, BcWebServiceProtocol? bcWebServiceProtocol = BcWebServiceProtocol.V1);
        Task<MeFriendApi.Domain.Dto.Paging.PagedResult<T>> GetPagedFromODataServiceAsync<T>(
            string serviceName,
            int? pageSize = null,
            string? continuationToken = null,
            string? queryString = null);
        Task<TResponse?> PostToODataServiceAsync<TRequest, TResponse>(string serviceName, TRequest payload, string? queryString = null);
        Task<TResponse?> PatchDataToBc<TRequest, TResponse>( string apiPath, TRequest payload, string? etag = null, BcWebServiceProtocol? bcWebServiceProtocol = BcWebServiceProtocol.V1);
        Task DeleteDataFromBc( string apiPath, string? etag = null, BcWebServiceProtocol? bcWebServiceProtocol = BcWebServiceProtocol.V1);
        Task<BcAttachmentResponseDto?> UploadAttachmentsToBcAsync(
            string filePathWithItemId,
            IFormFile file,
            BcWebServiceProtocol? bcWebServiceProtocol = BcWebServiceProtocol.V1);
    }
}
