using MudBlazor;

namespace FleetlyMobile.Base
{
    public class CostBasePage : BasePage, IDisposable
    {
        protected string? LocalFilePath;
        protected string? OriginalFileName;

        protected bool ShowFileError = false;

        protected async Task TakePhotoAsync()
        {
            try
            {
                if (MediaPicker.Default.IsCaptureSupported)
                {
                    var photo = await MediaPicker.Default.CapturePhotoAsync();
                    if (photo != null)
                    {
                        CleanUpOldFile();
                        await CacheFileAsync(photo);
                    }
                }
                else
                {
                    Snackbar.Add("Brak dostępu do aparatu.", Severity.Warning);
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Błąd aparatu: {ex.Message}", Severity.Error);
            }
        }

        protected async Task PickFileAsync()
        {
            try
            {
                var customFileType = new FilePickerFileType(
                    new Dictionary<DevicePlatform, IEnumerable<string>>
                    {
                        { DevicePlatform.iOS, new[] { "public.image", "com.adobe.pdf" } },
                        { DevicePlatform.Android, new[] { "image/*", "application/pdf" } }
                    });

                var options = new PickOptions
                {
                    PickerTitle = "Wybierz dowód zakupu",
                    FileTypes = customFileType
                };

                var file = await FilePicker.Default.PickAsync(options);

                if (file != null)
                {
                    CleanUpOldFile();
                    await CacheFileAsync(file);
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Błąd wyboru pliku: {ex.Message}", Severity.Error);
            }
        }

        private async Task CacheFileAsync(FileResult fileResult)
        {
            var newFile = Path.Combine(FileSystem.CacheDirectory, fileResult.FileName);

            using var stream = await fileResult.OpenReadAsync();
            using var newStream = File.OpenWrite(newFile);

            await stream.CopyToAsync(newStream);

            LocalFilePath = newFile;
            OriginalFileName = fileResult.FileName;
            ShowFileError = false;
            StateHasChanged();
        }

        protected void ClearFile()
        {
            if (LocalFilePath != null && File.Exists(LocalFilePath))
            {
                try 
                { 
                    File.Delete(LocalFilePath); 
                } 
                catch { }
            }

            LocalFilePath = null;
            OriginalFileName = null;
            StateHasChanged();
        }

        private void CleanUpOldFile()
        {
            if (!string.IsNullOrEmpty(LocalFilePath) && File.Exists(LocalFilePath))
            {
                try
                {
                    File.Delete(LocalFilePath);
                }
                catch { }
            }
        }

        public void Dispose()
        {
            CleanUpOldFile();
            GC.SuppressFinalize(this);
        }
    }
}