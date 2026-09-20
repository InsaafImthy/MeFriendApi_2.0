using MeFriendApi.Domain.Exceptions;
using Microsoft.AspNetCore.DataProtection;
using System.Text.Json;

namespace MeFriendApi.Services.Infrastructure;

public sealed class BusinessCentralContinuationTokenService
    : IBusinessCentralContinuationTokenService
{
    private const string ProtectorPurpose =
        "MeFriendApi.BusinessCentral.ContinuationToken.v1";

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly IDataProtector _protector;

    public BusinessCentralContinuationTokenService(IDataProtectionProvider provider)
    {
        _protector = provider.CreateProtector(ProtectorPurpose);
    }

    public string Protect(BusinessCentralContinuationToken token) =>
        _protector.Protect(JsonSerializer.Serialize(token, JsonOptions));

    public BusinessCentralContinuationToken Unprotect(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new BadRequestException("The continuation token is empty.");

        try
        {
            var json = _protector.Unprotect(token);
            return JsonSerializer.Deserialize<BusinessCentralContinuationToken>(json, JsonOptions)
                ?? throw new BadRequestException("The continuation token is invalid.");
        }
        catch (BadRequestException)
        {
            throw;
        }
        catch (Exception)
        {
            throw new BadRequestException("The continuation token is invalid or has expired.");
        }
    }
}
