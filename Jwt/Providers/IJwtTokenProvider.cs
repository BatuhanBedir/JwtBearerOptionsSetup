using Jwt.Models;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace Jwt.Providers;

public interface IJwtTokenProvider
{
    string GenerateToken(AppUser user);
}
