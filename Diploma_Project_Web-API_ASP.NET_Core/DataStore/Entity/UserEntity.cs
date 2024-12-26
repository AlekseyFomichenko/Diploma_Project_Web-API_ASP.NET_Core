namespace Diploma_Project_Web_API_ASP.NET_Core.DataStore.Entity
{
    public class UserEntity
    {
         public Guid Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public UserRole RoleType { get; set; }
        public virtual Role Role { get; set; }
    }
}
