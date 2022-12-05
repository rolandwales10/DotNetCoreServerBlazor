using System.Net.NetworkInformation;
using System.Security.Claims;
using FarmshareAdmin.Models;
using Microsoft.AspNetCore.Components.Authorization;

namespace FarmshareAdmin.Utilities
{
    public class Authorization
    {
        public AuthenticationStateProvider _AuthenticationStateProvider;
        public Authorization(AuthenticationStateProvider AuthenticationStateProvider)
        {
            _AuthenticationStateProvider = AuthenticationStateProvider;
        }

        public async Task<bool> IsAdmin()
        {
            bool auth = false;
            var authState = await _AuthenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            if (user.IsInRole("AGR-FarmShare-DataEntry") || user.IsInRole("DCN-Programmers")
                || user.IsInRole("OIT-CTS-ALL"))  //  oit-cts-all is for security and accessibility testing only.
            {
                auth = true;
            }
            return auth;
        }
    }
}
