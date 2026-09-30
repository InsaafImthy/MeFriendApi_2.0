using MeFriendApi.Domain.Dto.SalesInvoices;

namespace MeFriendApi.Services.Interfaces
{
    public interface ISalesInvoicesService
    {
        Task<MeFriendApi.Domain.Dto.Paging.PagedResult<SalesInvoiceDto>> GetSalesInvoicesAsync(
            MeFriendApi.Domain.Dto.Paging.PagedRequest request);
        Task<SalesInvoiceDto?> GetSalesInvoiceAsync(string id);
    }
}
