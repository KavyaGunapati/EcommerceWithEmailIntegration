using Models.DTOs;

namespace Interfaces.IManagers
{
    public interface IAuthManager
    {
        Task<Result> Register(Register register);
        Task<Result<AuthResponse>> LoginAsync(Login login);
        Task<Result> AssignRole(string userId, string role);
    }
}
