# Script to add missing using directives for IStorageService

$characterGetTests = @(
    "tests\NoviVovi.Application.Tests\Characters\GetCharacterHandlerTests.cs",
    "tests\NoviVovi.Application.Tests\Characters\GetCharactersHandlerTests.cs",
    "tests\NoviVovi.Application.Tests\Characters\GetCharacterStateHandlerTests.cs",
    "tests\NoviVovi.Application.Tests\Characters\GetCharacterStatesHandlerTests.cs"
)

foreach ($file in $characterGetTests) {
    if (Test-Path $file) {
        Write-Host "Processing: $file"
        
        $content = Get-Content $file -Raw
        
        # Add using for IStorageService if not present
        if ($content -notmatch "using NoviVovi.Application.Common.Abstractions;") {
            $content = $content -replace "(using NoviVovi.Application.Characters.Abstractions;)", "using NoviVovi.Application.Common.Abstractions;`r`n`$1"
            Set-Content -Path $file -Value $content -NoNewline
            Write-Host "  Fixed" -ForegroundColor Green
        } else {
            Write-Host "  Already has using" -ForegroundColor Yellow
        }
    }
}

Write-Host ""
Write-Host "Done!"
