<#
.SYNOPSIS
    Fixes relative paths in index.html for GitHub Pages deployment.
    
.DESCRIPTION
    This script updates stylesheet and script references in the published index.html
    to include the /color-picker/ base path for GitHub Pages deployment.
    
.PARAMETER PublishPath
    The path to the published wwwroot directory.
    
.PARAMETER BasePath
    The base path for GitHub Pages (e.g., /color-picker/).
    
.EXAMPLE
    .\fix-github-pages-paths.ps1 -PublishPath "./publish/wwwroot" -BasePath "/color-picker/"
#>

param(
    [Parameter(Mandatory=$true)]
    [string]$PublishPath,
    
    [Parameter(Mandatory=$true)]
    [string]$BasePath
)

$indexHtmlPath = Join-Path $PublishPath "index.html"

if (-not (Test-Path $indexHtmlPath)) {
    Write-Error "index.html not found at $indexHtmlPath"
    exit 1
}

Write-Host "Fixing GitHub Pages paths in $indexHtmlPath"
Write-Host "Base path: $BasePath"

# Read the index.html file
$content = Get-Content $indexHtmlPath -Raw

# Store original content for comparison
$originalContent = $content

# Fix stylesheet href attributes (but not the base href)
# Pattern: href="lib/... or href="css/... or href="ColorPicker...
$content = $content -replace 'href="(lib/|css/|ColorPicker)', "href=`"$BasePath`$1"

# Fix script src attributes (but not _framework paths which are already correct)
# Pattern: src="js/... (for custom scripts)
$content = $content -replace 'src="(js/)', "src=`"$BasePath`$1"

# Verify changes were made
if ($content -eq $originalContent) {
    Write-Host "Warning: No path replacements were made. Check if the file format matches expectations."
} else {
    Write-Host "Successfully updated paths in index.html"
}

# Write the updated content back
Set-Content $indexHtmlPath $content -NoNewline

Write-Host "GitHub Pages path fixes completed successfully"

