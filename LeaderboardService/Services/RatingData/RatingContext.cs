using LeaderboardService.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeaderboardService.Services.RatingData
{
    public class RatingContext : DbContext
    {
        public RatingContext(DbContextOptions<RatingContext> options) : base(options) => Database.EnsureCreated();

        public DbSet<UserRating> Ratings { get; set; }
    }
}
