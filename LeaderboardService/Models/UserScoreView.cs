using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeaderboardService.Models
{
    // User score without id field
    public class UserScoreView
    {
        public UserScoreView(UserScore score)
        {
            Login = score.Login;
            Score = score.Score;
            Rank = score.Rank;
        }
        public string Login { get; set; }
        public int Score { get; set; }
        public int Rank { get; set; }
    }
}
