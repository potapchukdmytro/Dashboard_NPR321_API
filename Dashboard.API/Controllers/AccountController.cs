﻿using Dashboard.BLL.Services.AccountService;
using Dashboard.BLL.Validators;
using Dashboard.DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Dashboard.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : BaseController
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("signin")]
        public async Task<IActionResult> SignInAsync([FromBody] SignInVM model)
        {
            var validator = new SignInValidator();
            var validation = await validator.ValidateAsync(model);

            if (!validation.IsValid)
            {
                return BadRequest(validation.Errors);
            }

            var response = await _accountService.SignInAsync(model);

            return GetResult(response);
        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignUpAsync([FromBody] SignUpVM model)
        {
            SignUpValidator validator = new SignUpValidator();
            var validation = await validator.ValidateAsync(model);

            if (!validation.IsValid)
            {
                return BadRequest(validation.Errors);
            }

            var response = await _accountService.SignUpAsync(model);
            return GetResult(response);
        }

        [HttpGet("emailconfrim")]
        public async Task<IActionResult> EmailConfirmAsync(string u, string t)
        {
            if(string.IsNullOrEmpty(u) || string.IsNullOrEmpty(t))
            {
                return NotFound();
            }

            var response = await _accountService.EmailConfirmAsync(u, t);
            return GetResult(response);
        }
    }
}