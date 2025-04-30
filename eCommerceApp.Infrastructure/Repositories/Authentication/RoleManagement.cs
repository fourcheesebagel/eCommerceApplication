using eCommerceApp.Domain.Entities.Identity;
using eCommerceApp.Domain.Interfaces.Authentication;
using eCommerceApp.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
namespace eCommerceApp.Infrastructure.Repositories.Authentication
{
    public class RoleManagement(UserManager<AppUser> userManager) : IRoleManagement
    {
        public async Task<bool> AddUserToRole(AppUser user, string roleName) => 
            (await userManager.AddToRoleAsync(user, roleName)).Succeeded;

        public async Task<string?> GetUserRole(string userEmail)
        {
            var user = await userManager.FindByEmailAsync(userEmail);
            return (await userManager.GetRolesAsync(user!)).FirstOrDefault();
        }
    }

    public class UserManagement(IRoleManagement roleManagement, UserManager<AppUser> userManager, AppDbContext context) : IUserManagement
    {
        public async Task<bool> CreateUser(AppUser user)
        {
            AppUser? _user = await GetUserByEmail(user.Email!);
            if (user != null) return false;

            return (await userManager.CreateAsync(user!, user!.PasswordHash!)).Succeeded;
        }

        public async Task<IEnumerable<AppUser>?> GetAllUsers() => await context.Users.ToListAsync();

        public async Task<AppUser?> GetUserByEmail(string email) => await userManager.FindByEmailAsync(email);

        public async Task<AppUser> GetUserById(string id) 
        {
            var user = await userManager.FindByIdAsync(id);
            return user!;
        }
        public async Task<List<Claim>> GetUserClaims(string email)
        {
            var _user = await GetUserByEmail(email);
            string? roleName = await roleManagement.GetUserRole(_user!.Email!);

            List<Claim> claims = [
                new Claim("Fullname", _user!.FullName),
                new Claim(ClaimTypes.NameIdentifier, _user!.Id),
                new Claim(ClaimTypes.Email, _user!.Email!),
                new Claim(ClaimTypes.Role, roleName!),
                ];
            return claims;
        }

        public async Task<bool> LoginUser(AppUser user)
        {
            var _user = await GetUserByEmail(user.Email!); //check if user has a registered email first
            if (_user is null) return false;

            string? roleName = await roleManagement.GetUserRole(_user!.Email!); //check if user has a role
            if (string.IsNullOrEmpty(roleName)) return false;

            return await userManager.CheckPasswordAsync(_user, user.PasswordHash!); // check if user's password is correct
        }

        public async Task<int> RemoveUserByEmail(string email)
        {
            var user = await context.Users.FirstOrDefaultAsync(_=>_.Email == email);
            context.Users.Remove(user);
            return await context.SaveChangesAsync();
        }
    }


    public class TokenManagement(AppDbContext context, IConfiguration config) : ITokenManagement
    {
        public async Task<int> AddRefreshToken(string userId, string refreshToken)
        {
            context.RefreshToken.Add(new RefreshToken
            {
                UserId = userId,
                Token = refreshToken
            });
            return await context.SaveChangesAsync();
        }

        public string GenerateToken(List<Claim> claims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWT:Key"]!));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.UtcNow.AddHours(2);
            var token = new JwtSecurityToken(
                issuer: config["JWT:Issuer"],
                audience: config["JWT:Audience"],
                claims: claims,
                expires: expiration,
                signingCredentials: cred);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GetRefreshToken()
        {
            throw new NotImplementedException();
        }

        public List<Claim> GetUserClaimsFromToken(string email)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetUserIdByRefreshToken(string refreshToken)
        {
            throw new NotImplementedException();
        }

        public Task<int> UpdateRefreshToken(string userId, string refreshToken)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ValidateRefreshToken(string refreshToken)
        {
            throw new NotImplementedException();
        }
    }
}
