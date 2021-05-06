using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LeaderboardService.Models;

namespace LeaderboardService.Services
{
    public interface ILeaderboardService
    {
        public Task AddScore(string login, int score);
        public Task<(bool, UserScoreView)> GetScore(string login);
        public Task<IEnumerable<UserScoreView>> GetTopUsers(int rank);
    }
}
