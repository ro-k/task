using Lib.DataAccess;
using Lib.DTOs;

namespace Lib.Services;

public class AuthService(
    ISignInManagerWrapper signInManager,
    IUserDataAccess userDataAccess,
    ITokenService tokenService)
    : IAuthService
{
    public async Task<string> Login(LoginDto login)
    {
        var result = await signInManager.PasswordSignInAsync(login.Username, login.Password, false, lockoutOnFailure: false);
        
        // todo: add custom exceptions
        if (!result.Succeeded) throw new Exception("Unauthorized");
        
        
        var cancellationToken = new CancellationToken();
        var user = await userDataAccess.FindByNameAsync(login.Username, cancellationToken);
        var roles = await userDataAccess.GetRolesAsync(user, cancellationToken);
        var token = tokenService.GenerateJwtToken(user, roles.ToList());
        return token;
    }

    public async Task Logout(string user)
    {
        throw new NotImplementedException();
    }

    public async Task Register()
    {
        throw new NotImplementedException();
    }

    public async Task ExternalLogin(string loginProvider, string providerKey)
    {
        throw new NotImplementedException();
    }

    public async Task<string> CreateToken()
    {
        throw new NotImplementedException();
    }

    public async Task<string> RefreshToken()
    {
        throw new NotImplementedException();
    }
}