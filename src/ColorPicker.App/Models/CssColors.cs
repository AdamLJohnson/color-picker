using System.Drawing;

namespace ColorPicker.App.Models;

/// <summary>
/// Provides CSS color utilities including text color contrast calculation, color blending, and luminance computation.
/// Implements WCAG 2.0 contrast ratio calculations for accessibility compliance.
/// </summary>
public class CssColors
{
    // Luminance calculation constants (WCAG 2.0 standard)
    private const double RedLuminanceWeight = 0.2126;
    private const double GreenLuminanceWeight = 0.7152;
    private const double BlueLuminanceWeight = 0.0722;
    private const double LuminanceThreshold = 0.03928;
    private const double LuminanceDivisor = 12.92;
    private const double LuminanceOffset = 0.055;
    private const double LuminanceGamma = 1.055;
    private const double LuminanceExponent = 2.4;
    private const double ContrastRatioThreshold = 2.0 / 9.0;
    private const double LuminanceAdjustment = 0.05;

    private Color _mainColor;
    private readonly Color _blackColor = ColorTranslator.FromHtml("#000000");
    private readonly Color _whiteColor = ColorTranslator.FromHtml("#FFFFFF");

    /// <summary>
    /// Gets the recommended text color (black or white) for optimal contrast with the main color.
    /// </summary>
    public string TextColor => GetTextColor(_mainColor);

    /// <summary>
    /// Gets or sets the main color as an HTML color code.
    /// </summary>
    public string MainColor
    {
        get => ColorTranslator.ToHtml(_mainColor);
        set => SetColor(value);
    }

    /// <summary>
    /// Gets a light tint of the main color (15% opacity over white).
    /// </summary>
    public string MainColor10 => GetBlendedColorCode(0.15, blendWithBlack: false);

    /// <summary>
    /// Gets a light tint of the main color (35% opacity over white).
    /// </summary>
    public string MainColor20 => GetBlendedColorCode(0.35, blendWithBlack: false);

    /// <summary>
    /// Gets a medium tint of the main color (65% opacity over white).
    /// </summary>
    public string MainColor30 => GetBlendedColorCode(0.65, blendWithBlack: false);

    /// <summary>
    /// Gets a medium tint of the main color (80% opacity over white).
    /// </summary>
    public string MainColor40 => GetBlendedColorCode(0.8, blendWithBlack: false);

    /// <summary>
    /// Gets a dark shade of the main color (65% opacity over black).
    /// </summary>
    public string MainColor60 => GetBlendedColorCode(0.65, blendWithBlack: true);

    /// <summary>
    /// Gets a dark shade of the main color (35% opacity over black).
    /// </summary>
    public string MainColor70 => GetBlendedColorCode(0.35, blendWithBlack: true);

    /// <summary>
    /// Gets a dark shade of the main color (15% opacity over black).
    /// </summary>
    public string MainColor80 => GetBlendedColorCode(0.15, blendWithBlack: true);

    /// <summary>
    /// Initializes a new instance of the CssColors class with the specified color.
    /// </summary>
    /// <param name="color">The initial color to use.</param>
    public CssColors(Color color) => _mainColor = color;

    /// <summary>
    /// Sets the main color from an HTML color code.
    /// </summary>
    /// <param name="htmlColorCode">The HTML color code (e.g., "#FF0000").</param>
    private void SetColor(string htmlColorCode) => _mainColor = ColorTranslator.FromHtml(htmlColorCode);

    /// <summary>
    /// Gets the recommended text color (black or white) for optimal contrast with the specified color.
    /// </summary>
    /// <param name="htmlColorCode">The HTML color code to check contrast against.</param>
    /// <returns>The HTML color code for the recommended text color.</returns>
    public string GetTextColor(string htmlColorCode) => GetTextColor(ColorTranslator.FromHtml(htmlColorCode));

    /// <summary>
    /// Gets the recommended text color (black or white) for optimal contrast with the specified color.
    /// </summary>
    /// <param name="color">The color to check contrast against.</param>
    /// <returns>The HTML color code for the recommended text color.</returns>
    public string GetTextColor(Color color)
    {
        var contrastRatio = CalculateContrastRatio(_whiteColor, color);
        return contrastRatio < ContrastRatioThreshold
            ? ColorTranslator.ToHtml(_whiteColor)
            : ColorTranslator.ToHtml(_blackColor);
    }

    /// <summary>
    /// Gets a secondary text color that complements the specified color.
    /// </summary>
    /// <param name="htmlColorCode">The HTML color code to complement.</param>
    /// <returns>The HTML color code for the secondary text color.</returns>
    public string GetSecondaryTextColor(string htmlColorCode)
    {
        return GetSecondaryTextColor(ColorTranslator.FromHtml(htmlColorCode));
    }

