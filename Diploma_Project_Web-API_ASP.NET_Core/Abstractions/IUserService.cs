using Diploma_Project_Web_API_ASP.NET_Core.DataStore.Entity;

namespace Diploma_Project_Web_API_ASP.NET_Core.Abstractions
{
    public interface IUserService
    {
        public Guid  UserAdd(string username, string password, UserRole roleId);
        public string CheckRole(string username, string password);
    }
}
