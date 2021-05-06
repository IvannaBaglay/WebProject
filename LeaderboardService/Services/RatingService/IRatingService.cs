using LeaderboardService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeaderboardService.Services.RatingService
{
    public interface IRatingService
    {
        public Task<bool> Update(List<NameScore> data);
        public Task<List<UserRating>> View();
        public Task<UserRatingView> GetRating(string name);
        public Task<List<UserRating>> GetRating(List<string> names);

        public Task<IEnumerable<UserRatingView>> GetTopUsers(int rank);
    }
}
