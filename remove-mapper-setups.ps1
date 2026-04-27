# Script to remove mapper .Setup() calls from test files

$testFiles = Get-ChildItem -Path "tests\NoviVovi.Application.Tests" -Recurse -Filter "*Tests.cs" | 
    Where-Object { (Get-Content $_.FullName -Raw) -match "_mockMapper|_mockTransformMapper" }

foreach ($file in $testFiles) {
    Write-Host "Processing: $($file.Name)"
    
    $lines = Get-Content $file.FullName
    $newLines = @()
    $skipBlock = $false
    $blockDepth = 0
    
    for ($i = 0; $i -lt $lines.Count; $i++) {
        $line = $lines[$i]
        
        # Check if this line starts a mapper setup block
        if ($line -match "^\s+_mock(Mapper|TransformMapper)\s*$") {
            # Look ahead to see if next line is .Setup
            if ($i + 1 -lt $lines.Count -and $lines[$i + 1] -match "^\s+\.Setup\(") {
                $skipBlock = $true
                $blockDepth = 0
                continue
            }
        }
        
        if ($skipBlock) {
            # Count parentheses to track nesting
            $openParens = ($line.ToCharArray() | Where-Object { $_ -eq '(' }).Count
            $closeParens = ($line.ToCharArray() | Where-Object { $_ -eq ')' }).Count
            $blockDepth += $openParens - $closeParens
            
            # Check if block ends with semicolon
            if ($line -match ";\s*$" -and $blockDepth -le 0) {
                $skipBlock = $false
            }
            continue
        }
        
        $newLines += $line
    }
    
    # Write back to file
    $newContent = $newLines -join "`r`n"
    Set-Content -Path $file.FullName -Value $newContent
    Write-Host "  Processed" -ForegroundColor Green
}

Write-Host ""
Write-Host "Done! Processed $($testFiles.Count) files."
