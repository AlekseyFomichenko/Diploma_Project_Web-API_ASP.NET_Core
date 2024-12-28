using MessageService.Repo;

namespace UnitTest
{
    public class MessageServiceTests
    {
        private readonly MockMessageRepo _mockRepo;
        private readonly MessageRepo _service;

        public MessageServiceTests(MockMessageRepo mockRepo, MessageRepo service)
        {
            _mockRepo = mockRepo;
            _service = service;
        }

        [Fact]
        public void SendMessage_AddsNewMessageAndReturnsItsId()
        {
            // Arrange
            const string text = "Test Message";
            const string userFrom = "John";
            const string userTo = "Jane";

            // Act
            var result = _service.SendMessage(text, userFrom, userTo);

            // Assert
            Assert.Equal(3, result);
            Assert.Contains(_mockRepo._messages, m => m.Id == 3 && m.Text == text && m.UserFrom == userFrom && m.UserTo == userTo);
        }

        [Fact]
        public void GetAllMessages_ReturnsMessagesForSpecificUser()
        {
            // Arrange
            const string userTo = "Alice";

            // Act
            var result = _service.GetAllMessages(userTo);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("World", result[0].Text);
            Assert.Equal("Bob", result[0].UserFrom);
            Assert.Equal("Alice", result[0].UserTo);
        }
    }
}
