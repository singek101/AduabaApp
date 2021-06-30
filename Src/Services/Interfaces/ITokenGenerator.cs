using Aduaba.Models;
using Aduaba.Presentation;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace Aduaba.Services.Interfaces
{
    public interface ITokenGenerator
    {
        Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user);
        Task<AuthenticationResponse> RefreshTokenAsync(string token);
        public bool RevokeRefreshToken(string token);
    }
}
