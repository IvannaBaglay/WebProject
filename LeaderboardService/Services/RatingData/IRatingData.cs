using LeaderboardService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeaderboardService.Services.RatingData
{
    public interface IRatingData
    {
        public Task CreateRatingsIfNotExist(List<string> names, float defaultRating);

        public Task<List<UserRating>> GetUserRatings(List<string> names);

        public Task<bool> UpdateUserRatings(List<UserRating> ratings);

        public Task<List<UserRating>> View();

        public Task<UserRating> GetUserRating(string name);

        public Task<IEnumerable<UserRating>> GetTopUsers(int rank);
    }
}
