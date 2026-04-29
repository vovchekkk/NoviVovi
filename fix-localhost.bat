@echo off
:: Fix Localhost Resolution - Easy Launcher
:: Double-click this file to fix localhost connection issues

echo.
echo ============================================
echo   Localhost Resolution Fix Tool
echo ============================================
echo.
echo This will fix the "Connection refused: localhost" error
echo.
echo Press any key to continue or close this window to cancel...
pause >nul

:: Run PowerShell script with admin privileges
powershell -ExecutionPolicy Bypass -Command "Start-Process powershell -ArgumentList '-ExecutionPolicy Bypass -File \"%~dp0fix-localhost-production.ps1\"' -Verb RunAs"

echo.
echo Script launched. Please check the PowerShell window that opened.
echo.
timeout /t 3 >nul
