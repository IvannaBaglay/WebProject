using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LeaderboardService.Models;

namespace LeaderboardService.Services
{
    public class LeaderboardContext : DbContext
    {
        public LeaderboardContext(DbContextOptions<LeaderboardContext> options) : base(options) => Database.EnsureCreated();

        public DbSet<UserScore> Leaderboard { get; set; }
    }
}
