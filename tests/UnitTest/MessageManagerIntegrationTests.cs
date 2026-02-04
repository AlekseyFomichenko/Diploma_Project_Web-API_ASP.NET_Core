using System.Net.Http.Json;
using MessageService.Models.Dto;
using Xunit;

namespace UnitTest
{
    public class MessageManagerIntegrationTests
    {
        private readonly HttpClient _client;

        public MessageManagerIntegrationTests()
        {
            var rsaPath = Path.Combine(AppContext.BaseDirectory, "rsa", "public_key.pem");
            if (File.Exists(rsaPath))
                Environment.SetEnvironmentVariable("Jwt__RsaPublicKeyPath", Path.GetFullPath(rsaPath));
            var factory = new CustomWebApplicationFactory();
            _client = factory.CreateClient();
            var token = TestJwtHelper.CreateToken(userId: 1, role: "User");
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        [Fact]
        public async Task GetMessages_ReturnsOkAndJson()
        {
            var response = await _client.GetAsync("/api/MessageManager/GetMessages");
            response.EnsureSuccessStatusCode();
            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType?.ToString());
        }

        [Fact]
        public async Task GetMessages_ReturnsList()
        {
            var response = await _client.GetAsync("/api/MessageManager/GetMessages");
            response.EnsureSuccessStatusCode();
            var messages = await response.Content.ReadFromJsonAsync<List<MessageDTO>>();
            Assert.NotNull(messages);
        }

        [Fact]
        public async Task SendMessage_ReturnsSuccess()
        {
            var request = new SendMessageRequest { Text = "Test", ReceiverId = 2 };
            var response = await _client.PostAsJsonAsync("/api/MessageManager/SendMessage", request);
            response.EnsureSuccessStatusCode();
        }
    }
}
