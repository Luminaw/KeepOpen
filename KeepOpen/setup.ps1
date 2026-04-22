<#
.SYNOPSIS
    Sets up or removes KeepOpen as a startup task in Windows.

.DESCRIPTION
    Registers a Scheduled Task that runs KeepOpen at user logon with highest privileges.
    Uses native PowerShell cmdlets for better reliability and self-elevation.
#>

param (
    [switch]$Uninstall
)

$TaskName = "KeepOpen"
$BinaryName = "KeepOpen.exe"
$CurrentDir = $PSScriptRoot

# 1. Self-Elevation Logic
$IsAdmin = ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole] "Administrator")
if (-not $IsAdmin) {
    Write-Host "Elevating privileges to manage scheduled tasks..." -ForegroundColor Cyan
    $Arguments = "-NoProfile -ExecutionPolicy Bypass -File `"$PSCommandPath`""
    if ($Uninstall) { $Arguments += " -Uninstall" }
    
    try {
        Start-Process powershell.exe -ArgumentList $Arguments -Verb RunAs -Wait
        exit
    }
    catch {
        Write-Host "Error: This script must be run as Administrator." -ForegroundColor Red
        Write-Host "Please right-click your terminal and select 'Run as Administrator'."
        exit 1
    }
}

$ExePath = Join-Path $CurrentDir $BinaryName

if ($Uninstall) {
    Write-Host "Uninstalling KeepOpen..." -ForegroundColor Yellow
    
    # Stop and Unregister Task
    if (Get-ScheduledTask -TaskName $TaskName -ErrorAction SilentlyContinue) {
        Write-Host "Removing scheduled task..." -ForegroundColor Gray
        Unregister-ScheduledTask -TaskName $TaskName -Confirm:$false
        $DeleteSuccess = $true
    }
    else {
        $DeleteSuccess = $false
    }
    
    # Kill the process if running
    Write-Host "Terminating any running instances of $BinaryName..." -ForegroundColor Gray
    Stop-Process -Name (([System.IO.Path]::GetFileNameWithoutExtension($BinaryName))) -Force -ErrorAction SilentlyContinue
    
    if ($DeleteSuccess) {
        Write-Host "Successfully removed KeepOpen from startup." -ForegroundColor Green
    }
    else {
        Write-Host "Cleanup complete (task was not found)." -ForegroundColor Gray
    }
    exit
}

# 2. Check if binary exists
if (-not (Test-Path $ExePath)) {
    Write-Host "Error: Could not find $BinaryName in $CurrentDir" -ForegroundColor Red
    Write-Host "Ensure you are running this script from the same folder as the executable."
    exit 1
}

Write-Host "Registering KeepOpen as a startup task..." -ForegroundColor Cyan

# 3. Create the task using native cmdlets
# We use -WorkingDirectory to ensure the app can find appsettings.json
$Action = New-ScheduledTaskAction -Execute $ExePath -WorkingDirectory $CurrentDir
$Trigger = New-ScheduledTaskTrigger -AtLogOn
$Settings = New-ScheduledTaskSettingsSet -AllowStartIfOnBatteries -DontStopIfGoingOnBatteries -ExecutionTimeLimit 0

try {
    # Principal defines WHO runs the task and at what level
    # Using the current user context but with Highest privileges
    Register-ScheduledTask -TaskName $TaskName -Action $Action -Trigger $Trigger -Settings $Settings -RunLevel Highest -Force
    
    Write-Host "Successfully registered KeepOpen to run at startup!" -ForegroundColor Green
    Write-Host "The app will now start automatically whenever you log in."
    Write-Host "You can run '.\setup.ps1 -Uninstall' at any time to remove it."
}
catch {
    Write-Host "Error: Failed to register startup task." -ForegroundColor Red
    Write-Host $_.Exception.Message
    exit 1
}
