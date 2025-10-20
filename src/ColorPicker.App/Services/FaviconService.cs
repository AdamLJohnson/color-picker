using Microsoft.JSInterop;

namespace ColorPicker.App.Services;

/// <summary>
/// Service for dynamically updating the browser favicon.
/// Uses JavaScript interop to generate and update SVG-based favicons.
/// </summary>
public class FaviconService : IAsyncDisposable
{
    private readonly IJSRuntime _jsRuntime;
    private IJSObjectReference? _module;

    /// <summary>
    /// Initializes a new instance of the FaviconService class.
    /// </summary>
    /// <param name="jsRuntime">The JavaScript runtime for interop.</param>
    public FaviconService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    /// <summary>
    /// Initializes the favicon service by loading the JavaScript module.
    /// This should be called once when the service is first used.
    /// </summary>
    private async Task EnsureModuleLoaded()
    {
        if (_module == null)
        {
            _module = await _jsRuntime.InvokeAsync<IJSObjectReference>("import", "./js/favicon.js");
        }
    }

    /// <summary>
    /// Updates the favicon to a solid colored square with the specified hex color.
    /// </summary>
    /// <param name="hexColor">The hex color code (e.g., "#FF0000").</param>
    public async Task UpdateFaviconAsync(string hexColor)
    {
        try
        {
            await EnsureModuleLoaded();
            if (_module != null)
            {
                await _module.InvokeVoidAsync("updateFavicon", hexColor);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating favicon: {ex.Message}");
        }
    }

    /// <summary>
    /// Updates the favicon to a solid colored circle with the specified hex color.
    /// </summary>
    /// <param name="hexColor">The hex color code (e.g., "#FF0000").</param>
    public async Task UpdateFaviconWithCircleAsync(string hexColor)
    {
        try
        {
            await EnsureModuleLoaded();
            if (_module != null)
            {
                await _module.InvokeVoidAsync("updateFaviconWithCircle", hexColor);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating favicon: {ex.Message}");
        }
    }

    /// <summary>
    /// Disposes the JavaScript module reference.
    /// </summary>
    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        if (_module is not null)
        {
            await _module.DisposeAsync();
        }
    }
}

