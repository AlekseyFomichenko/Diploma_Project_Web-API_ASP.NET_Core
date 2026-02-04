namespace UserService.Db
{
    public partial class User
    {
        public int Id { get; set; }

        public string Email { get; set; } = null!;

        public string? Name { get; set; }

        public byte[] Password { get; set; } = null!;

        public byte[] Salt { get; set; } = null!;

        public RoleId RoleId { get; set; }

        public virtual Role Role { get; set; } = null!;
    }
}
