using MeFriendApi.Domain.Dto.SalesOrders;
using MeFriendApi.Services.Interfaces;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using static MeFriendApi.Domain.Constants;

namespace MeFriendApi.Services.Services
{
    public class SalesOrdersService : ISalesOrdersService
    {
        private readonly ID365CommonService _d365CommonService;

        public SalesOrdersService(ID365CommonService d365CommonService)
        {
            _d365CommonService = d365CommonService;
        }

        public async Task<IEnumerable<SalesOrderDto>> GetSalesOrdersAsync()
        {
            try
            {
                return await _d365CommonService.GetDataFromBc<SalesOrderDto>(
                    "/SalesOrderHeaders",
                    "?$expand=SalesOrderLines",
                    BcWebServiceProtocol.V1);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving sales orders: {ex.Message}", ex);
            }
        }

        public async Task<SalesOrderPostResponse?> CreateSalesOrderAsync(CreateSalesOrderRequest request)
        {
            try
            {
                var encodedRequest = CreateBase64Request(request);

                return await _d365CommonService.PostToODataServiceAsync<SalesOrderBase64Request, SalesOrderPostResponse>(
                    "MefriendLLP_SalesOrder",
                    encodedRequest);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating sales order: {ex.Message}", ex);
            }
        }

        private static SalesOrderBase64Request CreateBase64Request(CreateSalesOrderRequest request)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            var json = JsonSerializer.Serialize(request, options);
            var base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(json));

            return new SalesOrderBase64Request
            {
                Base64 = base64
            };
        }
    }
}
