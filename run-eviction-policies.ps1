docker build . -t redis-test:0.0.1

$evictionPolicies = $("allkeys-lru", "allkeys-lfu", "allkeys-random", "volatile-lru", "volatile-lfu", "volatile-random", "volatile-ttl", "noeviction")

foreach ($policy in $evictionPolicies) {
    .\up.ps1 "35mb" $policy
    docker run --name redistest  --network app-tier --rm redis-test:0.0.1
}
