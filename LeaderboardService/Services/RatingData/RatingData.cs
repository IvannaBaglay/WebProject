using LeaderboardService.Models;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeaderboardService.Services.RatingData
{
    public class RatingData : IRatingData
    {
        RatingContext Context;

        public RatingData(RatingContext context)
        {
            Context = context;
        }

        public async Task CreateRatingsIfNotExist(List<string> names, float defaultRating)
        {
            foreach (string name in names)
            {
                UserRating rating = await Context.Ratings.FirstOrDefaultAsync(item => item.Name == name);
                if (rating == null)
                {
                    Context.Ratings.Add(
                        new UserRating
                        {
                            Name = name,
                            Matches = 0,
                            Rating = defaultRating
                        }
                    );
                }
            }

            await Context.SaveChangesAsync();
        }

        public async Task<List<UserRating>> GetUserRatings(List<string> names)
        {
            return await Context.Ratings
                .Where(rating => names.Contains(rating.Name))
                .ToListAsync();
        }

        public async Task<bool> UpdateUserRatings(List<UserRating> ratings)
        {
            await Context.SaveChangesAsync();
            return true;
        }

        public async Task<List<UserRating>> View()
        {
            return await Context.Ratings
                .ToListAsync();
        }

        public async Task<UserRating> GetUserRating(string name)
        {
            return Context.Ratings
                .OrderByDescending(row => row.Rating)
                .AsEnumerable()
                .Select((u, i) =>
                {
                    u.Rank = i + 1;
                    return u;
                })
                .Where(user => user.Name == name)
                .FirstOrDefault();
        }

        public async Task<IEnumerable<UserRating>> GetTopUsers(int rank)
        {
            const int pageSize = 10;
            int offset = rank <= 0 ? 0 : rank - 1;

            return Context.Ratings
                .OrderByDescending(row => row.Rating)
                .AsEnumerable()
                .Skip(pageSize * offset)
                .Select((u, i) =>
                {
                    u.Rank = i + (pageSize * offset) + 1;
                    return u;
                })
                .Take(pageSize);
        }

    }
}
