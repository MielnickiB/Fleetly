using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;

namespace FleetlyMobile.Base;

public abstract class DialogBase<TModel> : ComponentBase where TModel : class, new()
{
    [CascadingParameter] protected IMudDialogInstance MudDialog { get; set; } = default!;
    [Inject] protected ISnackbar Snackbar { get; set; } = default!;

    protected TModel Model { get; set; } = new();
    protected bool IsBusy = false;

    protected EditForm? EditForm;
    protected EditContext? EditContext;

    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender && EditForm?.EditContext != null)
        {
            EditContext = EditForm.EditContext;
            StateHasChanged();
        }
    }

    protected void Cancel() => MudDialog.Cancel();

    protected async Task SubmitForm()
    {
        if (EditContext is null) return;

        if (!EditContext.Validate())
        {
            Snackbar.Add("Popraw błędy w formularzu", Severity.Warning);
            return;
        }

        IsBusy = true;
        try
        {
            await Submit();
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Wystąpił nieoczekiwany błąd: {ex.Message}", Severity.Error);
        }
        finally
        {
            IsBusy = false;
            StateHasChanged();
        }
    }

    protected abstract Task Submit();
}