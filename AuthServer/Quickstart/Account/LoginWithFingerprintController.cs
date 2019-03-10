using AuthServer.Data;
using AuthServer.Models;
using FingerprintAuth.Server;
using FingerprintAuth.Shared.DTO;
using FingerprintAuth.Web.Hubs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;


namespace AuthServer.Quickstart.Account
{
    public class LoginWithFingerprintController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _dbContext;
        private readonly IFingerprintAuth _fingerprintAuth;
        private readonly IHubContext<LoginHub> _loginHub;
        

        public LoginWithFingerprintController(
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext applicationDbContext, IFingerprintAuth fingerprintAuth,
            UserManager<ApplicationUser> userManager, IHubContext<LoginHub> loginHub
        )
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _dbContext = applicationDbContext;
            _fingerprintAuth = fingerprintAuth;
            _loginHub = loginHub;
        }

        [HttpPost]
        [Route("[controller]")]
        public async Task<IActionResult> PostIndex([FromBody] C2SLoginAttemptDto dto) {
            var response = new GenericResultDto() {
                Success = false,
            };

            // Try to fetch the challenge
            var challenge = await _dbContext.LoginChallenges.FirstOrDefaultAsync(x => x.Id == dto.Id);
            if(challenge == null) {
                response.Message = "Failed to fetch the challenge.";
                return new JsonResult(response);
            }

            // Try to fetch the user allegedly assigned to this challenge
            var user = await _userManager.FindByNameAsync(dto.Username);
            if(user == null) {
                response.Message = $"Cannot find the user {dto.Username} in the database.";
                return new JsonResult(response);
            }

            // Verify the user has a phone already added
            /*if(user.PublicKey == null) {
                response.Message = $"{dto.Username} has no mobile phone added to their account.";
                return new JsonResult(response);
            }

            // Verify the timestamp
            if(_fingerprintAuth.HasLoginChallengeExpired(challenge.ExpiresAt)) {
                response.Message = "Nonce expired. Try logging in again.";
                return new JsonResult(response);
            }

            // Call a single method to do all the verification
            if(!_fingerprintAuth.VerifyLoginNonce(dto, challenge, user)) {
                response.Message = "Nonce verification failed.";
                return new JsonResult(response);
            }*/

            // If all went good, update the challenge row by setting the username and invoke singlaR
            challenge.Username = dto.Username;
            await _dbContext.SaveChangesAsync();
            // Invoke SignalR
            await _loginHub.Clients.Groups(challenge.Id).SendAsync("loginComplete", challenge.Id);

            // All good
            response.Success = true;
            response.Message = "Successfully logged in!";
            return new JsonResult(response);
        }
    }
}
