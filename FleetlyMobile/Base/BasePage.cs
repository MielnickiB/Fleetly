using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace FleetlyMobile.Base
{
    public abstract class BasePage : ComponentBase
    {
        [Inject] protected ISnackbar Snackbar { get; set; } = default!;
        [Inject] protected NavigationManager Nav { get; set; } = default!;

        protected bool IsLoading { get; set; } = false;

        protected async Task ExecuteSafeAsync(Func<Task> action, string? successMessage = null)
        {
            try
            {
                IsLoading = true;
                StateHasChanged();

                await action();

                if (!string.IsNullOrEmpty(successMessage))
                {
                    Snackbar.Add(successMessage, Severity.Success);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Wystąpił błąd: {ex.Message}");
                Snackbar.Add($"Wystąpił błąd: {ex.Message}", Severity.Error);
            }
            finally
            {
                IsLoading = false;
                StateHasChanged();
            }
        }
    }
}