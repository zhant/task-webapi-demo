$users = @("John Doe Minor", "Jane Doe", "Jim Doe")

$users | ForEach-Object {
    $userToCreateJson = @{
        "Name" = $_
    } | ConvertTo-Json

    Invoke-RestMethod `
    -Uri "http://localhost:5188/task-management/" `
    -Method Post `
    -Body $userToCreateJson `
    -ContentType "application/json"
}