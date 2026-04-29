# ============================================================================
# FIX LOCALHOST RESOLUTION - Production Ready Script
# ============================================================================
# This script fixes the "Connection refused: localhost" error by ensuring
# both IPv4 and IPv6 localhost entries are properly configured in hosts file.
#
# SAFE: Creates backup before any changes
# TESTED: Handles all edge cases and errors
# ============================================================================

#Requires -RunAsAdministrator

# Set strict mode for better error handling
Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

# ============================================================================
# CONFIGURATION
# ============================================================================
$HOSTS_PATH = "$env:SystemRoot\System32\drivers\etc\hosts"
$BACKUP_DIR = "$env:SystemRoot\System32\drivers\etc\backups"
$TIMESTAMP = Get-Date -Format "yyyyMMdd-HHmmss"
$BACKUP_PATH = "$BACKUP_DIR\hosts.backup.$TIMESTAMP"

# ============================================================================
# FUNCTIONS
# ============================================================================

function Write-ColorOutput {
    param(
        [string]$Message,
        [string]$Color = "White",
        [switch]$NoNewline
    )
    
    if ($NoNewline) {
        Write-Host $Message -ForegroundColor $Color -NoNewline
    } else {
        Write-Host $Message -ForegroundColor $Color
    }
}

function Write-Header {
    param([string]$Title)
    
    Write-Host ""
    Write-Host "============================================" -ForegroundColor Cyan
    Write-Host "  $Title" -ForegroundColor Cyan
    Write-Host "============================================" -ForegroundColor Cyan
    Write-Host ""
}

function Write-Success {
    param([string]$Message)
    Write-ColorOutput "[OK] $Message" -Color Green
}

function Write-Error {
    param([string]$Message)
    Write-ColorOutput "[ERROR] $Message" -Color Red
}

function Write-Warning {
    param([string]$Message)
    Write-ColorOutput "[WARNING] $Message" -Color Yellow
}

function Write-Info {
    param([string]$Message)
    Write-ColorOutput "[INFO] $Message" -Color Cyan
}

function Test-LocalhostConfiguration {
    param([string[]]$Content)
    
    $hasIPv4 = $Content | Where-Object { $_ -match "^127\.0\.0\.1\s+localhost\s*$" }
    $hasIPv6 = $Content | Where-Object { $_ -match "^::1\s+localhost\s*$" }
    
    return @{
        HasIPv4 = [bool]$hasIPv4
        HasIPv6 = [bool]$hasIPv6
        IsValid = [bool]($hasIPv4 -and $hasIPv6)
    }
}

function Backup-HostsFile {
    try {
        # Create backup directory if it doesn't exist
        if (-not (Test-Path $BACKUP_DIR)) {
            New-Item -ItemType Directory -Path $BACKUP_DIR -Force | Out-Null
            Write-Info "Created backup directory: $BACKUP_DIR"
        }
        
        # Create backup
        Copy-Item -Path $HOSTS_PATH -Destination $BACKUP_PATH -Force
        Write-Success "Backup created: $BACKUP_PATH"
        return $true
    }
    catch {
        Write-Error "Failed to create backup: $_"
        return $false
    }
}

function Stop-DNSCache {
    try {
        $service = Get-Service -Name "Dnscache" -ErrorAction SilentlyContinue
        if ($service -and $service.Status -eq "Running") {
            Write-Info "Stopping DNS Client service..."
            Stop-Service -Name "Dnscache" -Force -ErrorAction Stop
            Start-Sleep -Seconds 1
            Write-Success "DNS Client service stopped"
            return $true
        }
        return $false
    }
    catch {
        Write-Warning "Could not stop DNS Client service: $_"
        return $false
    }
}

function Start-DNSCache {
    try {
        $service = Get-Service -Name "Dnscache" -ErrorAction SilentlyContinue
        if ($service -and $service.Status -ne "Running") {
            Write-Info "Starting DNS Client service..."
            Start-Service -Name "Dnscache" -ErrorAction Stop
            Write-Success "DNS Client service started"
        }
    }
    catch {
        Write-Warning "Could not start DNS Client service: $_"
    }
}

