using MeFriendApi.Domain.Dto.SalesInvoices;
using MeFriendApi.Services.Infrastructure;
using MeFriendApi.Services.Interfaces;
using static MeFriendApi.Domain.Constants;

namespace MeFriendApi.Services.Services
{
    public class SalesInvoicesService : ISalesInvoicesService
    {
        private readonly ID365CommonService _d365CommonService;

        public SalesInvoicesService(ID365CommonService d365CommonService)
        {
            _d365CommonService = d365CommonService;
        }

        public async Task<MeFriendApi.Domain.Dto.Paging.PagedResult<SalesInvoiceDto>> GetSalesInvoicesAsync(
            MeFriendApi.Domain.Dto.Paging.PagedRequest request)
        {
            var query = BusinessCentralListQueries.SalesInvoices(request);
            return await ServiceOperationExecutor.ExecuteAsync(
                () => _d365CommonService.GetPagedDataFromBc<SalesInvoiceDto>(
                    BusinessCentralDefaults.ApiPaths.SalesInvoices,
                    request.PageSize,
                    request.ContinuationToken,
                    query,
                    BcWebServiceProtocol.V1),
                "Error retrieving sales invoices");
        }

        public async Task<SalesInvoiceDto?> GetSalesInvoiceAsync(string id)
        {
            var query = ODataQueryBuilder.BuildSingleFilter(
                "invoiceNo",
                id,
                expand: "SalesInvoiceLines");

            return await ServiceOperationExecutor.ExecuteAsync(
                () => _d365CommonService.GetSingleDataFromBc<SalesInvoiceDto>(
                    BusinessCentralDefaults.ApiPaths.SalesInvoices,
                    query,
                    BcWebServiceProtocol.V1),
                "Error retrieving sales invoice");
        }
    }
}
