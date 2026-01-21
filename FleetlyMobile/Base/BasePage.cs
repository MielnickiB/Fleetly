using Microsoft.AspNetCore.Components;
using FleetlyMobile.Components.PagesComponents.Dialogs;
using FleetlyMobile.Helpers;
using MudBlazor;

namespace FleetlyMobile.Base
{
    public abstract class BasePage : ComponentBase
    {
        [Inject] protected ISnackbar Snackbar { get; set; } = default!;
        [Inject] protected NavigationManager Nav { get; set; } = default!;
        [Inject] protected IDialogService DialogService { get; set; } = default!;

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

        protected static string BuildImageUrl(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return string.Empty;

            var safePath = fileName.Replace("\\", "/");
            return safePath;
        }

        protected async Task OpenImagePreviewAsync(string? fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                Snackbar.Add("Brak zdjęcia.", Severity.Warning);
                return;
            }

            string fullUrl = BuildImageUrl(fileName);

            var parameters = new DialogParameters
            {
                ["ImageUrl"] = fullUrl
            };

            await DialogService.ShowAsync<ImagePreviewDialog>("Podgląd", parameters, DialogHelper.GetImagePreviewOptions());
        }
    }
}