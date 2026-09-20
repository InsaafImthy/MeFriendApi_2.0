namespace MeFriendApi.Services.Infrastructure;

public interface IBusinessCentralContinuationTokenService
{
    string Protect(BusinessCentralContinuationToken token);
    BusinessCentralContinuationToken Unprotect(string token);
}

public sealed record BusinessCentralContinuationToken(
    string NextLink,
    Guid CompanyId,
    string CompanyName,
    string ApiPath,
    int Protocol,
    string QueryString,
    int PageSize);
