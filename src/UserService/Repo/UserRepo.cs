using System.Security.Cryptography;
using System.Text;
using UserService.Db;
using UserService.Interfaces;

namespace UserService.Repo
{
    public sealed class UserRepository : IUserRepo
    {
        private readonly UserContext _context;

        public UserRepository(UserContext context)
        {
            _context = context;
        }

        public void UserAdd(string email, string password, RoleId roleId)
        {
            int adminCount = 0;
            if (roleId == RoleId.Admin)
                adminCount = _context.Users.Count(x => x.RoleId == RoleId.Admin);
            if (adminCount > 0)
                throw new Exception("Администратор может быть только один");

            var user = new User
            {
                Email = email,
                Name = email,
                RoleId = roleId,
                Salt = new byte[16]
            };
            Random.Shared.NextBytes(user.Salt);
            var data = Encoding.ASCII.GetBytes(password).Concat(user.Salt).ToArray();
            user.Password = SHA512.HashData(data);

            _context.Add(user);
            _context.SaveChanges();
        }

        public (RoleId RoleId, int UserId) UserCheck(string email, string password)
        {
            var user = _context.Users.FirstOrDefault(x => x.Email == email)
                ?? throw new Exception("User not found");

            var data = Encoding.ASCII.GetBytes(password).Concat(user.Salt).ToArray();
            var hash = SHA512.HashData(data);
            if (!user.Password.SequenceEqual(hash))
                throw new Exception("Wrong password");
            return (user.RoleId, user.Id);
        }

        public (int UserId, RoleId RoleId) EnsureUserByEmail(string email, string? name)
        {
            var user = _context.Users.FirstOrDefault(x => x.Email == email);
            if (user != null)
                return (user.Id, user.RoleId);

            var newUser = new User
            {
                Email = email,
                Name = name ?? email,
                RoleId = RoleId.User,
                Salt = new byte[16]
            };
            Random.Shared.NextBytes(newUser.Salt);
            newUser.Password = SHA512.HashData(Encoding.ASCII.GetBytes(Guid.NewGuid().ToString()).Concat(newUser.Salt).ToArray());
            _context.Add(newUser);
            _context.SaveChanges();
            return (newUser.Id, newUser.RoleId);
        }
    }
}