namespace Diploma_Project_Web_API_ASP.NET_Core.DataStore.Entity
{
    public partial class Role
    {
        public UseRole Id { get; set; }
        public string Name { get; set; }
        public List<UserEntity> Users { get; set; }
    }
}
