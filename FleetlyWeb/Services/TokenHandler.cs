using System.Net.Http.Headers;
using Blazored.LocalStorage;

namespace FleetlyWeb.Services
{
    public class TokenHandler(ILocalStorageService localStorage) : DelegatingHandler
    {
        private readonly ILocalStorageService _localStorage = localStorage;

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await _localStorage.GetItemAsync<string>("fleetly_token", cancellationToken);

            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }

}
