namespace MeFriendApi.Services.Infrastructure;

public interface IBusinessCentralCompanyContext
{
    Guid CompanyId { get; }
    string CompanyName { get; }
}
