﻿using Aduaba.Models;
using Aduaba.Presentation;
using Aduaba.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aduaba.Services
{
    public class MailServices : IMailServices
    {
        private readonly  IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public MailServices( IConfiguration configuration, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _configuration = configuration;
            _userManager = userManager;
            _roleManager = roleManager;
        }
            

        public async Task<AuthenticationResponse> ConfirmEmailAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new AuthenticationResponse
                {
                    IsAuthenticated = false,
                    Message = "User not found"
                };
            }
             var decodedToken = WebEncoders.Base64UrlDecode(token);
             string normaltoken = Encoding.UTF8.GetString(decodedToken);
            var result = await _userManager.ConfirmEmailAsync(user, normaltoken);
            if (result.Succeeded)
                return new AuthenticationResponse
                {
                    Message = "Email confirmed successfully",
                    IsAuthenticated = true
                };
            return new AuthenticationResponse
            {
                IsAuthenticated = false,
                Message = "Email did not confirm",
                Errors=result.Errors.Select(e=> e.Description)
            };






        }

        public async Task SendEmailAsync(string email, string subject, string content)
        {
            var apiKey = Environment.GetEnvironmentVariable("NAME_OF_THE_ENVIRONMENT_VARIABLE_FOR_YOUR_SENDGRID_KEY");
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("Confirm@Aduaba.com", "Aduaba App");           
            var to = new EmailAddress(email);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, content, content);
            var response = await client.SendEmailAsync(msg);
        }
    }
}
