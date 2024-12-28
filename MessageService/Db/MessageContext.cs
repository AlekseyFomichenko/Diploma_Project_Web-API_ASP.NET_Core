using MessageService.Models;
using Microsoft.EntityFrameworkCore;

namespace MessageService.Db
{
    public partial class MessageContext : DbContext
    {
        private readonly string _connectingString;
        public MessageContext(string connectionString)
        {
            _connectingString = connectionString;
        }
        public DbSet<Message> Messages { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder
        .LogTo(Console.WriteLine)
        .UseNpgsql(_connectingString);

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("users_pkey");
                entity.HasIndex(e => e.Name).IsUnique();

                entity.ToTable("users");

                entity.Property(e => e.Id).HasColumnName("id");
                
                entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");

                entity.Property(e => e.Email).HasColumnName("email");

                entity.Property(e => e.Password).HasColumnName("password");
                
                entity.Property(e => e.Salt).HasColumnName("salt");

                entity.Property(e => e.Role).HasConversion<int>();
            });
            modelBuilder.Entity<Message>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("messages_pkey");
                entity.HasIndex(e => e.Id).IsUnique();

                entity.ToTable("messages");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.SenderMail)
                .HasColumnName("senderMail");

                entity.Property(e => e.ReceiverMail).HasColumnName("receiverMail");

                entity.Property(e => e.Text).HasColumnName("text");

                entity.Property(e => e.IsRead).HasDefaultValue(false).HasColumnName("isRead");
            });
            OnModelCreatingPartial(modelBuilder);
        }
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
