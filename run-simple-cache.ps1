.\up.ps1

$network = "app-tier"

docker run --name siege --network $network --rm -t yokogawa/siege -c 1 -r 1 --content-type "application/json" http://app/test #warmup
docker run --name siege --network $network --rm -t yokogawa/siege -b -t 65s -c 50 --content-type "application/json" http://app/test
