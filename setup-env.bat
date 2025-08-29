@echo off
REM MCP Environment Setup Script for Chop Chop Development
REM This script loads environment variables from .env file

echo ========================================
echo  Chop Chop MCP Environment Setup
echo ========================================
echo.

REM Check if .env file exists
if not exist ".env" (
    echo ❌ .env file not found!
    echo Please create a .env file with your API keys.
    echo See docs/MCP_SETUP_GUIDE.md for instructions.
    pause
    exit /b 1
)

echo Loading environment variables from .env file...
echo.

REM Load .env file
for /f "tokens=1,2 delims==" %%a in (.env) do (
    if not "%%a"=="" if not "%%a:~0,1%"=="#" (
        set %%a=%%b
        echo ✅ %%a loaded
    )
)

echo.
echo ========================================
echo  Environment Setup Complete!
echo ========================================
echo.
echo Next steps:
echo 1. Restart VS Code to pick up environment variables
echo 2. Test Chop Chop AI agent with MCP servers
echo 3. Check VS Code output panel for MCP server status
echo.

pause