    /// <summary>
    /// Gets a secondary text color that complements the specified color.
    /// </summary>
    /// <param name="color">The color to complement.</param>
    /// <returns>The HTML color code for the secondary text color.</returns>
    public string GetSecondaryTextColor(Color color)
    {
        var contrastRatio = CalculateContrastRatio(_whiteColor, color);
        return contrastRatio < ContrastRatioThreshold
            ? ColorTranslator.ToHtml(color)
            : ColorTranslator.ToHtml(_blackColor);
    }

    /// <summary>
    /// Gets a blended color code by blending the main color with either black or white.
    /// </summary>
    /// <param name="opacity">The opacity of the main color (0.0 to 1.0).</param>
    /// <param name="blendWithBlack">If true, blends with black; otherwise blends with white.</param>
    /// <returns>The HTML color code of the blended color.</returns>
    private string GetBlendedColorCode(double opacity, bool blendWithBlack)
    {
        var backgroundColor = blendWithBlack ? _blackColor : _whiteColor;
        var blendedColor = BlendColors(_mainColor, backgroundColor, opacity);
        return ColorTranslator.ToHtml(blendedColor);
    }

    /// <summary>
    /// Blends two colors together using the specified opacity.
    /// </summary>
    /// <param name="overlayColor">The overlay color.</param>
    /// <param name="backgroundColor">The background color.</param>
    /// <param name="opacity">The opacity of the overlay color (0.0 to 1.0).</param>
    /// <returns>The resulting blended color.</returns>
    public static Color BlendColors(Color overlayColor, Color backgroundColor, double opacity)
    {
        var red = BlendColorComponent(overlayColor.R, backgroundColor.R, opacity);
        var green = BlendColorComponent(overlayColor.G, backgroundColor.G, opacity);
        var blue = BlendColorComponent(overlayColor.B, backgroundColor.B, opacity);

        return Color.FromArgb(red, green, blue);
    }

    /// <summary>
    /// Blends a single color component (R, G, or B) using the specified opacity.
    /// </summary>
    /// <param name="overlayComponent">The overlay component value (0-255).</param>
    /// <param name="backgroundComponent">The background component value (0-255).</param>
    /// <param name="opacity">The opacity of the overlay (0.0 to 1.0).</param>
    /// <returns>The blended component value.</returns>
    private static int BlendColorComponent(int overlayComponent, int backgroundComponent, double opacity)
    {
        return (int)Math.Round(opacity * overlayComponent + (1.0 - opacity) * backgroundComponent);
    }

    /// <summary>
    /// Calculates the WCAG 2.0 contrast ratio between two colors.
    /// </summary>
    /// <param name="colorOne">The first color.</param>
    /// <param name="colorTwo">The second color.</param>
    /// <returns>The contrast ratio (typically between 1 and 21).</returns>
    private double CalculateContrastRatio(Color colorOne, Color colorTwo)
    {
        var luminance1 = CalculateLuminance(colorOne);
        var luminance2 = CalculateLuminance(colorTwo);

        var lighterLuminance = Math.Min(luminance1, luminance2);
        var darkerLuminance = Math.Max(luminance1, luminance2);

        return (lighterLuminance + LuminanceAdjustment) / (darkerLuminance + LuminanceAdjustment);
    }

    /// <summary>
    /// Calculates the relative luminance of a color according to WCAG 2.0 standards.
    /// </summary>
    /// <param name="color">The color to calculate luminance for.</param>
    /// <returns>The relative luminance value (0.0 to 1.0).</returns>
    private double CalculateLuminance(Color color)
    {
        var redLuminance = CalculateComponentLuminance(color.R);
        var greenLuminance = CalculateComponentLuminance(color.G);
        var blueLuminance = CalculateComponentLuminance(color.B);

        return (RedLuminanceWeight * redLuminance)
            + (GreenLuminanceWeight * greenLuminance)
            + (BlueLuminanceWeight * blueLuminance);
    }

    /// <summary>
    /// Calculates the luminance of a single color component according to WCAG 2.0 standards.
    /// </summary>
    /// <param name="componentValue">The component value (0-255).</param>
    /// <returns>The component luminance value.</returns>
    private double CalculateComponentLuminance(int componentValue)
    {
        var normalizedValue = componentValue / 255.0;

        if (normalizedValue <= LuminanceThreshold)
        {
            return normalizedValue / LuminanceDivisor;
        }

        return Math.Pow((normalizedValue + LuminanceOffset) / LuminanceGamma, LuminanceExponent);
    }
}
