# Script to fix Mapperly mapper mocks in test files
# Replaces Mock<XxxMapper> with real XxxMapper instances

$testFiles = Get-ChildItem -Path "tests\NoviVovi.Application.Tests" -Recurse -Filter "*Tests.cs" | 
    Where-Object { (Get-Content $_.FullName -Raw) -match "Mock<.*Mapper>" }

foreach ($file in $testFiles) {
    Write-Host "Processing: $($file.Name)"
    
    $content = Get-Content $file.FullName -Raw
    $modified = $false
    
    # Pattern 1: Replace Mock<XxxMapper> field declarations
    if ($content -match "private readonly Mock<(\w+Mapper)> _mock(\w+);") {
        $content = $content -replace "private readonly Mock<(\w+Mapper)> _mock(\w+);", 'private readonly $1 _mock$2;'
        $modified = $true
    }
    
    # Pattern 2: Replace Mock<XxxMapper> instantiation
    if ($content -match "_mock(\w+) = new Mock<(\w+Mapper)>\(\);") {
        $content = $content -replace "_mock(\w+) = new Mock<(\w+Mapper)>\(\);", '_mock$1 = new $2();'
        $modified = $true
    }
    
    # Pattern 3: Replace .Object references
    if ($content -match "_mock(\w+)\.Object") {
        $content = $content -replace "_mock(\w+)\.Object", '_mock$1'
        $modified = $true
    }
    
    if ($modified) {
        Set-Content -Path $file.FullName -Value $content -NoNewline
        Write-Host "  Modified" -ForegroundColor Green
    } else {
        Write-Host "  No changes needed" -ForegroundColor Yellow
    }
}

Write-Host ""
Write-Host "Done! Processed $($testFiles.Count) files."
