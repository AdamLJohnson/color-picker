namespace ColorPicker.App.Services;

using System;
using System.Drawing;
using System.Globalization;
using System.Text.RegularExpressions;

public static class ColorNamer
{
    public readonly record struct Rgb(byte R, byte G, byte B);
    public readonly record struct Lab(double L, double A, double B);

    public static string ClosestSystemDrawingColorName(string hex, bool includeSystemColors = false)
    {
        var rgb = ParseHex(hex);
        var lab = RgbToLab(rgb);

        string bestName = "";
        double best = double.PositiveInfinity;

        foreach (KnownColor kc in Enum.GetValues(typeof(KnownColor)))
        {
            var c = Color.FromKnownColor(kc);

            // Skip anything that isn't a real known color entry
            if (!c.IsKnownColor) continue;

            // Skip system colors unless explicitly included
            if (!includeSystemColors && c.IsSystemColor) continue;

            // Some entries can be "empty"/0 alpha; skip if you want
            if (c.A == 0) continue;

            var namedLab = RgbToLab(new Rgb(c.R, c.G, c.B));
            //var dE = DeltaE76(lab, namedLab);
            var dE = DeltaE2000(lab, namedLab);

            if (dE < best)
            {
                best = dE;
                bestName = kc.ToString();
            }
        }

        return ToTitleCaseWithSpaces(bestName);
    }

    static string ToTitleCaseWithSpaces(string pascalCase)
    {
        return Regex.Replace(pascalCase, "(?<!^)([A-Z])", " $1");
    }

    // --- Parsing ---

    private static Rgb ParseHex(string hex)
    {
        if (hex is null) throw new ArgumentNullException(nameof(hex));
        hex = hex.Trim();

        if (hex.StartsWith("#", StringComparison.Ordinal))
            hex = hex[1..];

        // Support shorthand #RGB
        if (hex.Length == 3)
            hex = string.Concat(hex[0], hex[0], hex[1], hex[1], hex[2], hex[2]);

        if (hex.Length != 6)
            throw new FormatException("Hex must be RRGGBB or RGB (optionally prefixed with #).");

        byte r = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
        byte g = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
        byte b = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);

