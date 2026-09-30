using System.ComponentModel.DataAnnotations;

namespace MeFriendApi.Domain.Dto.Documents
{
    public sealed class HtmlToPdfRequest
    {
        public const int MaxHtmlLength = 2_000_000;

        [Required]
        [StringLength(MaxHtmlLength, MinimumLength = 1)]
        public required string Html { get; init; }

        [Required]
        [StringLength(180, MinimumLength = 5)]
        public required string FileName { get; init; }
    }
}
