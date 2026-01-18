using System.Net.Http.Headers;
using FleetlyMobile.Constants;
using System.Net;
using FleetlyMobile.Services.Auth;

namespace FleetlyMobile.Services
{
    public class TokenHandler(IServiceProvider serviceProvider) : DelegatingHandler
    {
        private readonly IServiceProvider _serviceProvider = serviceProvider;
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await SecureStorage.Default.GetAsync(AppConstants.AuthTokenKey);

            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await base.SendAsync(request, cancellationToken);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                if (request.RequestUri != null && (request.RequestUri.AbsolutePath.Contains("/login")))
                {
                    return response;
                }
                var authService = _serviceProvider.GetRequiredService<IAuthService>();
                await authService.LogoutAsync();
            }

            return response;
        }
    }
}