# Script to fix Label tests with complete mapper dependency chain

$labelTests = Get-ChildItem -Path "tests\NoviVovi.Application.Tests\Labels" -Filter "*Tests.cs"

foreach ($file in $labelTests) {
    Write-Host "Processing: $($file.Name)"
    
    $content = Get-Content $file.FullName -Raw
    
    # Replace the mapper initialization block
    $oldPattern = "var characterMapper = new CharacterDtoMapper\(\);\s+var sizeMapper = new SizeDtoMapper\(\);\s+var imageMapper = new ImageDtoMapper\(_mockStorageService\.Object, sizeMapper\);\s+var transformMapper = new TransformDtoMapper\(\);\s+var stepMapper = new StepDtoMapper\(characterMapper, imageMapper, transformMapper\);"
    
    $newInit = @"
var sizeMapper = new SizeDtoMapper();
        var imageMapper = new ImageDtoMapper(_mockStorageService.Object, sizeMapper);
        var transformMapper = new TransformDtoMapper();
        var characterStateMapper = new CharacterStateDtoMapper(imageMapper, transformMapper);
        var characterMapper = new CharacterDtoMapper(characterStateMapper);
        var stepMapper = new StepDtoMapper(characterMapper, imageMapper, transformMapper);
"@
    
    if ($content -match $oldPattern) {
        $content = $content -replace $oldPattern, $newInit
        Set-Content -Path $file.FullName -Value $content -NoNewline
        Write-Host "  Fixed" -ForegroundColor Green
    } else {
        Write-Host "  No match found" -ForegroundColor Yellow
    }
}

Write-Host ""
Write-Host "Done!"
