using MeFriendApi.Domain.Exceptions;

namespace MeFriendApi.Services.Infrastructure
{
    internal static class ServiceOperationExecutor
    {
        internal static async Task<T> ExecuteAsync<T>(
            Func<Task<T>> operation,
            string errorMessage)
        {
            try
            {
                return await operation();
            }
            catch (BadRequestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception($"{errorMessage}: {ex.Message}", ex);
            }
        }
    }
}
