using LeaderboardService.Models;
using LeaderboardService.Services.RatingData;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeaderboardService.Services.RatingService
{
    public class RatingService : IRatingService
    {
        public IRatingData Data;

        private const float MIN_RATING = 25;
        private const float MAX_CONTRIBUTION = 50;
        private const float MIN_CONTRIBUTION = 10;
        private const float COEFFICIENT = 1 / 50;

        public RatingService(IRatingData data)
        {
            Data = data;
        }

        private List<string> ExtractNames(List<NameScore> data)
        {
            List<string> names = new List<string>();
            foreach (NameScore nameScore in data)
            {
                names.Add(nameScore.Name);
            }
            return names;
        }

        private float CalculateRatingContribution(float averageMatches)
        {
            return (MAX_CONTRIBUTION - MIN_CONTRIBUTION) / (COEFFICIENT * averageMatches + 1) + MIN_CONTRIBUTION;
        }

        public async Task<bool> Update(List<NameScore> data)
        {
            if (data == null || data.Count < 2)
            {
                return false;
            }

            List<string> names = ExtractNames(data);
            if (names == null || names.Count != data.Count)
            {
                return false;
            }

            await Data.CreateRatingsIfNotExist(names, MIN_RATING);

            List<UserRating> ratings = await Data.GetUserRatings(names);
            if (ratings == null || ratings.Count != data.Count)
            {
                return false;
            }

            float averageRate = ratings.Sum(item => item.Rating) / ratings.Count;
            float averageMatches = ratings.Sum(item => item.Matches) / ratings.Count;

            foreach (UserRating userRating in ratings)
            {
                ++userRating.Matches;
            }

            float C = CalculateRatingContribution(averageMatches);

            float ratesBank = ratings.Count * C;
            float scoresSum = data.Sum(item => item.Score);
            if (Math.Abs(scoresSum) <= 0.0001)
            {
                return await Data.UpdateUserRatings(ratings);
            }
            float ratePerScore = ratesBank / scoresSum;

            for (int i = 0; i < ratings.Count; ++i)
            {
                int score = data.FirstOrDefault(item => item.Name == ratings[i].Name).Score;
                ratings[i].Rating += score * ratePerScore - C;
                
                if (ratings[i].Rating < MIN_RATING)
                {
                    ratings[i].Rating = MIN_RATING;
                }
            }

            return await Data.UpdateUserRatings(ratings);
        }

        public async Task<List<UserRating>> View()
        {
            return await Data.View();
        }

        public async Task<UserRatingView> GetRating(string name)
        {
            UserRating rating = await Data.GetUserRating(name);
            return (rating == null) ? null : new UserRatingView(rating);
        }

        public async Task<List<UserRating>> GetRating(List<string> names)
        {
            List<UserRating> ratings = await Data.GetUserRatings(names);
            for (int i = 0; i < names.Count; ++i)
            {
                if (ratings.FirstOrDefault(item => item.Name == names[i]) == null)
                {
                    ratings.Add(new UserRating
                    {
                        Rating = MIN_RATING,
                        Matches = 0,
                        Name = names[i]
                    });
                }
            }

            return ratings;
        }

        public async Task<IEnumerable<UserRatingView>> GetTopUsers(int rank)
        {
            return (await Data.GetTopUsers(rank)).Select(u => new UserRatingView(u));
        }

    }
}
