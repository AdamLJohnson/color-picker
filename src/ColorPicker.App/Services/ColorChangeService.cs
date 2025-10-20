using ColorPicker.App.Models;

namespace ColorPicker.App.Services;

/// <summary>
/// Service for managing color changes and notifying subscribers of color updates.
/// Implements the observer pattern to allow components to react to color changes.
/// </summary>
public class ColorChangeService
{
    /// <summary>
    /// Event that fires when the user selects a new color.
    /// </summary>
    public event EventHandler<ColorChangedEventArgs>? OnColorChanged;

    /// <summary>
    /// Notifies all subscribers that the color has changed.
    /// </summary>
    /// <param name="cssColors">The updated CssColors instance with all color variants.</param>
    public void NotifyColorChanged(CssColors cssColors)
    {
        OnColorChanged?.Invoke(this, new ColorChangedEventArgs(cssColors));
    }
}

/// <summary>
/// Event arguments for color change events.
/// </summary>
public class ColorChangedEventArgs : EventArgs
{
    /// <summary>
    /// Gets the CssColors instance containing all color variants.
    /// </summary>
    public CssColors CssColors { get; }

    /// <summary>
    /// Initializes a new instance of the ColorChangedEventArgs class.
    /// </summary>
    /// <param name="cssColors">The CssColors instance with all color variants.</param>
    public ColorChangedEventArgs(CssColors cssColors)
    {
        CssColors = cssColors;
    }
}

