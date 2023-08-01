$users = @("John Doe Minor", "Jane Doe", "Jim Doe")

$users | ForEach-Object {
    $userToCreateJson = @{
        "Name" = $_
    } | ConvertTo-Json

    Invoke-RestMethod `
    -Uri "http://localhost:8000/task-management/users" `
    -Method Post `
    -Body $userToCreateJson `
    -ContentType "application/json"
}