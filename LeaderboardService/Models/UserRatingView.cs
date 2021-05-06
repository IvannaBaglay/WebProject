using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeaderboardService.Models
{
    public class UserRatingView
    {
        public UserRatingView() { }
        public UserRatingView(UserRating rating)
        {
            Login = rating.Name;
            Rating = rating.Rating;
            Rank = rating.Rank;
            Matches = rating.Matches;
        }

        public string Login { get; set; }
        public float Rating { get; set; }
        public int Rank { get; set; }
        public uint Matches { get; set; }
    }
}
