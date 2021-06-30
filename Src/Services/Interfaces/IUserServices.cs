using Aduaba.DTO;
using Aduaba.DTO.Account;
using Aduaba.Entities;
using Aduaba.Models;
using Aduaba.Presentation;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace Aduaba.Services
{
    public interface IUserServices
    {
        Task<AuthenticationResponse> RegisterUserAsync(RegisterRequest model);
        Task<AuthenticationResponse> LoginAsync(LoginRequest model);
        Task<AuthenticationResponse> UpdateAsync(UpdateRequest model);
        Task<string> DeleteAsync();
        Task<AuthenticationResponse> ForgetPasswordAsync(string email);
        Task<AuthenticationResponse> ResetPasswordAsync(ResetPassword model);

       /* RefreshToken CreateRefreshToken();
        Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user);

       //Task<string> LogoutAsync();*/
    }
}
