# 🎨 ColorPicker

An interactive color picker web application built with Blazor WebAssembly that demonstrates WCAG 2.0 compliant color contrast calculations and automatic color variant generation.

## ✨ Features

- **Interactive Color Selection**: Native HTML5 color picker for seamless color selection
- **WCAG 2.0 Contrast Compliance**: Automatically calculates optimal text colors (black or white) based on background luminance
- **Color Variant Generation**: Generates 8 color variants by blending the selected color with black and white at different opacity levels:
  - Light tints (10%, 20%, 30%, 40%)
  - Dark shades (60%, 70%, 80%)
- **Dynamic UI Updates**: Navigation and favicon update in real-time to reflect the selected color
- **Accessibility-Focused**: Ensures all text meets WCAG 2.0 contrast ratio requirements for readability
- **Responsive Design**: Built with Bootstrap 5 for a mobile-friendly experience

## 🛠️ Technology Stack

- **Framework**: [Blazor WebAssembly](https://docs.microsoft.com/aspnet/core/blazor/) (.NET 10)
- **Language**: C# 
- **UI Framework**: Bootstrap 5
- **Hosting**: GitHub Pages (via GitHub Actions CI/CD)

## 📁 Project Structure

```
ColorPicker/
├── ColorPicker.sln                    # Solution file
├── .github/
│   └── workflows/
│       └── deploy-to-github-pages.yml # GitHub Actions deployment workflow
└── src/
    └── ColorPicker.App/
        ├── Layout/                    # Blazor layout components
        │   ├── MainLayout.razor
        │   └── NavMenu.razor
        ├── Models/
        │   └── CssColors.cs           # Core color utility class with WCAG calculations
        ├── Pages/
        │   └── Home.razor             # Main interactive color picker page
        ├── Services/
        │   ├── ColorChangeService.cs  # Event service for color change notifications
        │   └── FaviconService.cs      # Dynamic favicon update service
        ├── wwwroot/                   # Static web assets
        │   ├── css/
        │   ├── js/
        │   └── index.html
        └── Program.cs                 # Application entry point
```

## 🚀 Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) or later

### Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/yourusername/ColorPicker.git
   cd ColorPicker
   ```

2. Restore dependencies:
   ```bash
   dotnet restore
   ```

3. Run the application:
   ```bash
   dotnet run --project src/ColorPicker.App
   ```

4. Open your browser and navigate to:
   - HTTP: `http://localhost:5147`
   - HTTPS: `https://localhost:7127`

## 🔧 Building for Production

To build a production-ready release:

```bash
dotnet publish src/ColorPicker.App/ColorPicker.App.csproj -c Release -o ./publish
```

The published output will be in the `./publish/wwwroot` directory.

## 🌐 Deployment

This project includes a GitHub Actions workflow for automatic deployment to GitHub Pages. On every push to the `main` branch:

1. The project is built and published
2. Static files are uploaded as artifacts
3. The site is deployed to GitHub Pages

## 💡 How It Works

### CssColors Class

The core `CssColors` class provides:

- **Luminance Calculation**: Implements WCAG 2.0 relative luminance formula using proper gamma correction
- **Contrast Ratio Calculation**: Determines contrast ratios between colors to ensure accessibility
- **Text Color Selection**: Automatically chooses black or white text based on background luminance
- **Color Blending**: Creates color variants by blending the main color with black or white at specified opacity levels

### Color Variants

The application generates the following variants from any selected color:

| Variant | Description |
|---------|-------------|
| Main Color 10 | 15% opacity over white |
| Main Color 20 | 35% opacity over white |
| Main Color 30 | 65% opacity over white |
| Main Color 40 | 80% opacity over white |
| Main Color | The original selected color |
| Main Color 60 | 65% opacity over black |
| Main Color 70 | 35% opacity over black |
| Main Color 80 | 15% opacity over black |

## 🧪 Development

### Running in Development Mode

```bash
dotnet watch --project src/ColorPicker.App
```

This enables hot reload for rapid development.

### Solution Structure

The solution uses the standard .NET project structure with:
- `ColorPicker.sln` - Visual Studio solution file
- `src/ColorPicker.App` - Main Blazor WebAssembly application

