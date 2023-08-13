## Key eviction test
```powershell
.\run-eviction-policies.ps1
```

## Probabilistic cache test
```powershell
.\run-probabilistic-cache.ps1
```

## Simple cache test
```powershell
.\run-simple-cache.ps1
```

### Results (probabilistic cache)
```
Transactions:                  40262 hits
Availability:                 100.00 %
Elapsed time:                  64.80 secs
Data transferred:               3.84 MB
Response time:                  0.02 secs
Transaction rate:             621.33 trans/sec
Throughput:                     0.06 MB/sec
Concurrency:                    9.97
Successful transactions:       40262
Failed transactions:               0
Longest transaction:            1.13
Shortest transaction:           0.00
```

### Results (simple cache)
```
Transactions:                  44961 hits
Availability:                 100.00 %
Elapsed time:                  64.64 secs
Data transferred:               4.29 MB
Response time:                  0.01 secs
Transaction rate:             695.56 trans/sec
Throughput:                     0.07 MB/sec
Concurrency:                    9.92
Successful transactions:       44961
Failed transactions:               0
Longest transaction:            5.32
Shortest transaction:           0.00
```

Even though probabilistic cache ensures only one external call is made per expiry time, it's slower than simple cache due to multiple calls (TTL, isComputing).