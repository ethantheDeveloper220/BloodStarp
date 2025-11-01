# Script to fix Bloxstrap namespace references to Voidstrap in XAML files

$files = @(
    "Bloxstrap\UI\Elements\Dialogs\AdvancedSettingsDialog.xaml",
    "Bloxstrap\UI\Elements\Dialogs\FlagDialog.xaml",
    "Bloxstrap\UI\Elements\Dialogs\GlobalSettingsDialog.xaml",
    "Bloxstrap\UI\Elements\Dialogs\AddFastFlagDialog_Froststrap.xaml",
    "Bloxstrap\UI\Elements\Settings\Pages\RobloxSettingsPage.xaml",
    "Bloxstrap\UI\Elements\Controls\SquareCard.xaml"
)

foreach ($file in $files) {
    $fullPath = Join-Path $PSScriptRoot $file
    if (Test-Path $fullPath) {
        Write-Host "Fixing $file..." -ForegroundColor Yellow
        $content = Get-Content $fullPath -Raw
        
        # Replace all Bloxstrap namespace references with Voidstrap
        $content = $content -replace 'x:Class="Bloxstrap\.', 'x:Class="Voidstrap.'
        $content = $content -replace 'clr-namespace:Bloxstrap\.', 'clr-namespace:Voidstrap.'
        
        Set-Content $fullPath $content -NoNewline
        Write-Host "  Fixed!" -ForegroundColor Green
    } else {
        Write-Host "  File not found: $fullPath" -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "All namespace fixes complete!" -ForegroundColor Cyan
