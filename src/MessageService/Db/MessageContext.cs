using MessageService.Models;
using Microsoft.EntityFrameworkCore;

namespace MessageService.Db
{
    public partial class MessageContext : DbContext
    {
        public MessageContext(DbContextOptions<MessageContext> options) : base(options) { }

        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Message>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("messages_pkey");
                entity.ToTable("messages");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.SenderId).HasColumnName("senderId");
                entity.Property(e => e.ReceiverId).HasColumnName("receiverId");
                entity.Property(e => e.Text).HasColumnName("text");
                entity.Property(e => e.IsRead).HasDefaultValue(false).HasColumnName("isRead");
            });
            OnModelCreatingPartial(modelBuilder);
        }
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
