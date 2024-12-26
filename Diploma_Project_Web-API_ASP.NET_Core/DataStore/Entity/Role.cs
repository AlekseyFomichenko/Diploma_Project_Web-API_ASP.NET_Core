namespace Diploma_Project_Web_API_ASP.NET_Core.DataStore.Entity
{
    public partial class Role
    {
        public UserRole RoleType { get; set; }
        public string Name { get; set; }
        public virtual List<UserEntity> Users { get; set; }
    }
}
