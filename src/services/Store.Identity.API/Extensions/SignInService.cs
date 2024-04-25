using Microsoft.AspNetCore.Identity;

namespace Store.Identity.API.Extensions;

public class SignInService : ISignInService
{
    public SignInManager<IdentityUser> SignInManager { get; }

    public SignInService(SignInManager<IdentityUser> signInManager)
    {
        SignInManager = signInManager;
    }
}