# Script to fix ImageDtoMapper initialization in all tests

$testFiles = Get-ChildItem -Path "tests\NoviVovi.Application.Tests" -Recurse -Filter "*Tests.cs" | 
    Where-Object { (Get-Content $_.FullName -Raw) -match "new ImageDtoMapper\(\)" }

foreach ($file in $testFiles) {
    Write-Host "Processing: $($file.Name)"
    
    $content = Get-Content $file.FullName -Raw
    $modified = $false
    
    # Add IStorageService mock if not present
    if ($content -notmatch "Mock<IStorageService>") {
        # Add field declaration after other mocks
        if ($content -match "(private readonly Mock<\w+> _mock\w+;)") {
            $content = $content -replace "(private readonly Mock<IUnitOfWork> _mock\w+;)", "`$1`r`n    private readonly Mock<IStorageService> _mockStorageService;"
            $modified = $true
        }
        
        # Add mock initialization in constructor
        if ($content -match "(\s+_mockUnitOfWork = new Mock<IUnitOfWork>\(\);)") {
            $content = $content -replace "(\s+_mockUnitOfWork = new Mock<IUnitOfWork>\(\);)", "`$1`r`n        _mockStorageService = new Mock<IStorageService>();`r`n        _mockStorageService.Setup(s => s.GetViewUrl(It.IsAny<string>())).Returns(`"https://test.com/view`");"
            $modified = $true
        }
    }
    
    # Replace ImageDtoMapper initialization
    if ($content -match "var imageMapper = new ImageDtoMapper\(\);") {
        $content = $content -replace "var imageMapper = new ImageDtoMapper\(\);", "var sizeMapper = new SizeDtoMapper();`r`n        var imageMapper = new ImageDtoMapper(_mockStorageService.Object, sizeMapper);"
        $modified = $true
    }
    
    # Add using for IStorageService if not present
    if ($content -notmatch "using NoviVovi.Application.Common.Abstractions;") {
        $content = $content -replace "(using NoviVovi.Application.Common.Exceptions;)", "using NoviVovi.Application.Common.Abstractions;`r`n`$1"
        $modified = $true
    }
    
    if ($modified) {
        Set-Content -Path $file.FullName -Value $content -NoNewline
        Write-Host "  Fixed" -ForegroundColor Green
    } else {
        Write-Host "  No changes" -ForegroundColor Yellow
    }
}

Write-Host ""
Write-Host "Done! Processed $($testFiles.Count) files."
