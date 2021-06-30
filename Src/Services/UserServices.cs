using Aduaba.Constants;
using Aduaba.Data;
using Aduaba.DTO;
using Aduaba.DTO.Account;
using Aduaba.Entities;
using Aduaba.Models;
using Aduaba.Presentation;
using Aduaba.Services.Interfaces;
using Aduaba.Setings;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Aduaba.Services
{
    public class UserServices : IUserServices
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly JWT _jwt;
        private readonly ApplicationDbContext _context;
        private readonly IAuthenticatedUserService _authenticatedUser;
        private readonly IConfiguration _configuration;
        private readonly IMailServices _mailService;

        public UserServices(IConfiguration configuration, IMailServices mailService, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IOptions<JWT> jwt, ApplicationDbContext context, IAuthenticatedUserService authenticatedUser)
        {
            _configuration = configuration;
            _mailService = mailService;
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _jwt = jwt.Value;
            _authenticatedUser = authenticatedUser;
        }
        public async Task<AuthenticationResponse> RegisterUserAsync(RegisterRequest model)
        {                  
            if (model == null)
            {
                throw new NullReferenceException("Register Model is Null");
            }
            if (model.Password != model.ConfirmPassword)
            {
                return new AuthenticationResponse
                {
                    Message = "Confirm Password doesn't match assword",
                    IsSuccess = false
                };
            }

            var user = new ApplicationUser
            {
                UserName = model.Username,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.PhoneNumber,
                HouseNumber = model.HouseNumber,
                StreetName = model.StreetName,
                City = model.City,
                State = model.State,
                PostalCode = model.PostalCode,
                Country = model.Country
            };

            var userWithSameEmail = await _userManager.FindByEmailAsync(model.Email);
            if (userWithSameEmail == null)
            {
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, Authorization.Roles.User.ToString());
                    var confirmEmailToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                    var encodedEmailToken = Encoding.UTF8.GetBytes(confirmEmailToken);
                    var validEmailToken = WebEncoders.Base64UrlEncode(encodedEmailToken);
                    string url = $"{_configuration["AppUrl"]}/api/auth/confirmemail?userid={user.Id}&token={validEmailToken}";

                    await _mailService.SendEmailAsync(user.Email, "Confirm your email", "<h1> Welcome to Aduaba App </h1>" +
                        $"<p>please confirm your email by <a> href'{url}' >Clicking here </a> </p>");
                    

                    return new AuthenticationResponse
                    {
                        Message = $"User Registered with username {user.UserName}",
                        IsSuccess = true,
                        IsAuthenticated = true,
                    };
                }
            }
            else
            {
                return new AuthenticationResponse
                {
                    Message = $"Email {user.Email } is already registered.",
                    IsSuccess = false,

                };

            }
            return new AuthenticationResponse
            {
                Message = $"User not created",
                IsSuccess = false,
                IsAuthenticated = false,
            };

        }

        public async Task<AuthenticationResponse> LoginAsync(LoginRequest model)
        {
            var authenticationModel = new AuthenticationResponse();
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return new AuthenticationResponse
                {
                 Message = $"No Accounts Registered with {model.Email}.",
                 IsAuthenticated = false,
                };
          
            }
            var result = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!result)
            {
                return new AuthenticationResponse
                {
                    Message = $"Incorrect Credentials for user {user.Email}.",
                    IsAuthenticated = false,
                };

            }
            
                authenticationModel.IsAuthenticated = true;
                JwtSecurityToken jwtSecurityToken = await CreateJwtToken(user);
                authenticationModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
                authenticationModel.Email = user.Email;
                authenticationModel.UserName = user.UserName;
                var rolesList = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
                authenticationModel.Roles = rolesList.ToList();

                if (user.RefreshTokens.Any(a => a.IsActive))
                {
                    var activeRefreshToken = user.RefreshTokens
                        .Where(a => a.IsActive == true)
                        .FirstOrDefault();
                    return new AuthenticationResponse
                    {
                        RefreshToken = activeRefreshToken.Token,
                        RefreshTokenExpiration = activeRefreshToken.Expires

                    };
                }
                else
                {
                    var refreshToken = CreateRefreshToken();

                    authenticationModel.RefreshToken = refreshToken.Token;
                    authenticationModel.RefreshTokenExpiration = refreshToken.Expires;
                    user.RefreshTokens.Add(refreshToken);
                    _context.Update(user);
                    _context.SaveChanges();


                    return new AuthenticationResponse
                    {
                        RefreshToken = refreshToken.Token,
                        RefreshTokenExpiration = refreshToken.Expires

                    };

                }     

        }
       

      
        public async Task<string> DeleteAsync()
        {
            var currentUser = await _userManager.FindByIdAsync(_authenticatedUser.UserId);
            await _userManager.DeleteAsync(currentUser);
            return "Sorry, to see you go, your account has been deleted";
        }




        //public async Task<string> LogoutAsync()
        //{
        //    var currentUser = await _userManager.FindByIdAsync(_authenticatedUser.UserId);
        //    var userClaims = await _userManager.GetClaimsAsync(currentUser);
        //    // await _userManager.RemoveClaimsAsync(currentUser, userClaims);
        //    // return "Signed out succesfully";


        //}
       
        public async Task<AuthenticationResponse> UpdateAsync(UpdateRequest model)
        {
            var currentUser = await _userManager.FindByIdAsync(_authenticatedUser.UserId);
            currentUser.UserName = model.Username;
            currentUser.FirstName = model.FirstName;
            currentUser.LastName = model.LastName;
            currentUser.PhoneNumber = model.PhoneNumber;
            currentUser.HouseNumber = model.HouseNumber;
            currentUser.StreetName = model.StreetName;
            currentUser.City = model.City;
            currentUser.State = model.State;
            currentUser.Country = model.Country;


            await _userManager.UpdateAsync(currentUser);
            return new AuthenticationResponse
            {
                Message = $"{currentUser.UserName }, your account details has been updated"

            };
          


        }
       
        private async Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);

            var roleClaims = new List<Claim>();

            for (int i = 0; i < roles.Count; i++)
            {
                roleClaims.Add(new Claim("roles", roles[i]));
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id.ToString())
            }
            .Union(userClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwt.DurationInMinutes),
                signingCredentials: signingCredentials);
            return jwtSecurityToken;
        }

        private  RefreshToken CreateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var generator = new RNGCryptoServiceProvider();
            generator.GetBytes(randomNumber);
            return new RefreshToken
            {
                Token = Convert.ToBase64String(randomNumber),
                Expires = DateTime.UtcNow.AddDays(10),
                Created = DateTime.UtcNow
            };
        }

        public async Task<AuthenticationResponse> ForgetPasswordAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)         
                return new AuthenticationResponse
                {
                    Message = "No user associated with this email",
                    IsAuthenticated = false

                };
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = Encoding.UTF8.GetBytes(token);
            var validToken = WebEncoders.Base64UrlEncode(encodedToken);
            //generate url to reroute user
            string url = $"{_configuration["AppUrl"]}/ResetPassword?email={email}$token={validToken}";
            //sending url back to user
            await _mailService.SendEmailAsync(email, "Reset Password", "<h> Follow the instruction to reset password</h>"+
                $"<p>To reset password <a> href ='{url}'>Click here </a></p>");
            return new AuthenticationResponse
            {
                IsSuccess = true,
                IsAuthenticated = true,
                Message="Reset Passworl url has been sent to email"
            };
        }

       public async  Task<AuthenticationResponse> ResetPasswordAsync(ResetPassword model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if(user==null)
            
                return new AuthenticationResponse
                {
                    IsAuthenticated = false,
                    Message="No user with that email address"
                };
            if (model.NewPassword != model.ConfirmPassword)
                return new AuthenticationResponse
                {
                    IsAuthenticated = false,
                    Message = "Passwords do not match"
                };
            var decodedToken = WebEncoders.Base64UrlDecode(model.Token);
            string normalToken = Encoding.UTF8.GetString(decodedToken);
            var result = await _userManager.ResetPasswordAsync(user, normalToken, model.NewPassword);
            if (result.Succeeded)
                return new AuthenticationResponse
                {
                    IsSuccess = true,
                    Message = "Password has been reset successfully"
                };
            return new AuthenticationResponse
            {
                Message="Ooops! Something went wrong",
                IsSuccess=false,
                Errors=result.Errors.Select(e=>e.Description)
            };             
       }

       
    }
}
