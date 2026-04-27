# Script to add _mockStorageService to Character Get tests

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
        
        # Add _mockStorageService field if not present
        if ($content -notmatch "Mock<IStorageService> _mockStorageService") {
            $content = $content -replace "(private readonly Mock<ICharacterRepository> _mockCharacterRepo;)", "`$1`r`n    private readonly Mock<IStorageService> _mockStorageService;"
        }
        
        # Add _mockStorageService initialization if not present
        if ($content -notmatch "_mockStorageService = new Mock<IStorageService>") {
            $content = $content -replace "(_mockCharacterRepo = new Mock<ICharacterRepository>\(\);)", "`$1`r`n        _mockStorageService = new Mock<IStorageService>();`r`n        _mockStorageService.Setup(s => s.GetViewUrl(It.IsAny<string>())).Returns(`"https://test.com/view`");"
        }
        
        Set-Content -Path $file -Value $content -NoNewline
        Write-Host "  Fixed" -ForegroundColor Green
    }
}

Write-Host ""
Write-Host "Done!"
