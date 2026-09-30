using MeFriendApi.Domain.Dto.SalesInvoices;
using MeFriendApi.Domain.Exceptions;
using MeFriendApi.Services.Infrastructure;
using MeFriendApi.Services.Interfaces;
using static MeFriendApi.Domain.Constants;

namespace MeFriendApi.Services.Services
{
    public class SalesInvoicesService : ISalesInvoicesService
    {
        private const int MaxInvoiceNumberLength = 20;
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

        public async Task<SalesInvoiceDto?> GetSalesInvoiceAsync(
            string invoiceNumber,
            string? salespersonCode = null)
        {
            var normalizedInvoiceNumber = ValidateInvoiceNumber(invoiceNumber);
            var query = ODataQueryBuilder.BuildSingleFilter(
                "invoiceNo",
                normalizedInvoiceNumber,
                expand: "SalesInvoiceLines",
                scopeField: "salesPerson",
                scopeValue: salespersonCode);

            return await ServiceOperationExecutor.ExecuteAsync(
                () => _d365CommonService.GetSingleDataFromBc<SalesInvoiceDto>(
                    BusinessCentralDefaults.ApiPaths.SalesInvoices,
                    query,
                    BcWebServiceProtocol.V1),
                "Error retrieving sales invoice");
        }

        private static string ValidateInvoiceNumber(string invoiceNumber)
        {
            if (string.IsNullOrWhiteSpace(invoiceNumber))
                throw new BadRequestException("Invoice number is required.");

            var normalizedInvoiceNumber = invoiceNumber.Trim();

            if (Guid.TryParse(normalizedInvoiceNumber, out _))
            {
                throw new BadRequestException(
                    "Invoice detail requires an invoice number, not a Business Central system ID.");
            }

            if (normalizedInvoiceNumber.Length > MaxInvoiceNumberLength)
            {
                throw new BadRequestException(
                    $"Invoice number cannot exceed {MaxInvoiceNumberLength} characters.");
            }

            return normalizedInvoiceNumber;
        }
    }
}
