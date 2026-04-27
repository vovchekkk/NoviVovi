# Script to fix Label tests with LabelDtoMapper dependencies

$labelTests = @(
    "tests\NoviVovi.Application.Tests\Labels\GetLabelHandlerTests.cs",
    "tests\NoviVovi.Application.Tests\Labels\GetLabelsHandlerTests.cs",
    "tests\NoviVovi.Application.Tests\Labels\PatchLabelHandlerTests.cs"
)

$usingsToAdd = @"
using NoviVovi.Application.Characters.Mappers;
using NoviVovi.Application.Images.Mappers;
using NoviVovi.Application.Scene.Mappers;
using NoviVovi.Application.Steps.Mappers;
"@

$mapperInitCode = @"
        
        // LabelDtoMapper requires StepDtoMapper, which requires other mappers
        var characterMapper = new CharacterDtoMapper();
        var imageMapper = new ImageDtoMapper();
        var transformMapper = new TransformDtoMapper();
        var stepMapper = new StepDtoMapper(characterMapper, imageMapper, transformMapper);
        _mockMapper = new LabelDtoMapper(stepMapper);
"@

foreach ($file in $labelTests) {
    if (Test-Path $file) {
        Write-Host "Processing: $file"
        
        $content = Get-Content $file -Raw
        
        # Add missing usings after existing usings
        if ($content -match "(using NoviVovi\.Application\.Labels\.Mappers;)") {
            $content = $content -replace "(using NoviVovi\.Application\.Labels\.Mappers;)", "`$1`r`n$usingsToAdd"
        }
        
        # Replace mapper initialization
        $content = $content -replace "_mockMapper = new LabelDtoMapper\(\);", $mapperInitCode
        
        # Fix .Object references in constructor
        $content = $content -replace "(_mockNovelRepo|_mockLabelRepo|_mockRepository),", "`$1.Object,"
        $content = $content -replace "(_mockUnitOfWork),", "`$1.Object,"
        
        Set-Content -Path $file -Value $content -NoNewline
        Write-Host "  Fixed" -ForegroundColor Green
    }
}

Write-Host ""
Write-Host "Done!"
