using DakGharNiryatKendra.Data;
using DakGharNiryatKendra.Models;
using DakGharNiryatKendra.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace DakGharNiryatKendra.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string SessionKeyUser = "DNK_ACTIVE_USER";

        public AuthService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<User?> GetCurrentUserAsync()
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            string? username = session?.GetString(SessionKeyUser);

            if (string.IsNullOrEmpty(username))
            {
                username = "exporter1"; // Default demo active user
                session?.SetString(SessionKeyUser, username);
            }

            return await _context.Users
                .Include(u => u.AssignedDnkCenter)
                .FirstOrDefaultAsync(u => u.Username == username)
                ?? await _context.Users.FirstOrDefaultAsync();
        }

        public async Task SetCurrentUserAsync(string username)
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            if (session != null)
            {
                session.SetString(SessionKeyUser, username);
            }
        }

        public async Task<List<User>> GetAllDemoUsersAsync()
        {
            return await _context.Users
                .Include(u => u.AssignedDnkCenter)
                .OrderBy(u => u.Role)
                .ToListAsync();
        }

        public async Task<User?> AuthenticateAsync(string username)
        {
            var user = await _context.Users
                .Include(u => u.AssignedDnkCenter)
                .FirstOrDefaultAsync(u => u.Username.ToLower() == username.Trim().ToLower());

            if (user != null)
            {
                await SetCurrentUserAsync(user.Username);
            }

            return user;
        }

        // Password-based authentication
        public async Task<User?> AuthenticateWithPasswordAsync(string usernameOrEmail, string password)
        {
            var normalizedInput = usernameOrEmail.Trim().ToLower();

            var user = await _context.Users
                .Include(u => u.AssignedDnkCenter)
                .FirstOrDefaultAsync(u =>
                    u.Username.ToLower() == normalizedInput ||
                    u.Email.ToLower() == normalizedInput);

            if (user == null)
                return null;

            // Demo users (no password hash) - allow login with any password or the username itself
            if (string.IsNullOrEmpty(user.PasswordHash))
            {
                await SetCurrentUserAsync(user.Username);
                return user;
            }

            // Verify password
            if (!VerifyPassword(password, user.PasswordHash))
                return null;

            await SetCurrentUserAsync(user.Username);
            return user;
        }

        public async Task<(bool Success, string Error, User? User)> RegisterUserAsync(RegisterViewModel model)
        {
            // Check for duplicate username
            if (await IsUsernameTakenAsync(model.Username))
            {
                return (false, "This username is already taken. Please choose a different one.", null);
            }

            // Check for duplicate email
            if (await IsEmailTakenAsync(model.Email))
            {
                return (false, "An account with this email already exists. Please log in instead.", null);
            }

            var user = new User
            {
                Username = model.Username.Trim(),
                FullName = model.FullName.Trim(),
                Email = model.Email.Trim(),
                PhoneNumber = model.PhoneNumber.Trim(),
                PasswordHash = HashPassword(model.Password),
                Role = UserRole.Exporter,
                BusinessName = model.BusinessName?.Trim(),
                IecNumber = model.IecNumber?.Trim(),
                Gstin = model.Gstin?.Trim(),
                AdCode = model.AdCode?.Trim(),
                LutNumber = model.LutNumber?.Trim(),
                Address = model.Address?.Trim(),
                City = model.City?.Trim(),
                State = model.State?.Trim(),
                Pincode = model.Pincode?.Trim(),
                IsVerified = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            await SetCurrentUserAsync(user.Username);

            return (true, string.Empty, user);
        }

        public string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        public bool VerifyPassword(string password, string hash)
        {
            var computedHash = HashPassword(password);
            return computedHash == hash;
        }

        public async Task<bool> IsUsernameTakenAsync(string username)
        {
            return await _context.Users
                .AnyAsync(u => u.Username.ToLower() == username.Trim().ToLower());
        }

        public async Task<bool> IsEmailTakenAsync(string email)
        {
            return await _context.Users
                .AnyAsync(u => u.Email.ToLower() == email.Trim().ToLower());
        }
    }
}
