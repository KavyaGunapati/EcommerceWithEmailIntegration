using DataAccess.Entities;
using Interfaces.IManagers;
using Managers.JwtTokenService;
using Microsoft.AspNetCore.Identity;
using Models.DTOs;

namespace Managers
{
    public class AuthManager : IAuthManager
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly TokenService _tokenService;

        public AuthManager(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, TokenService tokenService)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _tokenService = tokenService;
        }
        public async Task<Result> AssignRole(string userId, string role)
        {
            try
            {
                var userExist = await _userManager.FindByIdAsync(userId);
                if (userExist == null) return new Result { Success = false, Message = "User Not found" };
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }
                var assign = await _userManager.AddToRoleAsync(userExist, role);
                return assign.Succeeded ? new Result { Success = true, Message = "Role Assigned" } : new Result { Success = false, Message = string.Join(", ", assign.Errors.Select(e => e.Description)) };
            }
            catch (Exception ex)
            {
                return new Result { Success = false, Message = ex.Message };
            }
        }
        public async Task<Result<AuthResponse>> LoginAsync(Login login)
        {
            try
            {
                var userExist = await _userManager.FindByEmailAsync(login.Email);
                if (userExist == null) { return new Result<AuthResponse> { Success = false, Message = "user Not Found" }; }
                var loggedin = await _userManager.CheckPasswordAsync(userExist, login.Password);
                if (loggedin == false) return new Result<AuthResponse> { Success = false, Message = "Unauthorized User" };
                var roles = await _userManager.GetRolesAsync(userExist);
                var accessToken = _tokenService.GenerateToken(userExist, roles);
                return new Result<AuthResponse> { Success = true, Message = "User login successfully", Data = new AuthResponse { AccessToken = accessToken, Email = login!.Email } };
            }
            catch (Exception ex)
            {
                return new Result<AuthResponse> { Success = false, Message = ex.Message };
            }
        }

        public async Task<Result> Register(Register register)
        {
            var userExist = await _userManager.FindByEmailAsync(register.Email);
            if (userExist != null) { return new Result { Success = false, Message = "user Already  Registered" }; }
            var newUser = new ApplicationUser
            {
                FullName = register.FullName,
                Email = register.Email,
                UserName = register.Email,
            };
            var result = await _userManager.CreateAsync(newUser, register.Password);
            if (!string.IsNullOrEmpty(register.Role))
            {
                if (!await _roleManager.RoleExistsAsync(register.Role))
                    await _roleManager.CreateAsync(new IdentityRole(register.Role));
                await _userManager.AddToRoleAsync(newUser, register.Role);
            }
            return result.Succeeded ? new Result { Success = true, Message = "Registered Successfully" } : new Result { Success = false, Message = string.Join(", ", result.Errors.Select(e => e.Description)) };
        }
    }
}
