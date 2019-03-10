// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using AuthServer.Models;
using Microsoft.AspNetCore.Identity;
using FingerprintAuth.Server;
using FingerprintAuth.Shared.DTO;
using System;
using System.Text;

namespace IdentityServer4.Quickstart.UI
{
    /// <summary>
    /// This sample controller allows a user to revoke grants given to clients
    /// </summary>
    //[SecurityHeaders]
    //[Authorize]   
    public class GrantsController : Controller
    {
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IClientStore _clients;
        private readonly IResourceStore _resources;

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IFingerprintAuth _fingerprintAuth;

        public GrantsController(IIdentityServerInteractionService interaction,
            IClientStore clients,
            IResourceStore resources,
            UserManager<ApplicationUser> userManager,
            IFingerprintAuth fingerprintAuth)
        {
            _interaction = interaction;
            _clients = clients;
            _resources = resources;

            _userManager = userManager;
            _fingerprintAuth = fingerprintAuth;
        }

        /// <summary>
        /// Show list of grants
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var vm = await BuildViewModelAsync();

            var user = await _userManager.GetUserAsync(User);
            if(user == null) {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            // Check if the user had already paired up their device. If so, display a message.
            if(user.PublicKey != null)
                vm.StatusMessage = "You have already paired a device.";

            // If the challenge needs to be refreshed, refresh it.
            if(_fingerprintAuth.NeedsInitializationChallengeRefresh(user)) {
                // Challenge contains a nonce and its expiry datetime
                var newChallenge = _fingerprintAuth.MakeInitializationChallenge();

                // Map this model into a FingerprintUser...
                user.Nonce = newChallenge.Nonce;
                user.NonceExpiresAt = newChallenge.NonceExpiresAt;

                // Update the user in the database!
                await _userManager.UpdateAsync(user);
            }

            // Make the challenge into a base64 encoded png image
            vm.QrCodeBase64Image = _fingerprintAuth.GetInitializationChallenge(user);

            return View("Index", vm);
        }

        /// <summary>
        /// Show list of grants
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> PostIndex([FromBody] C2SKeyExchangeDto clientToServerDto)
        {
            var response = new GenericResultDto() {
                Success = false
            };

            // Get the parameters, decrypted
            var decryptedDto = _fingerprintAuth.GetDecryptedInitializationChallengeParameters(clientToServerDto);
            if(decryptedDto == null) {
                response.Message = "Failed to decrypt the initialization challenge";
                return new JsonResult(response);
            }

            // Fetch the user in question. Convert the username from base64 to a bytearray and then the utf8 bytearray to a string.
            var username = Convert.FromBase64String(decryptedDto.Username);
            var user = await _userManager.FindByNameAsync(Encoding.UTF8.GetString(username));
            if(user == null) {
                response.Message = "Unable find the user in the database";
                return new JsonResult(response);
            }

            // Verify the timestamp first
            if(_fingerprintAuth.HasInitializationChallengeExpired(user.NonceExpiresAt)) {
                response.Message = "Nonce expired. Try pairing up again.";
                return new JsonResult(response);
            }

            // Verify nonces (check if they match)
            if(!_fingerprintAuth.VerifyInitializationNonce(decryptedDto, user.Nonce)) {
                response.Message = "Nonce verification failed.";
                return new JsonResult(response);
            }

            // All good, match the user with their key and update the user, returning a success response
            user.PublicKey = decryptedDto.PublicKey;
            await _userManager.UpdateAsync(user);

            response.Success = true;
            return new JsonResult(response);
        }

        /// <summary>
        /// Handle postback to revoke a client
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Revoke(string clientId)
        {
            await _interaction.RevokeUserConsentAsync(clientId);
            return RedirectToAction("Index");
        }

        private async Task<GrantsViewModel> BuildViewModelAsync()
        {
            var grants = await _interaction.GetAllUserConsentsAsync();

            var list = new List<GrantViewModel>();
            foreach(var grant in grants)
            {
                var client = await _clients.FindClientByIdAsync(grant.ClientId);
                if (client != null)
                {
                    var resources = await _resources.FindResourcesByScopeAsync(grant.Scopes);

                    var item = new GrantViewModel()
                    {
                        ClientId = client.ClientId,
                        ClientName = client.ClientName ?? client.ClientId,
                        ClientLogoUrl = client.LogoUri,
                        ClientUrl = client.ClientUri,
                        Created = grant.CreationTime,
                        Expires = grant.Expiration,
                        IdentityGrantNames = resources.IdentityResources.Select(x => x.DisplayName ?? x.Name).ToArray(),
                        ApiGrantNames = resources.ApiResources.Select(x => x.DisplayName ?? x.Name).ToArray()
                    };

                    list.Add(item);
                }
            }

            return new GrantsViewModel
            {
                Grants = list
            };
        }
    }
}