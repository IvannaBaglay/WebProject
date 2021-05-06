using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LeaderboardService.Models
{
    public class UserRating
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public float Rating { get; set; }
        public uint Matches { get; set; }

        [NotMapped]
        public int Rank { get; set; } // User position in ordered leaderboard: 1 for first
    }
}
