using AuthenticationService.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthenticationService.Models
{
    public interface IUserService
    {
        public Task<(bool, List<User>)> GetAllUsers();
        public Task<(bool, User)> GetUser(int id);

        public Task<(bool, Token)> Login(string login, string password);
        public Task<(bool, Token)> Refresh(IEnumerable<Claim> claims);
        public Task<bool> CloseSession(IEnumerable<Claim> claims);

        public Task<(bool, Token)> Register(string login, string password);

        public Task<(bool, Token)> ChangeUser(IEnumerable<Claim> claims, User newUser);
        public Task<(bool, Token)> ChangeUser(int id, User newUser);

        public Task<bool> DeleteUser(IEnumerable<Claim> claims);
        public Task<bool> DeleteUser(int id);
    }
}
