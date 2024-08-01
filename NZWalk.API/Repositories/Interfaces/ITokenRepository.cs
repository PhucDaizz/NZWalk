using Microsoft.AspNetCore.Identity;

namespace NZWalk.API.Repositories.Interfaces
{
    public interface ITokenRepository
    {
        string CreateJWTToken(IdentityUser user, List<string> roles);
    }
}
