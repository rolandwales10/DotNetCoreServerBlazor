using System.Net.NetworkInformation;
using System.Security.Claims;
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

            if (user.IsInRole("DCN-Programmers"))
            {
                auth = true;
            }
            return auth;
        }
    }
}
