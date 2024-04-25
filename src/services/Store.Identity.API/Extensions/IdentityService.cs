namespace Store.Identity.API.Extensions;

using Microsoft.AspNetCore.Identity;

public class IdentityService
{
    public SignInManager<IdentityUser> SignInManager { get; }
    public UserManager<IdentityUser> UserManager { get; }

    public IdentityService(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
    {
        SignInManager = signInManager;
        UserManager = userManager;
    }
}