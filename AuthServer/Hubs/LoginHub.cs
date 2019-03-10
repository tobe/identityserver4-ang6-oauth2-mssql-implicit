using System;
using System.Threading.Tasks;
using AuthServer.Data;
using FingerprintAuth.Server;
using FingerprintAuth.Server.Models;
using Microsoft.AspNetCore.SignalR;

namespace FingerprintAuth.Web.Hubs {
    public class LoginHub: Hub {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IFingerprintAuth _fingerprintAuth;

        public LoginHub(ApplicationDbContext applicationDbContext, IFingerprintAuth fingerprintAuth) {
            _applicationDbContext = applicationDbContext;
            _fingerprintAuth = fingerprintAuth;
        }

        public async Task<string> CreateChallenge() {
            // Create a new login challenge in the db
            //Debug.WriteLine("CONNECTIONID: " + Context.ConnectionId);

            // Make a new challenge
            var loginChallenge = _fingerprintAuth.MakeLoginChallenge();

            // Make a new row in the database for this challenge (db model)
            var challengeRow = new LoginChallenge() {
                ExpiresAt = loginChallenge.ExpiresAt,
                Nonce = loginChallenge.Nonce,
                ParameterG = loginChallenge.ParameterG,
                ParameterP = loginChallenge.ParameterP,
                PrivateKey = loginChallenge.PrivateKey
            };

            // Insert
            _applicationDbContext.Add(challengeRow);
            _applicationDbContext.SaveChanges();

            // Now get the challenge as a qr code
            var qrCode = _fingerprintAuth.GetLoginChallenge(loginChallenge, challengeRow);

            // Map this client by the newly created challenge GUID
            //await Groups.AddToGroupAsync(Context.ConnectionId, challengeRow.Id);
            await Groups.AddToGroupAsync(Context.ConnectionId, "5652dbca-5931-412d-9042-4b8deba7c0f1");

            // And return the JSON payload!
            return qrCode;
        }

        public Task ValidateChallenge(Guid challengeGuid) {
            return Clients.Group(challengeGuid.ToString()).SendAsync("updatmee");
        }
    }
}