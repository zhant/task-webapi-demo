$userToCreateJson = @{
    "Name" = "John Doe"
} | ConvertTo-Json

Invoke-RestMethod `
-Uri "http://localhost:5188/task-management/" `
-Method Post `
-Body $userToCreateJson `
-ContentType "application/json"