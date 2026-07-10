namespace MeFriendApi.Domain.DTO
{
    public class BcAttachmentResponseDto
    {
        public string? Id { get; set; }
        public string? FileName { get; set; }
        public string? FileUrl { get; set; }
        public string? MediaId { get; set; }
        public long? FileSize { get; set; }
    }
}
