using Microsoft.AspNetCore.Identity;

namespace Store.Identity.API.Extensions;

public class UserService : IUserService
{
    public UserManager<IdentityUser> UserManager { get; }

    public UserService(UserManager<IdentityUser> userManager)
    {
        UserManager = userManager;
    }
}