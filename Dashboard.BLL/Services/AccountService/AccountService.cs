﻿using Dashboard.BLL.Services.MailService;
using Dashboard.DAL;
using Dashboard.DAL.Models.Identity;
using Dashboard.DAL.Repositories.UserRepository;
using Dashboard.DAL.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Dashboard.BLL.Services.AccountService
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<User> _userManager;
        private readonly IUserRepository _userRepository;
        private readonly IMailService _mailService;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AccountService(UserManager<User> userManager, IUserRepository userRepository, IMailService mailService, IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _userRepository = userRepository;
            _mailService = mailService;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<ServiceResponse> EmailConfirmAsync(string id, string token)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
            {
                return ServiceResponse.BadRequestResponse("Користувача не знайдено");
            }

            var bytes = WebEncoders.Base64UrlDecode(token);
            var validToken = Encoding.UTF8.GetString(bytes);

            var result = await _userRepository.ConfirmEmailAsync(user, validToken);

            if(!result.Succeeded)
            {
                return ServiceResponse.BadRequestResponse(result.Errors.First().Description);
            }

            return ServiceResponse.OkResponse("Пошта успішно підтверджена");
        }

        public async Task<ServiceResponse> SignInAsync(SignInVM model)
        {
            var user = await _userRepository.GetByEmailAsync(model.Email, true);

            if (user == null)
            {
                return ServiceResponse.BadRequestResponse($"Користувача з поштою {model.Email} не знайдено");
            }

            var result = await _userRepository.CheckPasswordAsync(user, model.Password);

            if (!result)
            {
                return ServiceResponse.BadRequestResponse($"Пароль вказано невірно");
            }

            var issuer = _configuration["AuthSettings:issuer"];
            var audience = _configuration["AuthSettings:audience"];
            var keyString = _configuration["AuthSettings:key"];
            var symmetricKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));

            var claims = new List<Claim>
            {
                new Claim("id", user.Id),
                new Claim("email", user.Email)
            };

            if(user.UserRoles.Count() > 0)
            {
                var roleClaims = user.UserRoles.Select(ur => new Claim(
                    "role",
                    ur.Role.Name
                    )).ToArray();

                claims.AddRange(roleClaims);
            }
            else
            {
                claims.Add(new Claim("role", Settings.UserRole));
            }

            // Creating token
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: new SigningCredentials(symmetricKey, SecurityAlgorithms.HmacSha256)
                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return ServiceResponse.OkResponse("Успіший вхід", jwt);
        }

        public async Task<ServiceResponse> SignUpAsync(SignUpVM model)
        {
            if(!await _userRepository.IsUniqueUserNameAsync(model.UserName))
            {
                return ServiceResponse.BadRequestResponse($"{model.UserName} вже викорстовується");
            }

            if (!await _userRepository.IsUniqueEmailAsync(model.Email))
            {
                return ServiceResponse.BadRequestResponse($"{model.Email} вже викорстовується");
            }

            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                UserName = model.UserName,
                NormalizedEmail = model.Email.ToUpper(),
                NormalizedUserName = model.UserName.ToUpper()
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return ServiceResponse.BadRequestResponse(result.Errors.First().Description);
            }

            await _userManager.AddToRoleAsync(user, Settings.UserRole);

            await SendConfirmEmailAsync(user);

            return ServiceResponse.OkResponse($"Користувач {model.Email} успішно зареєстрований", "jwt token");
        }

        private async Task SendConfirmEmailAsync(User user)
        {
            string token = await _userRepository.GenerateEmailConfirmationTokenAsync(user);
            var bytes = Encoding.UTF8.GetBytes(token);
            var validToken = WebEncoders.Base64UrlEncode(bytes);
            var address = _configuration["Host:Address"];

            const string URL_PARAM = "emailConfirmUrl";
            string confirmationUrl = $"{address}/api/account/emailconfrim?u={user.Id}&t={validToken}";

            string rootPath = _webHostEnvironment.ContentRootPath;
            string templatePath = Path.Combine(rootPath, Settings.HtmlPagesPath, "emailconfirmation.html");
            string messageText = File.ReadAllText(templatePath);
            messageText = messageText.Replace(URL_PARAM, confirmationUrl);

            await _mailService.SendEmailAsync(user.Email, "Підтвердження пошти", messageText, true);
        }
    }
}
