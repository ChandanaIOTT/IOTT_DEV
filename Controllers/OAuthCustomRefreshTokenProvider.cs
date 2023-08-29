﻿using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Web.Http.Cors;

namespace TokenBasedAuthentication.Providers
{
    [EnableCors(origins: "", headers: "", methods: "*")]
    public class OAuthCustomRefreshTokenProvider : IAuthenticationTokenProvider
    {

        // Add a static variable
        private static ConcurrentDictionary<string, AuthenticationTicket> _refreshTokens = new ConcurrentDictionary<string, AuthenticationTicket>();
        public async Task CreateAsync(AuthenticationTokenCreateContext context)
        {
            var guid = Guid.NewGuid().ToString();
            /* Copy claims from previous token
             ***********************************/
            var refreshTokenProperties = new AuthenticationProperties(context.Ticket.Properties.Dictionary)
            {
                IssuedUtc = context.Ticket.Properties.IssuedUtc,
                ExpiresUtc = DateTime.UtcNow.AddMinutes(40)
            };
            var refreshTokenTicket = await Task.Run(() => new AuthenticationTicket(context.Ticket.Identity, refreshTokenProperties));

            _refreshTokens.TryAdd(guid, refreshTokenTicket);

            // consider storing only the hash of the handle  
            context.SetToken(guid);
        }
        [EnableCors(origins: "*", headers: "*", methods: "*")]

        public async Task ReceiveAsync(AuthenticationTokenReceiveContext context)
        {
            AuthenticationTicket ticket;
            string header = await Task.Run(() => context.OwinContext.Request.Headers["Authorization"]);

            if (_refreshTokens.TryRemove(context.Token, out ticket))
                context.SetTicket(ticket);
        }
        [EnableCors(origins: "*", headers: "*", methods: "*")]

        public void Create(AuthenticationTokenCreateContext context)
        {
            throw new NotImplementedException();
        }
        [EnableCors(origins: "*", headers: "*", methods: "*")]

        public void Receive(AuthenticationTokenReceiveContext context)
        {
            throw new NotImplementedException();
        }


    }
}