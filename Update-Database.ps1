# File: Update-Database.ps1
# Purpose: Applies EF Core migrations to the database

# Ensure script runs with UTF-8 encoding
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8

try {
    Write-Host "🛠️ Starting database migration..." -ForegroundColor Cyan

    # Navigate to the project directory (adjust path if needed)
    Set-Location "C:\Users\Admin\Desktop\Xyz_Challenge\XYZUniversityAPI"

    # Run the EF Core update-database command
    dotnet ef database update

    Write-Host "✅ Migration applied successfully!" -ForegroundColor Green
}
catch {
    Write-Host "❌ Migration failed!" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Yellow
    exit 1
}
finally {
    Write-Host "🏁 Update script finished." -ForegroundColor Magenta
}
