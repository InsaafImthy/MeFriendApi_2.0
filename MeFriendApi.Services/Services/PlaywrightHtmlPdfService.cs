using MeFriendApi.Services.Interfaces;
using Microsoft.Playwright;

namespace MeFriendApi.Services.Services
{
    public sealed class PlaywrightHtmlPdfService : IHtmlPdfService, IAsyncDisposable
    {
        private readonly SemaphoreSlim _browserLock = new(1, 1);
        private IPlaywright? _playwright;
        private IBrowser? _browser;

        public async Task<byte[]> GenerateAsync(string html, CancellationToken cancellationToken)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(html);

            var browser = await GetBrowserAsync(cancellationToken);
            await using var context = await browser.NewContextAsync();
            var page = await context.NewPageAsync();

            // The invoice HTML is self-contained. Blocking network requests prevents
            // application-generated markup from turning this endpoint into a URL fetcher.
            await page.RouteAsync("**/*", route => route.AbortAsync());
            await page.SetContentAsync(html, new PageSetContentOptions
            {
                WaitUntil = WaitUntilState.Load
            });
            await page.EmulateMediaAsync(new PageEmulateMediaOptions
            {
                Media = Media.Print
            });
            await page.EvaluateAsync(
                "async () => { await document.fonts.ready; await Promise.all(Array.from(document.images).map(image => image.complete ? Promise.resolve() : new Promise(resolve => { image.addEventListener('load', resolve, { once: true }); image.addEventListener('error', resolve, { once: true }); }))); }");

            cancellationToken.ThrowIfCancellationRequested();

            return await page.PdfAsync(new PagePdfOptions
            {
                Format = "A4",
                PreferCSSPageSize = true,
                PrintBackground = true
            });
        }

        private async Task<IBrowser> GetBrowserAsync(CancellationToken cancellationToken)
        {
            if (_browser?.IsConnected == true)
            {
                return _browser;
            }

            await _browserLock.WaitAsync(cancellationToken);
            try
            {
                if (_browser?.IsConnected == true)
                {
                    return _browser;
                }

                _playwright ??= await Playwright.CreateAsync();
                _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
                {
                    Headless = true
                });

                return _browser;
            }
            finally
            {
                _browserLock.Release();
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_browser is not null)
            {
                await _browser.DisposeAsync();
            }

            _playwright?.Dispose();
            _browserLock.Dispose();
        }
    }
}
