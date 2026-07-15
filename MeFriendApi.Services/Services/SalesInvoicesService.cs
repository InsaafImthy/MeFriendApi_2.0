using MeFriendApi.Domain.Dto.SalesInvoices;
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
            try
            {
                return await _d365CommonService.GetDataFromBc<SalesInvoiceDto>(
                    "/SalesInvoiceHeaders",
                    "?$expand=SalesInvoiceLines",
                    BcWebServiceProtocol.V1);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving sales invoices: {ex.Message}", ex);
            }
        }
    }
}