        return new Rgb(r, g, b);
    }

    // --- Color math: sRGB -> Linear -> XYZ(D65) -> Lab ---

    private static Lab RgbToLab(Rgb rgb)
    {
        double sr = rgb.R / 255.0;
        double sg = rgb.G / 255.0;
        double sb = rgb.B / 255.0;

        double r = SrgbToLinear(sr);
        double g = SrgbToLinear(sg);
        double b = SrgbToLinear(sb);

        // linear RGB -> XYZ (D65), matrix for sRGB
        double x = r * 0.4124564 + g * 0.3575761 + b * 0.1804375;
        double y = r * 0.2126729 + g * 0.7151522 + b * 0.0721750;
        double z = r * 0.0193339 + g * 0.1191920 + b * 0.9503041;

        // D65 reference white
        const double Xn = 0.95047;
        const double Yn = 1.00000;
        const double Zn = 1.08883;

        double fx = Fxyz(x / Xn);
        double fy = Fxyz(y / Yn);
        double fz = Fxyz(z / Zn);

        double L = 116 * fy - 16;
        double A = 500 * (fx - fy);
        double B = 200 * (fy - fz);

        return new Lab(L, A, B);
    }

    private static double SrgbToLinear(double c)
        => (c <= 0.04045) ? (c / 12.92) : Math.Pow((c + 0.055) / 1.055, 2.4);

    private static double Fxyz(double t)
    {
        const double delta = 6.0 / 29.0;
        double delta3 = delta * delta * delta;
        return (t > delta3) ? Math.Pow(t, 1.0 / 3.0) : (t / (3 * delta * delta) + 4.0 / 29.0);
    }

    // ΔE76 (simple Euclidean distance in Lab)
    private static double DeltaE76(Lab a, Lab b)
    {
        double dL = a.L - b.L;
        double dA = a.A - b.A;
        double dB = a.B - b.B;
        return Math.Sqrt(dL * dL + dA * dA + dB * dB);
    }

    // --- ΔE2000 (CIEDE2000) ---

    public static double DeltaE2000(Lab lab1, Lab lab2)
    {
        // Weighting factors (usually 1)
        const double kL = 1.0;
        const double kC = 1.0;
        const double kH = 1.0;

        double L1 = lab1.L, a1 = lab1.A, b1 = lab1.B;
        double L2 = lab2.L, a2 = lab2.A, b2 = lab2.B;

        double C1 = Math.Sqrt(a1 * a1 + b1 * b1);
        double C2 = Math.Sqrt(a2 * a2 + b2 * b2);
        double Cbar = (C1 + C2) / 2.0;

        double Cbar7 = Math.Pow(Cbar, 7.0);
        double G = 0.5 * (1.0 - Math.Sqrt(Cbar7 / (Cbar7 + Math.Pow(25.0, 7.0))));

        double a1p = (1.0 + G) * a1;
        double a2p = (1.0 + G) * a2;

        double C1p = Math.Sqrt(a1p * a1p + b1 * b1);
        double C2p = Math.Sqrt(a2p * a2p + b2 * b2);

        double h1p = Hp(b1, a1p);
        double h2p = Hp(b2, a2p);

        double dLp = L2 - L1;
        double dCp = C2p - C1p;

        double dhp = DeltaHp(C1p, C2p, h1p, h2p);
        double dHp = 2.0 * Math.Sqrt(C1p * C2p) * Math.Sin(DegToRad(dhp / 2.0));

        double LpBar = (L1 + L2) / 2.0;
        double CpBar = (C1p + C2p) / 2.0;

        double hpBar = HBar(C1p, C2p, h1p, h2p);

        double T = 1.0
                 - 0.17 * Math.Cos(DegToRad(hpBar - 30.0))
                 + 0.24 * Math.Cos(DegToRad(2.0 * hpBar))
                 + 0.32 * Math.Cos(DegToRad(3.0 * hpBar + 6.0))
                 - 0.20 * Math.Cos(DegToRad(4.0 * hpBar - 63.0));

        double dTheta = 30.0 * Math.Exp(-Math.Pow((hpBar - 275.0) / 25.0, 2.0));
        double Rc = 2.0 * Math.Sqrt(Math.Pow(CpBar, 7.0) / (Math.Pow(CpBar, 7.0) + Math.Pow(25.0, 7.0)));

        double Sl = 1.0 + (0.015 * Math.Pow(LpBar - 50.0, 2.0)) / Math.Sqrt(20.0 + Math.Pow(LpBar - 50.0, 2.0));
        double Sc = 1.0 + 0.045 * CpBar;
        double Sh = 1.0 + 0.015 * CpBar * T;

        double Rt = -Math.Sin(DegToRad(2.0 * dTheta)) * Rc;

        double dL = dLp / (kL * Sl);
        double dC = dCp / (kC * Sc);
        double dH = dHp / (kH * Sh);

        return Math.Sqrt(dL * dL + dC * dC + dH * dH + Rt * dC * dH);
    }

    private static double Hp(double b, double ap)
    {
        // hue angle in degrees [0..360)
        if (ap == 0 && b == 0) return 0.0;
        double h = RadToDeg(Math.Atan2(b, ap));
        if (h < 0) h += 360.0;
        return h;
    }

    private static double DeltaHp(double C1p, double C2p, double h1p, double h2p)
    {
        if (C1p == 0 || C2p == 0) return 0.0;

        double dh = h2p - h1p;
        if (dh > 180.0) dh -= 360.0;
        if (dh < -180.0) dh += 360.0;
        return dh;
    }

    private static double HBar(double C1p, double C2p, double h1p, double h2p)
    {
        if (C1p == 0 || C2p == 0) return h1p + h2p;

        double hSum = h1p + h2p;
        double hDiff = Math.Abs(h1p - h2p);

        if (hDiff <= 180.0) return hSum / 2.0;

        // If > 180, wrap around 360
        if (hSum < 360.0) return (hSum + 360.0) / 2.0;
        return (hSum - 360.0) / 2.0;
    }

    private static double DegToRad(double deg) => deg * (Math.PI / 180.0);
    private static double RadToDeg(double rad) => rad * (180.0 / Math.PI);
}
