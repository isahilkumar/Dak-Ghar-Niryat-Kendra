using DakGharNiryatKendra.Models;
using DakGharNiryatKendra.Models.ViewModels;

namespace DakGharNiryatKendra.Services
{
    public interface IAuthService
    {
        Task<User?> GetCurrentUserAsync();
        Task SetCurrentUserAsync(string username);
        Task<List<User>> GetAllDemoUsersAsync();
        Task<User?> AuthenticateAsync(string username);

        // Password-based authentication
        Task<User?> AuthenticateWithPasswordAsync(string usernameOrEmail, string password);
        Task<(bool Success, string Error, User? User)> RegisterUserAsync(RegisterViewModel model);
        string HashPassword(string password);
        bool VerifyPassword(string password, string hash);
        Task<bool> IsUsernameTakenAsync(string username);
        Task<bool> IsEmailTakenAsync(string email);
    }
}
