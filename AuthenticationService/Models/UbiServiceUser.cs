using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationService.Models
{
    public class UbiServiceUser
    {
        public string nameOnPlatform { get; set; }
        public string ticket { get; set; }
        public string profileId { get; set; }
        public string userId { get; set; }
        public string serverTime { get; set; }
        public string expiration { get; set; }

        public int TokenExpirationTime()
        {
            DateTime serverTime_ = DateTime.Parse(serverTime);
            DateTime expiration_ = DateTime.Parse(expiration);

            Console.WriteLine(serverTime + " " + expiration);
            TimeSpan span = (expiration_ - serverTime_);
            int minutesToExpiration = span.Minutes + (span.Hours * 60);
            Console.WriteLine(minutesToExpiration);
            return minutesToExpiration;
        }
    }
}
