$userToCreateJson = @{
    "Name" = "John Doe"
} | ConvertTo-Json

Invoke-RestMethod `
-Uri "http://localhost:8000/task-management/users" `
-Method Post `
-Body $userToCreateJson `
-ContentType "application/json"