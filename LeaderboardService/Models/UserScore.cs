using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LeaderboardService.Models
{
    public class UserScore
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public int Score { get; set; }

        [NotMapped]
        public int Rank { get; set; } // User position in ordered leaderboard: 1 for first
    }
}
