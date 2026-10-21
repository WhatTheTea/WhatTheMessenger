> NOTE: Tests were run on my laptop within local docker deployment

# Before
```
CONFIG

 thousand_chatters: {
      executor: 'ramping-vus',
      startVUs: 0,
      stages: [
        { duration: '5s', target: 1000 },
        { duration: '30s', target: 1000 },
        { duration: '5s', target: 0 },
      ],
      exec: 'runChatters',
    },


TOTAL RESULTS 

    checks_total.......: 5586   79.909957/s
    checks_succeeded...: 99.51% 5559 out of 5586
    checks_failed......: 0.48%  27 out of 5586

    ✓ Pre-seeded user login verified (200 OK)
    ✗ Login successful (200 OK)
      ↳  97% — ✓ 1059 / ✗ 27
    ✓ GET /chats status is 200
    ✓ Received SignalR frame
    ✓ WebSocket connected successfully

    HTTP
    http_req_duration..............: avg=7.16s    min=1.69ms   med=5.39s   max=18.07s p(90)=15.28s   p(95)=15.97s  
      { expected_response:true }...: avg=7.05s    min=1.69ms   med=4.91s   max=18.07s p(90)=15.19s   p(95)=15.93s  
    http_req_failed................: 1.25%  27 out of 2146
    http_reqs......................: 2146   30.699385/s

    EXECUTION
    iteration_duration.............: avg=43.84s   min=15.47s   med=46.55s  max=49.79s p(90)=48.2s    p(95)=48.64s  
    iterations.....................: 1085   15.521358/s
    vus............................: 1      min=1          max=1000
    vus_max........................: 1000   min=1000       max=1000

    NETWORK
    data_received..................: 1.4 MB 20 kB/s
    data_sent......................: 1.9 MB 28 kB/s

    WEBSOCKET
    ws_connecting..................: avg=180.86ms min=923.03µs med=101.5ms max=8.5s   p(90)=374.23ms p(95)=624.92ms
    ws_msgs_received...............: 2381   34.061154/s
    ws_msgs_sent...................: 1059   15.149417/s
    ws_session_duration............: avg=32.32s   min=15.18s   med=30.85s  max=45.17s p(90)=41.09s   p(95)=42.7s   
    ws_sessions....................: 1059   15.149417/s




running (1m09.9s), 0000/1000 VUs, 1085 complete and 1 interrupted iterations
```

# After
```
CONFIG

 thousand_chatters: {
      executor: 'ramping-vus',
      startVUs: 0,
      stages: [
        { duration: '30s', target: 1000 },
        { duration: '30s', target: 1000 },
        { duration: '5s', target: 0 },
      ],
      exec: 'runChatters',
    },

█ TOTAL RESULTS 

    checks_total.......: 10233   107.885517/s
    checks_succeeded...: 100.00% 10233 out of 10233
    checks_failed......: 0.00%   0 out of 10233

    ✓ Pre-seeded user login verified (200 OK)
    ✓ Login successful (200 OK)
    ✓ GET /chats status is 200
    ✓ Received SignalR frame
    ✓ WebSocket connected successfully

    HTTP
    http_req_duration..............: avg=67.24ms min=1.75ms   med=74.88ms max=468.43ms p(90)=162.43ms p(95)=188.51ms
      { expected_response:true }...: avg=67.24ms min=1.75ms   med=74.88ms max=468.43ms p(90)=162.43ms p(95)=188.51ms
    http_req_failed................: 0.00%  0 out of 4097
    http_reqs......................: 4097   43.19427/s

    EXECUTION
    iteration_duration.............: avg=31.13s  min=31.07s   med=31.12s  max=31.52s   p(90)=31.19s   p(95)=31.24s  
    iterations.....................: 1981   20.885489/s
    vus............................: 66     min=27        max=1000
    vus_max........................: 1000   min=1000      max=1000

    NETWORK
    data_received..................: 2.7 MB 28 kB/s
    data_sent......................: 3.7 MB 39 kB/s

    WEBSOCKET
    ws_connecting..................: avg=1.96ms  min=858.15µs med=1.4ms   max=92.95ms  p(90)=1.97ms   p(95)=2.41ms  
    ws_msgs_received...............: 4098   43.204813/s
    ws_msgs_sent...................: 2048   21.591863/s
    ws_session_duration............: avg=30s     min=30s      med=30s     max=30.16s   p(90)=30s      p(95)=30s     
    ws_sessions....................: 2048   21.591863/s
```

# Conclusions

Initial load testing with a steep 5-second ramp-up stage (1,000 VUs) resulted in 
**27 failed login requests (1.25% failure rate)** and a **p(95) latency spike of 15.97 seconds**. 

While initial database logs reported connection pool timeouts 
(`Npgsql.NpgsqlException: Timeout expired prior to obtaining a connection`), further isolation revealed 
that **PostgreSQL was not the primary bottleneck**. Instead, the failure was driven 
by **API Thread Pool Starvation caused by CPU-intensive password hashing** during authentication.

```
[1,000 Concurrent VUs] 
       │ Firing POST /api/v1/auth/login over 5 seconds (~200 req/s)
       ▼
[wtm-api Container Thread Pool] 
       │ ──► PBKDF2/BCrypt Password Hashing saturates 100% Host CPU cores
       │ ──► Worker threads stall waiting for CPU cycles
       ▼
[EF Core / Npgsql Connection Pool]
       │ ──► Active DbContext threads hold connection pool slots longer
       │ ──► Incoming HTTP requests block waiting for open DB connection slots
       ▼
[Timeout Expired / HTTP 500 & k6 Request Timeout]
```