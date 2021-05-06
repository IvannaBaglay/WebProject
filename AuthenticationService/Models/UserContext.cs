using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationService.Models
{
    public class UserContext : DbContext
    {
        public UserContext() {}
        public UserContext(DbContextOptions<UserContext> options) : base(options) => Database.EnsureCreated();

        public DbSet<User> users { get; set; }
    }
}
