using System.Security.Cryptography;
using System.Text;
using UserService.Db;
using UserService.Interfaces;

namespace UserService.Repo
{
    public class UserRepository : IUserRepo
    {
        public void UserAdd(string name, string password, RoleId roleId)
        {
            using (var context = new UserContext())
            {
                if (roleId == RoleId.Admin)
                {
                    var c = context.Users.Count(x => x.RoleId == RoleId.Admin);
                }
                if (c > 0)
                {
                    throw new Exception("Администратор может быть только один");
                }
                var user = new User();
                user.Name = name;
                user.RoleId = roleId;

                user.Salt = new byte[16];
                new Random().NextBytes(user.Salt);

                var data = Encoding.ASCII.GetBytes(password).Concat(user.Salt).ToArray();

                SHA512 shaM = new SHA512Managed();

                user.Password = shaM.ComputeHash(data);

                context.Add(user);

                context.SaveChanges();
            }
        }
        public RoleId UserCheck(string name, string password)
        {
            using (var context = new UserContext())
            {
                var user = context.Users.FirstOrDefault(x => x.Name == name);

                if (user == null) throw new Exception("User not found");

                var data = Encoding.ASCII.GetBytes(password).Concat(user.Salt).ToArray();
                SHA512 shaM = new SHA512Managed();
                var bpassword = shaM.ComputeHash(data);

                if (user.Password.SequenceEqual(bpassword)) return user.RoleId;
                else throw new Exception("Wrong password");
            }
        }

    }
}