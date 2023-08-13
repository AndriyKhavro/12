$maxmemory = $args[0]
If ($null -eq $maxmemory) {
    $maxmemory = "100mb"
}

$evictionPolicy = $args[1]
If ($null -eq $evictionPolicy) {
    $evictionPolicy = "noeviction"
}

docker compose down

$env:maxmemory=$maxmemory
$env:maxmemory_policy=$evictionPolicy

Write-Host "Set maxmemory to $env:maxmemory"
Write-Host "Set maxmemory-policy to $env:maxmemory_policy"

docker compose up -d --build