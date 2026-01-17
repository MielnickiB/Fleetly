using Blazored.LocalStorage;
using FleetlyWeb.Constants;
using System.Net;
using System.Net.Http.Headers;
using FleetlyWeb.Services.Authorization;

namespace FleetlyWeb.Services
{
    public class TokenHandler(ILocalStorageService localStorage, IServiceProvider serviceProvider) : DelegatingHandler
    {
        private readonly ILocalStorageService _localStorage = localStorage;
        private readonly IServiceProvider _serviceProvider = serviceProvider;

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await _localStorage.GetItemAsync<string>(StorageKeys.AccessToken, cancellationToken);

            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await base.SendAsync(request, cancellationToken);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                var authService = _serviceProvider.GetRequiredService<IAuthService>();
                await authService.Logout();
            }

            return response;
        }
    }

}
