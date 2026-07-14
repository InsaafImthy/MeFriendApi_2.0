using MeFriendApi.Domain.Dto.SalesInvoices;
using MeFriendApi.Services.Interfaces;

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
                return await _d365CommonService.GetFromODataServiceAsync<SalesInvoiceDto>(
                    "MefriendLLP_SalesInvoice");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving sales invoices: {ex.Message}", ex);
            }
        }
    }
}
