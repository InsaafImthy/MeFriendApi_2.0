namespace MeFriendApi.Services.Interfaces
{
    public interface IHtmlPdfService
    {
        Task<byte[]> GenerateAsync(string html, CancellationToken cancellationToken);
    }
}
