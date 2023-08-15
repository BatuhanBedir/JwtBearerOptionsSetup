using Jwt.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Jwt.Contexts
{
    public class JwtDbContext : IdentityDbContext<AppUser>
    {
        public JwtDbContext(DbContextOptions<JwtDbContext> options) : base(options) { }
        
    }
}
