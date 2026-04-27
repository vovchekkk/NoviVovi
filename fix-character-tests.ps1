# Script to fix Character tests with proper mapper dependencies

$characterTests = Get-ChildItem -Path "tests\NoviVovi.Application.Tests\Characters" -Filter "*Tests.cs"

$usingsToAdd = @"
using NoviVovi.Application.Images.Mappers;
using NoviVovi.Application.Scene.Mappers;
"@

foreach ($file in $characterTests) {
    Write-Host "Processing: $($file.Name)"
    
    $content = Get-Content $file.FullName -Raw
    $modified = $false
    
    # Add missing usings if not present
    if ($content -notmatch "using NoviVovi.Application.Images.Mappers;") {
        $content = $content -replace "(using NoviVovi.Application.Characters.Mappers;)", "`$1`r`n$usingsToAdd"
        $modified = $true
    }
    
    # Fix CharacterDtoMapper initialization
    if ($content -match "_mockMapper = new CharacterDtoMapper\(\);") {
        $mapperInit = @"
        
        // CharacterDtoMapper requires CharacterStateDtoMapper
        var imageMapper = new ImageDtoMapper();
        var transformMapper = new TransformDtoMapper();
        var characterStateMapper = new CharacterStateDtoMapper(imageMapper, transformMapper);
        _mockMapper = new CharacterDtoMapper(characterStateMapper);
"@
        $content = $content -replace "_mockMapper = new CharacterDtoMapper\(\);", $mapperInit
        $modified = $true
    }
    
    # Fix CharacterStateDtoMapper initialization (for AddCharacterState and PatchCharacterState tests)
    if ($content -match "_mockMapper = new CharacterStateDtoMapper\(\);") {
        $mapperInit = @"
        
        // CharacterStateDtoMapper requires dependencies
        var imageMapper = new ImageDtoMapper();
        var transformMapper = new TransformDtoMapper();
        _mockMapper = new CharacterStateDtoMapper(imageMapper, transformMapper);
"@
        $content = $content -replace "_mockMapper = new CharacterStateDtoMapper\(\);", $mapperInit
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
Write-Host "Done!"
