using Fleetly.Shared.Dto;
using Fleetly.Shared.Dto.OrderDtos;
using Microsoft.AspNetCore.Components.Authorization;

namespace FleetlyWeb.Services
{
    public class OrderService(ApiClient api, AuthenticationStateProvider auth) : IOrderService
    {
        private readonly ApiClient _api = api;
        private readonly AuthenticationStateProvider _auth = auth;
        public async Task<PagedResult<OrderResponseDto>> GetAllOrdersAsync()
        {
            var resp = await _api.GetAsync<PagedResult<OrderResponseDto>>("api/admin/orders");
            if (!resp.Success)
            {
                throw new Exception(resp.Error ?? "Nie udało się pobrać zleceń.");
            }
            if (resp.Data is null)
            {
                throw new Exception("Nie udało się pobrać zleceń.");
            }
            return resp.Data!;
        }

        public async Task<OrderResponseDto?> GetOrderByIdAdminAsync(int orderId)
        {
            var resp = await _api.GetAsync<OrderResponseDto?>($"api/admin/orders/{orderId}")
                ?? throw new Exception("Brak odpowiedzi z serwera.");
            if (!resp.Success) throw new Exception(resp.Error ?? "Nie udało się pobrać zlecenia.");
            return resp.Data;
        }

        public async Task<OrderResponseDto?> GetOrderByIdClientAsync(int orderId)
        {
            var resp = await _api.GetAsync<OrderResponseDto?>($"api/client/orders/{orderId}/details")
                ?? throw new Exception("Brak odpowiedzi z serwera.");
            if (!resp.Success) throw new Exception(resp.Error ?? "Nie udało się pobrać zlecenia.");
            return resp.Data;
        }

        public async Task<OrderResponseDto?> GetOrderByIdWorkerAsync(int orderId)
        {
            var resp = await _api.GetAsync<OrderResponseDto?>($"api/worker/orders/{orderId}")
                ?? throw new Exception("Brak odpowiedzi z serwera.");
            if (!resp.Success) throw new Exception(resp.Error ?? "Nie udało się pobrać zlecenia.");
            return resp.Data;
        }

        public async Task<OrderResponseDto?> GetOrderByIdAsync(int orderId)
        {
            var authState = await _auth.GetAuthenticationStateAsync();
            var user = authState.User;

            if (user.IsInRole("Admin"))
                return await GetOrderByIdAdminAsync(orderId);
            if (user.IsInRole("Client"))
                return await GetOrderByIdClientAsync(orderId);
            if (user.IsInRole("Worker"))
                return await GetOrderByIdWorkerAsync(orderId);

            throw new UnauthorizedAccessException("Brak uprawnień do przeglądania zlecenia.");
        }
    }
}