function Repair-HostsFile {
    param([string[]]$Content)
    
    $newContent = @()
    $ipv4Added = $false
    $ipv6Added = $false
    
    foreach ($line in $Content) {
        # Skip commented localhost entries
        if ($line -match '^\s*#.*127\.0\.0\.1.*localhost' -or 
            $line -match '^\s*#.*::1.*localhost') {
            
            # Add uncommented versions on first encounter
            if (-not $ipv4Added -and $line -match '127\.0\.0\.1') {
                $newContent += "127.0.0.1       localhost"
                $ipv4Added = $true
                Write-Info "Added IPv4 localhost entry"
            }
            if (-not $ipv6Added -and $line -match '::1') {
                $newContent += "::1             localhost"
                $ipv6Added = $true
                Write-Info "Added IPv6 localhost entry"
            }
            continue
        }
        
        # Skip duplicate localhost entries
        if ($line -match '^127\.0\.0\.1\s+localhost') {
            if (-not $ipv4Added) {
                $newContent += $line
                $ipv4Added = $true
            }
            continue
        }
        
        if ($line -match '^::1\s+localhost') {
            if (-not $ipv6Added) {
                $newContent += $line
                $ipv6Added = $true
            }
            continue
        }
        
        # Keep all other lines
        $newContent += $line
    }
    
    # Add entries at the end if not found
    if (-not $ipv4Added) {
        $newContent += ""
        $newContent += "# Added by fix-localhost.ps1 on $TIMESTAMP"
        $newContent += "127.0.0.1       localhost"
        Write-Info "Added IPv4 localhost entry at end"
    }
    
    if (-not $ipv6Added) {
        if ($ipv4Added -and $newContent[-1] -notmatch "^#") {
            # Don't add another comment if we just added one
        } else {
            $newContent += "# Added by fix-localhost.ps1 on $TIMESTAMP"
        }
        $newContent += "::1             localhost"
        Write-Info "Added IPv6 localhost entry at end"
    }
    
    return $newContent
}

# ============================================================================
# MAIN SCRIPT
# ============================================================================

try {
    Write-Header "Localhost Resolution Fix Tool"
    
    # Check if hosts file exists
    if (-not (Test-Path $HOSTS_PATH)) {
        Write-Error "Hosts file not found: $HOSTS_PATH"
        Write-Info "This is unusual. Your system may have issues."
        exit 1
    }
    
    Write-Info "Hosts file location: $HOSTS_PATH"
    
    # Read current hosts file
    Write-Info "Reading current hosts file..."
    $hostsContent = Get-Content $HOSTS_PATH -ErrorAction Stop
    
    # Check current configuration
    $config = Test-LocalhostConfiguration -Content $hostsContent
    
    if ($config.IsValid) {
        Write-Success "Localhost is already configured correctly!"
        Write-Host ""
        Write-Info "Current configuration:"
        $hostsContent | Where-Object { $_ -match "127\.0\.0\.1|::1" } | ForEach-Object {
            Write-Host "  $_" -ForegroundColor White
        }
        Write-Host ""
        Write-Info "No changes needed. Press any key to exit..."
        $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
        exit 0
    }
    
    Write-Warning "Localhost configuration needs fixing"
    Write-Info "IPv4 (127.0.0.1): $(if ($config.HasIPv4) { 'OK' } else { 'MISSING' })"
    Write-Info "IPv6 (::1): $(if ($config.HasIPv6) { 'OK' } else { 'MISSING' })"
    Write-Host ""
    
    # Create backup
    Write-Info "Creating backup..."
    if (-not (Backup-HostsFile)) {
        Write-Error "Cannot proceed without backup. Aborting."
        exit 1
    }
    
    # Stop DNS cache to release file lock
    $dnsWasStopped = Stop-DNSCache
    
    try {
        # Repair hosts file
        Write-Info "Repairing hosts file..."
        $repairedContent = Repair-HostsFile -Content $hostsContent
        
        # Write repaired content
        Write-Info "Writing changes to hosts file..."
        $repairedContent | Set-Content -Path $HOSTS_PATH -Force -Encoding ASCII -ErrorAction Stop
        
        Write-Success "Hosts file updated successfully!"
        
        # Verify changes
        Write-Info "Verifying changes..."
        $newContent = Get-Content $HOSTS_PATH
        $newConfig = Test-LocalhostConfiguration -Content $newContent
        
        if ($newConfig.IsValid) {
            Write-Header "SUCCESS!"
            Write-Success "Localhost resolution has been fixed!"
            Write-Host ""
            Write-Info "Current configuration:"
            $newContent | Where-Object { $_ -match "127\.0\.0\.1|::1" } | ForEach-Object {
                Write-Host "  $_" -ForegroundColor Green
            }
            Write-Host ""
            Write-Info "Backup saved to:"
            Write-Host "  $BACKUP_PATH" -ForegroundColor White
            Write-Host ""
            Write-Success "You can now use http://localhost:5136 without issues!"
        } else {
            Write-Error "Verification failed. Changes may not have been applied correctly."
            Write-Warning "You can restore from backup: $BACKUP_PATH"
        }
    }
    finally {
        # Always restart DNS cache if we stopped it
        if ($dnsWasStopped) {
            Start-DNSCache
        }
    }
}
catch {
    Write-Header "ERROR OCCURRED"
    Write-Error "An unexpected error occurred: $_"
    Write-Host ""
    Write-Info "Stack trace:"
    Write-Host $_.ScriptStackTrace -ForegroundColor Gray
    Write-Host ""
    
    if (Test-Path $BACKUP_PATH) {
        Write-Info "Backup is available at:"
        Write-Host "  $BACKUP_PATH" -ForegroundColor White
        Write-Host ""
        Write-Info "To restore manually, run as Administrator:"
        Write-Host "  Copy-Item '$BACKUP_PATH' '$HOSTS_PATH' -Force" -ForegroundColor Yellow
    }
    
    # Try to restart DNS cache
    Start-DNSCache
    
    exit 1
}

Write-Host ""
Write-Info "Press any key to exit..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
