# MCP Environment Setup Script for Chop Chop Development
# This script loads environment variables from .env file

Write-Host "========================================" -ForegroundColor Cyan
Write-Host " Chop Chop MCP Environment Setup" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Check if .env file exists
$envFile = Join-Path $PSScriptRoot ".env"
if (-not (Test-Path $envFile)) {
    Write-Host "❌ .env file not found!" -ForegroundColor Red
    Write-Host "Please create a .env file with your API keys." -ForegroundColor Yellow
    Write-Host "See docs/MCP_SETUP_GUIDE.md for instructions." -ForegroundColor Yellow
    Read-Host "Press Enter to exit"
    exit 1
}

Write-Host "Loading environment variables from .env file..." -ForegroundColor Yellow
Write-Host ""

# Load .env file
Get-Content $envFile | ForEach-Object {
    if ($_ -match '^([^#][^=]+)=(.*)$') {
        $key = $matches[1].Trim()
        $value = $matches[2].Trim()
        [Environment]::SetEnvironmentVariable($key, $value, "Process")
        Write-Host "✅ $key loaded" -ForegroundColor Green
    }
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host " Environment Setup Complete!" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Next steps:" -ForegroundColor White
Write-Host "1. Restart VS Code to pick up environment variables" -ForegroundColor White
Write-Host "2. Test Chop Chop AI agent with MCP servers" -ForegroundColor White
Write-Host "3. Check VS Code output panel for MCP server status" -ForegroundColor White
Write-Host ""

Read-Host "Press Enter to continue"
