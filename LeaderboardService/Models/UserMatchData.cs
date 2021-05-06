using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeaderboardService.Models
{
    public class NameScore
    {
        public string Name { get; set; }
        public int Score { get; set; }
    }

    public class UserMatchData
    {
        public List<NameScore> MatchResults { get; set; }
    }
}
