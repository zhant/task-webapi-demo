Set-Variable -Name 'baseUrl' -Value 'http://localhost:5188/task-management/' -Option Constant
Set-Variable -Name 'numTasks' -Value 10
$apiEndpoint = $baseUrl + 'tasks'
for ($i=1; $i -le $numTasks; $i++) {
    $descriptionText = "My task description " + $i
    Invoke-RestMethod -Uri $apiEndpoint -Method Post -ContentType 'application/json' -Body (@{Description=$descriptionText} | ConvertTo-Json)
}