using MeFriendApi.Domain.Dto.SalesInvoices;

namespace MeFriendApi.Services.Interfaces
{
    public interface ISalesInvoicesService
    {
        Task<IEnumerable<SalesInvoiceDto>> GetSalesInvoicesAsync();
    }
}
