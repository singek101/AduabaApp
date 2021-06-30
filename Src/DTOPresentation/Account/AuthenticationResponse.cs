﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Aduaba.Presentation
{
    public class AuthenticationResponse
    {
        public string Message { get; set; }
        public bool IsSuccess { get; set; }
        public bool IsAuthenticated { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public IEnumerable<string> Errors { get; set; }
        public List<string> Roles { get; set; }
        public string Token { get; set; }
        [JsonIgnore]
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiration { get; set; }
        public DateTime? ExpiryDate { get; set; }

    }
}
