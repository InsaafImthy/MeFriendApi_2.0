using MeFriendApi.Domain.Exceptions;
using Microsoft.AspNetCore.Http;

namespace MeFriendApi.Services.Infrastructure;

public sealed class BusinessCentralCompanyContext : IBusinessCentralCompanyContext
{
    private readonly string[] _companyIdValues;
    private readonly string[] _companyNameValues;

    public BusinessCentralCompanyContext(IHttpContextAccessor httpContextAccessor)
    {
        var headers = httpContextAccessor.HttpContext?.Request.Headers;

        _companyIdValues = GetHeaderValues(headers, BusinessCentralDefaults.HeaderNames.CompanyId);
        _companyNameValues = GetHeaderValues(headers, BusinessCentralDefaults.HeaderNames.CompanyName);
    }

    public Guid CompanyId
    {
        get
        {
            var companyIdValue = GetRequiredSingleValue(
                _companyIdValues,
                BusinessCentralDefaults.HeaderNames.CompanyId,
                "for this Business Central operation");

            if (!Guid.TryParse(companyIdValue, out var companyId))
            {
                throw new BadRequestException(
                    $"The {BusinessCentralDefaults.HeaderNames.CompanyId} header must contain a valid Business Central company GUID.");
            }

            return companyId;
        }
    }

    public string CompanyName
    {
        get
        {
            return GetRequiredSingleValue(
                _companyNameValues,
                BusinessCentralDefaults.HeaderNames.CompanyName,
                "for this Business Central OData operation");
        }
    }

    private static string[] GetHeaderValues(IHeaderDictionary? headers, string headerName)
    {
        if (headers == null || !headers.TryGetValue(headerName, out var values) || values.Count == 0)
            return [];

        return values
            .Where(value => value != null)
            .Select(value => value!)
            .ToArray();
    }

    private static string GetRequiredSingleValue(
        string[] values,
        string headerName,
        string requirementContext)
    {
        if (values.Length > 1)
        {
            throw new BadRequestException(
                $"The {headerName} header must be supplied exactly once.");
        }

        if (values.Length == 0 || string.IsNullOrWhiteSpace(values[0]))
        {
            throw new BadRequestException(
                $"The {headerName} header is required {requirementContext}.");
        }

        return values[0];
    }
}
