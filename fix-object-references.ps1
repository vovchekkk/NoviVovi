# Script to fix .Object references in test constructors

$testFiles = Get-ChildItem -Path "tests\NoviVovi.Application.Tests" -Recurse -Filter "*Tests.cs"

foreach ($file in $testFiles) {
    $content = Get-Content $file.FullName -Raw
    $modified = $false
    
    # Fix common patterns where .Object is missing in handler constructors
    $patterns = @(
        @{Pattern = "(_mockNovelRepo|_mockRepository|_mockLabelRepo|_mockCharacterRepo|_mockImageRepo),"; Replacement = "`$1.Object,"},
        @{Pattern = "(_mockUnitOfWork),"; Replacement = "`$1.Object,"},
        @{Pattern = "(_mockNovelRepo|_mockRepository|_mockLabelRepo|_mockCharacterRepo|_mockImageRepo)\s*\)"; Replacement = "`$1.Object)"},
        @{Pattern = "(_mockUnitOfWork)\s*\)"; Replacement = "`$1.Object)"}
    )
    
    foreach ($pattern in $patterns) {
        if ($content -match $pattern.Pattern) {
            $content = $content -replace $pattern.Pattern, $pattern.Replacement
            $modified = $true
        }
    }
    
    if ($modified) {
        Set-Content -Path $file.FullName -Value $content -NoNewline
        Write-Host "Fixed: $($file.Name)" -ForegroundColor Green
    }
}

Write-Host ""
Write-Host "Done!"
