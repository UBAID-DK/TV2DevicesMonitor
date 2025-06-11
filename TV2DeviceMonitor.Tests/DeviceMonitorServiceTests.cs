using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using RichardSzalay.MockHttp;
using TV2DeviceMonitor.Core.Interfaces;
using TV2DeviceMonitor.Core.Models;
using TV2DeviceMonitor.Core.Services;
using Xunit;
using System.Net; // Add this for HttpStatusCode

namespace TV2DeviceMonitor.Tests
{
    public class BlackmagicApiClientTests
    {

        [Fact]
        public async Task GetDeviceStateAsync_ReturnsValidState()
        {
            // Arrange
            var mockHttp = new MockHttpMessageHandler();
            var ipAddress = "192.168.1.100";
            var apiEndpoint = $"http://{ipAddress}/control/api/v1/livestreams/0";

            // Use exact JSON structure with proper formatting
            var jsonResponse = @"{
                ""status"": ""On"",
                ""bitrate"": 7500000,
                ""bufferPct"": 30
            }";

            mockHttp.When(apiEndpoint)
                   .Respond("application/json", jsonResponse);

            var httpClient = mockHttp.ToHttpClient();
            var loggerMock = new Mock<ILogger<ApiClient>>();
            var apiClient = new ApiClient(httpClient, loggerMock.Object);

            // Act
            var state = await apiClient.GetDeviceStateAsync(ipAddress);

            // Assert
            Assert.NotNull(state);
            Assert.Equal("On", state!.Status);
            Assert.Equal(7500000, state.Bitrate);
            Assert.Equal(30, state.BufferPct);
        }

        [Fact]
        public async Task GetDeviceStateAsync_HandlesInvalidResponse()
        {
            // Arrange
            var mockHttp = new MockHttpMessageHandler();
            var ipAddress = "192.168.1.100";

            mockHttp.When($"http://{ipAddress}/control/api/v1/livestreams/0")
                   .Respond(HttpStatusCode.InternalServerError);

            var httpClient = mockHttp.ToHttpClient();
            var loggerMock = new Mock<ILogger<ApiClient>>();
            var apiClient = new ApiClient(httpClient, loggerMock.Object);

            // Act
            var state = await apiClient.GetDeviceStateAsync(ipAddress);

            // Assert
            Assert.Null(state);

            loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, _) => v.ToString().Contains(ipAddress)),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }
    }
}