using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationService.Models
{
    public interface IUserData
    {
        public Task<bool> AddUser(User user);

        public Task<List<User>> GetAllUsers();
        public Task<User> GetUser(string login);
        public Task<User> GetUser(int id);

        public Task<User> ChangeUser(User user, User newUser);

        public Task<User> DeleteUser(User user);

        public Task<bool> IsUserExist(string login, string password);
        public Task<bool> IsUserExist(string login);
        public Task<bool> IsUserExist(int id);
    }
}
