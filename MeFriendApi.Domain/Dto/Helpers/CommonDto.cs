namespace MeFriendApi.Domain.Dto.Helpers
{
    public class CommonDto
    {
        public class ResponseModel
        {
            public int StatusCode { get; set; } = 500;
            public bool Status { get; set; } = false;
            public string Message { get; set; } = Messages.DefaultResponse;
            public Guid Id { get; set; } = Guid.Empty;
        }
    }
}
