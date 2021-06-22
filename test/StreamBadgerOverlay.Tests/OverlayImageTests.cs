using Bunit;
using Microsoft.Extensions.DependencyInjection;
using StreamBadger.Shared;
using StreamBadgerOverlay.Pages;
using System;
using Xunit;

namespace StreamBadgerOverlay.Tests
{
    public class OverlayImageTests : TestContext
    {
        public OverlayImageTests()
        {
            Services.AddSingleton<ImageStore>();
        }

        [Fact]
        public void Renders()
        {

            var actual = RenderComponent<OverlayImage>(parameters =>
                parameters.Add(i => i.Name, "perry")
                );

            var src = actual.Find("img").GetAttribute("src");
            Assert.Equal("/images/perry", src);
        }
    }
}
