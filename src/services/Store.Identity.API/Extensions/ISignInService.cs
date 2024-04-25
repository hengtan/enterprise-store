using Microsoft.AspNetCore.Identity;

namespace Store.Identity.API.Extensions;

public interface ISignInService
{
    SignInManager<IdentityUser> SignInManager { get; }
}
