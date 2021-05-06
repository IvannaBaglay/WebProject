using LeaderboardService.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeaderboardService.Services
{
    public class LeaderboardData : ILeaderboardData
    {
        LeaderboardContext Context;

        public LeaderboardData(LeaderboardContext leaderboard) => Context = leaderboard;

        public async Task AddUserScore(string login, int score)
        {
            Context.Leaderboard.Add(new UserScore { Login = login, Score = score });

            await Context.SaveChangesAsync();
        }

        public async Task ChangeUserScore(UserScore user, int score)
        {
            user.Score = score;

            await Context.SaveChangesAsync();
        }

        public async Task<IEnumerable<UserScore>> GetTopUsers(int rank)
        {
            const int pageSize = 10;
            int offset = rank <= 0 ? 0 : rank - 1;

            return Context.Leaderboard
                .OrderByDescending(row => row.Score)
                .AsEnumerable()
                .Skip(pageSize * offset)
                .Select((u, i) =>
                {
                    u.Rank = i + (pageSize * offset) + 1;
                    return u;
                })
                .Take(pageSize);
        }

        public async Task<UserScore> GetUser(string login)
        {
            return Context.Leaderboard
                .OrderByDescending(row => row.Score)
                .AsEnumerable()
                .Select((u, i) =>
                {
                    u.Rank = i + 1;
                    return u;
                })
                .Where(user => user.Login == login)
                .FirstOrDefault();
        }
    }
}
