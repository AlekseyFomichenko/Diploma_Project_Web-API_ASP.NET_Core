namespace UnitTest
{
    public class ValuesControllerTests
    {
        private readonly HttpClient _client;

        public ValuesControllerTests()
        {
            var webAppFactory = new CustomWebApplicationFactory();
            _client = webAppFactory.CreateClient();
        }

        [Fact]
        public async Task Get_ReturnsOkResult()
        {
            // Act
            var response = await _client.GetAsync("/api/values");

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }

        [Theory]
        [InlineData("/api/values/1")]
        [InlineData("/api/values/2")]
        public async Task GetById_ReturnsOkResult(string url)
        {
            // Act
            var response = await _client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }
    }
}
