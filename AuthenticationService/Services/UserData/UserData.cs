using AuthenticationService.Helpers;
using AuthenticationService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationService.Services
{
    public class UserData : IUserData
    {
        private UserContext Context;

        public UserData(UserContext context) => Context = context;

        public async Task<bool> AddUser(User user)
        {
            Context.users.Add(user);
            await Context.SaveChangesAsync();

            return true;
        }

        public async Task<User> ChangeUser(User user, User newUser)
        {
            user.login = newUser.login;
            user.password = newUser.password;
            user.passwordSalt = newUser.passwordSalt;

            await Context.SaveChangesAsync();

            return user;
        }

        public async Task<User> DeleteUser(User user)
        {
            Context.users.Remove(user);
            await Context.SaveChangesAsync();

            return user;
        }


        public async Task<List<User>> GetAllUsers() => Context.users.ToList();
        public async Task<User> GetUser(string login) => Context.users.Where(_user => _user.login == login).FirstOrDefault();
        public async Task<User> GetUser(int id) => Context.users.Where(_user => _user.id == id).FirstOrDefault();


        public async Task<bool> IsUserExist(string login, string password)
        {
            bool isRealUser = await IsUserExist(login);
            if (!isRealUser)
            {
                return false;
            }

            var user = await GetUser(login);

            return PasswordHelper.VerifyPasswordHash(password, user.password, user.passwordSalt);
        }
        public async Task<bool> IsUserExist(string login) => Context.users.Any(_user => _user.login == login);
        public async Task<bool> IsUserExist(int id) => Context.users.Any(_user => _user.id == id);
    }
}
