using MeFriendApi.Domain.Dto.Documents;
using MeFriendApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MeFriendApi.Controllers
{
    [Route("api/documents")]
    [ApiController]
    public sealed class DocumentsController : ApiControllerBase
    {
        private readonly IHtmlPdfService _htmlPdfService;

        public DocumentsController(
            IHtmlPdfService htmlPdfService,
            ILogger<ApiControllerBase> logger)
            : base(logger)
        {
            _htmlPdfService = htmlPdfService;
        }

        [HttpPost("pdf")]
        [Consumes("application/json")]
        [Produces("application/pdf")]
        [RequestSizeLimit(HtmlToPdfRequest.MaxHtmlLength + 50_000)]
        public Task<IActionResult> GeneratePdf(
            [FromBody] HtmlToPdfRequest request,
            CancellationToken cancellationToken) =>
            ExecuteAsync(async () =>
            {
                if (ContainsExecutableContent(request.Html))
                {
                    return BadRequest("Executable HTML content is not allowed in PDF documents.");
                }

                var pdf = await _htmlPdfService.GenerateAsync(request.Html, cancellationToken);
                return File(pdf, "application/pdf", SanitizeFileName(request.FileName));
            });

        private static bool ContainsExecutableContent(string html) =>
            html.Contains("<script", StringComparison.OrdinalIgnoreCase) ||
            html.Contains("javascript:", StringComparison.OrdinalIgnoreCase) ||
            html.Contains("<iframe", StringComparison.OrdinalIgnoreCase) ||
            html.Contains("<object", StringComparison.OrdinalIgnoreCase) ||
            html.Contains("<embed", StringComparison.OrdinalIgnoreCase);

        private static string SanitizeFileName(string fileName)
        {
            var name = Path.GetFileName(fileName);
            foreach (var invalidCharacter in Path.GetInvalidFileNameChars())
            {
                name = name.Replace(invalidCharacter, '-');
            }

            if (!name.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            {
                name += ".pdf";
            }

            return name;
        }
    }
}
