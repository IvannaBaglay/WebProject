using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LeaderboardService.Models;

namespace LeaderboardService.Services
{
    public class Leaderboard : ILeaderboardService
    {
        ILeaderboardData LeaderboardData;

        public Leaderboard(ILeaderboardData leaderboardData) => LeaderboardData = leaderboardData;

        public async Task AddScore(string login, int score)
        {
            UserScore user = await LeaderboardData.GetUser(login);
            if (user == null)
            {
                await LeaderboardData.AddUserScore(login, score);
            }
            else if (user.Score < score)
            {
                await LeaderboardData.ChangeUserScore(user, score);
            }
        }

        public async Task<(bool, UserScoreView)> GetScore(string login)
        {
            UserScore user = await LeaderboardData.GetUser(login);

            return user == null ? (false, null) : (true, new UserScoreView(user));
        }

        public async Task<IEnumerable<UserScoreView>> GetTopUsers(int rank)
        {
            return (await LeaderboardData.GetTopUsers(rank)).Select(u => new UserScoreView(u));
        }
    }
}
