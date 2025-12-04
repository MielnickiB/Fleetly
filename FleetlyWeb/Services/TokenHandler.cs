using System.Net.Http.Headers;

namespace FleetlyWeb.Services
{
    public class TokenHandler(ILocalStorage storage) : DelegatingHandler
    {
        private readonly ILocalStorage _storage = storage;

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await _storage.GetTokenAsync();
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }

}
