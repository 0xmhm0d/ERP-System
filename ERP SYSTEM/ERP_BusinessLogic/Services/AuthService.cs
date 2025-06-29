using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ERP_BusinessLogic.Helpers;
using ERP_BusinessLogic.Options;
using ERP_Domians.IServices;
using ERP_Domians.Models;
using ERP_Domians.Models.ReturnModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ERP_BusinessLogic.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly JwtOptions _jwtOptions;

        public AuthService(
            IConfiguration configuration,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<JwtOptions> jwtOptions)
        {
            _configuration = configuration;
            _userManager = userManager;
            _jwtOptions = jwtOptions.Value;
            _roleManager = roleManager;
        }

        public async Task<ReturnedAuthModel> RegisterAsync(ApplicationUser user, string password)
        {
            // Check If Email Exists
            if (await _userManager.FindByEmailAsync(user.Email) is not null)
                return new ReturnedAuthModel() { Message = "Email Is Already Registered" }; // IsAuthenticated = false

            // Check If Username Exists
            if (await _userManager.FindByNameAsync(user.UserName) is not null)
                return new ReturnedAuthModel() { Message = "Username Is Already Registered" }; // IsAuthenticated = false

            // Create New User
            var result = await _userManager.CreateAsync(user, password);

            // Check If The Operation Succeeded or Not
            if (!result.Succeeded)
            {
                var errors = string.Empty;

                foreach (var error in result.Errors)
                    errors += $"{error.Description},";

                return new ReturnedAuthModel() { Message = errors }; // IsAuthenticated = false
            }

            // Add Default Role
            await _userManager.AddToRoleAsync(user, Roles.UserRole);

            // Generate Token
            var jwtToken = await CreateToken(user);
            var rolesList = await _userManager.GetRolesAsync(user);

            return new ReturnedAuthModel()
            {
                IsAuthenticated = true,
                Message = "Authentication Succeeded",
                Email = user.Email,
                UserName = user.UserName,
                Roles = rolesList.ToList(),
                Token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                ExpiresOn = jwtToken.ValidTo,
            };
        }

        public async Task<ReturnedAuthModel> LoginAsync(string email, string password)
        {
            var returnedAuthModel = new ReturnedAuthModel();

            var user = await _userManager.FindByEmailAsync(email);

            // Check If User Exists With This Email || Check If The Password is Correct For This User

            if (user is null || !await _userManager.CheckPasswordAsync(user, password)) // IsAuthenticated = false
            {
                returnedAuthModel.Message = "Email Or Password Is Not Valid";
                return returnedAuthModel;
            }

            // Generate Token
            var jwtToken = await CreateToken(user);
            var rolesList = await _userManager.GetRolesAsync(user);

            returnedAuthModel.IsAuthenticated = true;
            returnedAuthModel.Email = email;
            returnedAuthModel.UserName = user.UserName;
            returnedAuthModel.Roles = rolesList.ToList();
            returnedAuthModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
            returnedAuthModel.ExpiresOn = jwtToken.ValidTo;

            return returnedAuthModel;
        }

        public async Task<string> AddRoleAsync(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);

            // Check If User Exists With This Id || The Role is Exists
            if (user is null || !await _roleManager.RoleExistsAsync(role))
                return "UserId Or Role Is Not Valid";

            // Check If User Assigned To This Role
            if (await _userManager.IsInRoleAsync(user, role))
                return "User Already Assignee To This Role";

            var result = await _userManager.AddToRoleAsync(user, role);

            return result.Succeeded ? string.Empty : "Something Wrong Happened";
        }

        private async Task<JwtSecurityToken> CreateToken(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);

            var rolesClaims = new List<Claim>();

            var roles = await _userManager.GetRolesAsync(user);

            foreach (var role in roles)
                rolesClaims.Add(new Claim("role", role));

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Email, user.Email),
                new("name", $"{user.FirstName} {user.LastName}")
            }
            .Union(userClaims)
            .Union(rolesClaims)
            .ToList();

            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));

            var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtOptions.ValidIssuer,
                audience: _jwtOptions.ValidAudience,
                claims: claims,
                signingCredentials: signingCredentials,
                expires: DateTime.Now.AddDays(_jwtOptions.Lifetime)
                );

            return token;
        }

        #region Old Code

        //private string CreateToken(ApplicationUser applicationUser)
        //{
        //    var claims = GetClaimsAsync(applicationUser);
        //    var signingCredentials = GetSigningCredentials();
        //    var tokenOptions = GenerateTokenOptions(signingCredentials, claims.Result);

        //    return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        //}

        //private async Task<List<Claim>> GetClaimsAsync(ApplicationUser user)
        //{
        //    var userClaims = await _userManager.GetClaimsAsync(user);

        //    var rolesClaims = new List<Claim>();

        //    var roles = await _userManager.GetRolesAsync(user);

        //    foreach (var role in roles)
        //        rolesClaims.Add(new Claim("role", role));

        //    var claims = new List<Claim>
        //    {
        //        new(JwtRegisteredClaimNames.Email, user.Email),
        //        new("name", $"{user.FirstName} {user.LastName}")
        //    }
        //    .Union(userClaims)
        //    .Union(rolesClaims)
        //    .ToList();

        //    return claims;
        //}

        //private SigningCredentials GetSigningCredentials()
        //{
        //    var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));

        //    return new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
        //}

        //private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
        //{
        //    var tokenOptions = new JwtSecurityToken(

        //        //issuer: _configuration["Jwt:ValidIssuer"],
        //        issuer: _jwtOptions.ValidIssuer,
        //        //audience: _configuration["Jwt:ValidAudience"],
        //        audience: _jwtOptions.ValidAudience,
        //        claims: claims,
        //        signingCredentials: signingCredentials,
        //        //expires: DateTime.Now.AddDays(Convert.ToDouble((_configuration.GetSection("Jwt")).GetSection("Lifetime").Value))
        //        expires: DateTime.Now.AddDays(_jwtOptions.Lifetime)
        //        );

        //    return tokenOptions;
        //} 
        #endregion
    }
}
