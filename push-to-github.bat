@echo off
echo ========================================
echo   Push Bloodstrap to GitHub
echo ========================================
echo.

REM Initialize git if not already done
if not exist ".git" (
    echo Initializing git repository...
    git init
    git remote add origin https://github.com/ethantheDeveloper220/vibetrap.git
)

echo.
echo Adding all files...
git add .

echo.
echo Committing changes...
set /p commit_message="Enter commit message (or press Enter for default): "
if "%commit_message%"=="" set commit_message=Update Bloodstrap v0.0.5

git commit -m "%commit_message%"

echo.
echo Pushing to GitHub...
git branch -M main
git push -u origin main --force

echo.
echo ========================================
echo   Push Complete!
echo ========================================
echo.
echo Your code is now at:
echo https://github.com/ethantheDeveloper220/vibetrap
echo.

pause
