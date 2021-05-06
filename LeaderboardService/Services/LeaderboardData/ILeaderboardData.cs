using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LeaderboardService.Models;

namespace LeaderboardService.Services
{
    public interface ILeaderboardData
    {
        public Task<IEnumerable<UserScore>> GetTopUsers(int rank);
        public Task<UserScore> GetUser(string login);
        public Task ChangeUserScore(UserScore user, int score);
        public Task AddUserScore(string login, int score);
    }
}
