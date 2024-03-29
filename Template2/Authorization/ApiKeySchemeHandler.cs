﻿using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace Presentation.Authorization;

public class ApiKeySchemeHandler : AuthenticationHandler<ApiKeySchemeOptions>
{
    public ApiKeySchemeHandler(IOptionsMonitor<ApiKeySchemeOptions> options,

        ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
    {
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.ContainsKey(Options.HeaderName))
        {
            return AuthenticateResult.Fail("Header Not Found.");
        }

        var apiKey = Request.Headers[Options.HeaderName];
               

        if (apiKey == "")
        {
            return AuthenticateResult.Fail("Wrong Api Key.");
        }

        var claims = new Claim[]
        {
            new Claim(ClaimTypes.Name, apiKey)
        };

        var identiy = new ClaimsIdentity(claims, nameof(ApiKeySchemeHandler));
        var principal = new ClaimsPrincipal(identiy);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return AuthenticateResult.Success(ticket);
    }
}