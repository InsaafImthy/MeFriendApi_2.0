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

        public async Task<IEnumerable<SalesInvoiceDto>> GetSalesInvoicesAsync()
        {
            return await ServiceOperationExecutor.ExecuteAsync(
                () => _d365CommonService.GetDataFromBc<SalesInvoiceDto>(
                    BusinessCentralDefaults.ApiPaths.SalesInvoices,
                    BusinessCentralDefaults.Queries.SalesInvoicesWithLines,
                    BcWebServiceProtocol.V1,
                    BusinessCentralDefaults.ApiServiceNames.SalesInvoices),
                "Error retrieving sales invoices");
        }
    }
}
