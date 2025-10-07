# RTP API Performance Test Plan — Phase 1 (Isolated Endpoint Tests)

## 1. Objective
Establish baseline performance and P90 response times for each RTP Receive API endpoint under varying throughput levels, tested **in isolation**.

This phase focuses on identifying:
- The 90th percentile (P90) response time across throughput levels (1–50 TPS).
- The relative contribution of three processing legs:
  1. **API HTTP Azure Function processing**
  2. **HTTP Azure Function → FIS request/response**
  3. **FIS response → HTTP Azure Function response assembly**

Future phases will reconfigure the workflow to test **end-to-end sequential chaining** of the endpoints.

---

## 2. Scope
### In-Scope Endpoints
| Endpoint | Description | Auth | Notes |
|-----------|--------------|------|-------|
| `/account-validation` | Validate account details for RTP receive transaction | OAuth2 Signed-JWT | Machine-to-machine only |
| `/ofac-screen` | Screen participant against OFAC list | OAuth2 Signed-JWT | Stateless call |
| `/account-posting` | Post funds to DDA via FIS | OAuth2 Signed-JWT | Backend latency expected |
| `/events` | Receive asynchronous events (4 event types) | OAuth2 Signed-JWT | Response payload only logged |

Each endpoint will be tested independently.

---

## 3. Metrics & KPIs

| Metric | Description | Target / Evaluation |
|---------|--------------|--------------------|
| **P90 Response Time** | 90% of requests complete ≤ X ms | Baseline only (no strict SLA yet) |
| **Error Rate** | Ratio of failed HTTP responses | < 1% acceptable |
| **Throughput (TPS)** | Sustained transactions per second | 1, 2, 3, 4, 5, 10, 15, 20, 25, 50 |
| **System Resource Metrics** | CPU, memory, I/O on function apps and backend | For analysis only |
| **Leg Timings** | 1. Function duration, 2. FIS round-trip, 3. Response assembly | Logged, exported for deep dive |

---

## 4. Test Environment

| Component | Environment | Notes |
|------------|--------------|-------|
| **API Host** | Azure Function App (Prod-like) | Isolated slot preferred |
| **Backend** | FIS sandbox or test instance | Same region as Function App |
| **Auth** | OAuth2 Signed JWT | Client credentials via service principal |
| **Load Agent** | k6 Docker container | Executed from same VNet or low-latency region |
| **Time Synchronization** | NTP | For accurate timing comparison across systems |

---

## 5. Tools & Frameworks

| Purpose | Tool | Notes |
|----------|------|-------|
| Load Generation | [Grafana k6](https://k6.io) | JS test scripts, Trend metrics |
| Containerization | Docker / Podman | Detached mode execution |
| Metrics Visualization | Grafana + InfluxDB | Optional for continuous tests |
| Source Conversion | Postman → k6 | Use `postman-to-k6` NPM package |

Conversion command example:
```bash
npx postman-to-k6 collection.json -o test.js
```

---

## 6. Test Scenarios

### 6.1 Throughput Matrix

| TPS Level | Duration | Warm-up | Notes |
|------------|-----------|---------|-------|
| 1 | 2 min | 30s | Baseline latency |
| 2 | 2 min | 30s |  |
| 3 | 2 min | 30s |  |
| 4 | 2 min | 30s |  |
| 5 | 2 min | 30s |  |
| 10 | 2 min | 30s |  |
| 15 | 2 min | 30s |  |
| 20 | 2 min | 30s |  |
| 25 | 2 min | 30s |  |
| 50 | 3 min | 30s | Stress test |

Each run produces:
- Raw results (`summary.json`)
- Log files with 3-leg timings
- Aggregated P90 per run

---

## 7. Success Criteria

1. Each endpoint’s P90 remains stable (<20% variance) between 3 consecutive runs.  
2. Error rate ≤ 1%.  
3. No unhandled exceptions or timeouts at 50 TPS.  
4. Backend service logs provide usable per-leg timings.

---

## 8. Reporting

- Summary JSON from k6 includes:
  - P90, P95, P99 percentiles  
  - Error rate  
  - Iteration counts and durations  
- Logs with detailed per-leg timings exported to Azure Log Analytics or file share.  
- Results compiled into a comparison matrix by endpoint and TPS level.

---

## 9. Container Execution

### Option A — Simple Docker Run
```bash
docker run -d --name k6-rtp-test   -v $(pwd):/scripts   -e BASE_URL=https://api.yourdomain.com   -e TOKEN=$(cat token.txt)   grafana/k6 run /scripts/test.js
```

### Option B — Podman Compose (Detached)

**compose.yml**
```yaml
services:
  k6:
    image: grafana/k6:latest
    container_name: k6-rtp
    environment:
      - BASE_URL=https://api.yourdomain.com
      - TOKEN_FILE=/secrets/token.txt
    volumes:
      - ./scripts:/scripts
      - ./secrets:/secrets:ro
      - ./results:/results
    command: ["run", "/scripts/test.js"]
    restart: "no"
```

**Run with:**
```bash
podman-compose up -d
```

**Optional Grafana/InfluxDB stack (add under `services:`):**
```yaml
  influxdb:
    image: influxdb:2
    ports: ["8086:8086"]
  grafana:
    image: grafana/grafana
    ports: ["3000:3000"]
```

---

## 10. Next Steps / Open Items

- [ ] Confirm authentication method (JWT signing key or client secret).  
- [ ] Provide Postman collection for conversion.  
- [ ] Confirm backend FIS endpoint visibility from load environment.  
- [ ] Decide output format for timing logs (structured JSON vs text).  
- [ ] Define thresholds for acceptable latency in future SLA/SLO phase.

---

## 11. Version Control

| Version | Date | Author | Description |
|----------|------|---------|-------------|
| 0.1 | YYYY-MM-DD | Daniel | Initial draft (isolated endpoint tests) |
| 0.2 | TBD | TBD | Sequential chaining scenario |
| 1.0 | TBD | TBD | Final baseline report |

---

## 12. Next Step

Once the following are confirmed:
- **Authentication flow:** JWT signing vs. token fetch endpoint  
- **Postman collection filename:** for conversion to k6 script  
- **Log destination:** stdout, file, or InfluxDB  

Then the next deliverable will be:
- A matching **`test.js` k6 script** with scenario configuration and  
- A **structured log schema** to capture each call leg:  
  1. API processing time  
  2. FIS request/response time  
  3. Response assembly time  