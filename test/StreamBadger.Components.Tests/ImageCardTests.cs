using AngleSharp.Html.Dom;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using StreamBadger.Clients;
using System;
using Xunit;
using Xunit.Abstractions;

namespace StreamBadger.Components.Tests
{
    public class ImageCardTests : TestContext
    {
        private readonly IServerClient _serverClient;
        private readonly ITestOutputHelper _testOutputHelper;

        public ImageCardTests(ITestOutputHelper testOutputHelper)
        {
            _serverClient = Substitute.For<IServerClient>();
            Services.AddSingleton(_serverClient);
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void ClickCallsServerClient()
        {
            var actual = RenderComponent<ImageCard>(parameters =>
                parameters.Add(i => i.ImageName, "perry")
                );

            var button = actual.Find("button");
            Assert.NotNull(button);
            button.Click();

            _serverClient.Received().ShowImage("perry");
        }
    }
}
