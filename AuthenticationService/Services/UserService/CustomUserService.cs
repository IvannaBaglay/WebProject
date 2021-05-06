using AuthenticationService.Helpers;
using AuthenticationService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthenticationService.Services.UserService
{
    public class CustomUserService : IUserService
    {
        private IUserData UserData;

        public CustomUserService(IUserData userData) => UserData = userData;

        public async Task<(bool, List<User>)> GetAllUsers() => (true, await UserData.GetAllUsers());
        public async Task<(bool, User)> GetUser(int id)
        {
            if (await UserData.IsUserExist(id))
            {
                return (true, await UserData.GetUser(id));
            }
            return (false, null);
        }

        public async Task<(bool, Token)> Login(string login, string password)
        {
            User usertest = await UserData.GetUser(login);
            if (await UserData.IsUserExist(login, password))
            {
                User user = await UserData.GetUser(login);
                return Success(TokenManager.GenerateToken(user));
            }
            else
            {
                return Fail();
            }
        }
        public async Task<(bool, Token)> Refresh(IEnumerable<Claim> claims)
        {
            string login = Utils.GetLoginFromClaims(claims);
            string role = Utils.GetUserRoleFromClaims(claims);

            if (login != null && role != null)
            {
                User user = new User { login = login, role = role };
                return Success(TokenManager.GenerateToken(user));
            }
            return Fail();
        }
        public async Task<bool> CloseSession(IEnumerable<Claim> claims)
        {
            return true;
        }


        public async Task<(bool, Token)> Register(string login, string password)
        {
            byte[] passwordHash, salt;
            PasswordHelper.CreatePasswordHash(password, out passwordHash, out salt);

            User user = new User {
                login = login,
                password = passwordHash,
                passwordSalt = salt,
                role = "user" };

            if (await UserData.IsUserExist(login))
            {
                return Fail();
            }

            if (await UserData.AddUser(user))
            {
                return Success(TokenManager.GenerateToken(user));
            }
            return Fail();
        }

        public async Task<(bool, Token)> ChangeUser(IEnumerable<Claim> claims, User newUser)
        {
            string login = Utils.GetLoginFromClaims(claims);

            if (login != null)
            {
                return await ChangeUser(await UserData.GetUser(login), newUser);
            }
            return Fail();
        }
        public async Task<(bool, Token)> ChangeUser(int id, User newUser) => await ChangeUser(await UserData.GetUser(id), newUser);
        private async Task<(bool, Token)> ChangeUser(User user, User newUser)
        {
            if (user == null
                || (!user.login.Equals(newUser.login)
                    && await UserData.IsUserExist(newUser.login)))
            {
                return Fail();
            }

            User changedUser = await UserData.ChangeUser(user, newUser);
            if (changedUser != null)
            {
                return Success(TokenManager.GenerateToken(changedUser));
            }
            return Fail();
        }


        public async Task<bool> DeleteUser(IEnumerable<Claim> claims)
        {
            string login = Utils.GetLoginFromClaims(claims);

            if (login != null)
            {
                return await DeleteUser(await UserData.GetUser(login));
            }
            return false;
        }
        public async Task<bool> DeleteUser(int id) => await DeleteUser(await UserData.GetUser(id));
        public async Task<bool> DeleteUser(User user)
        {
            if (user == null)
            {
                return false;
            }

            User deletedUser = await UserData.DeleteUser(user);

            return deletedUser != null;
        }

        protected (bool, Token) Fail() => (false, null);
        protected (bool, Token) Success(Token token) => (true, token);
    }
}
