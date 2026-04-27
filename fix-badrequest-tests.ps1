# Script to fix all BadRequest expectations to UnprocessableEntity
# DomainException is mapped to 422 (UnprocessableEntity), not 400 (BadRequest)

$testFiles = Get-ChildItem -Path "tests\NoviVovi.Api.Tests" -Recurse -Filter "*Tests.cs"

foreach ($file in $testFiles) {
    $content = Get-Content $file.FullName -Raw
    $modified = $false
    
    # Replace BadRequest expectations with UnprocessableEntity for domain validation errors
    # Pattern: Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    if ($content -match "Assert\.Equal\(HttpStatusCode\.BadRequest, response\.StatusCode\)") {
        $content = $content -replace "Assert\.Equal\(HttpStatusCode\.BadRequest, response\.StatusCode\)", "Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode)"
        $modified = $true
    }
    
    # Also update test method names to reflect the change
    if ($content -match "_ReturnsBadRequest\(\)") {
        $content = $content -replace "_ReturnsBadRequest\(\)", "_ReturnsUnprocessableEntity()"
        $modified = $true
    }
    
    if ($modified) {
        Set-Content -Path $file.FullName -Value $content -NoNewline
        Write-Host "Fixed: $($file.Name)" -ForegroundColor Green
    }
}

Write-Host ""
Write-Host "Done! All BadRequest expectations changed to UnprocessableEntity."
