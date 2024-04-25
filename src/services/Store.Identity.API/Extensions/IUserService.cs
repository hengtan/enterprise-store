using Microsoft.AspNetCore.Identity;

namespace Store.Identity.API.Extensions;

public interface IUserService
{
    UserManager<IdentityUser> UserManager { get; }
}