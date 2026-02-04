using MessageService.Interfaces;
using MessageService.Models.Dto;
using Xunit;

namespace UnitTest
{
    public class MessageServiceTests
    {
        [Fact]
        public void MockMessageRepo_SendMessage_AddsMessage()
        {
            var repo = new MockMessageRepo();
            var countBefore = repo.Messages.Count;
            var result = repo.SendMessage("New text", 1, 2);
            Assert.Equal(countBefore + 1, result);
            Assert.Contains(repo.Messages, m => m.Text == "New text" && m.SenderId == 1 && m.ReceiverId == 2);
        }

        [Fact]
        public void MockMessageRepo_GetAllMessages_FiltersByReceiver()
        {
            var repo = new MockMessageRepo();
            var forUser1 = repo.GetAllMessages(1);
            Assert.NotNull(forUser1);
            Assert.Single(forUser1);
            Assert.Equal("World", forUser1[0].Text);
            Assert.Equal(2, forUser1[0].SenderId);
            Assert.Equal(1, forUser1[0].ReceiverId);
        }
    }
}
