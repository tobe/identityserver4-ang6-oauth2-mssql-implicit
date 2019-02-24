using AuthServer.Models;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthServer {
    public class ProfileService: IProfileService {
        protected UserManager<ApplicationUser> _userManager;
        protected RoleManager<IdentityRole> _roleManager;

        public ProfileService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager) {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context) {
            //>Processing
            var user = await _userManager.GetUserAsync(context.Subject);
            var userRoles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                //new Claim("FullName", user.FullName),
                new Claim("Role", userRoles[0]),
                new Claim("Role", "Overridden"),
                new Claim("Avion", "[\"Pariz\", \"Zagreb\"]"),
                new Claim("CanRead", "yes"),
                new Claim("CanWrite", "yes"),
                new Claim("CanDelete", "yes") // dodao sam ovo, ovo ce ic dinamicki jel
            };

            context.IssuedClaims.AddRange(claims);
        }

        public async Task IsActiveAsync(IsActiveContext context) {
            //>Processing
            var user = await _userManager.GetUserAsync(context.Subject);

            context.IsActive = (user != null)/* && user.Active*/;
        }
    }
}
