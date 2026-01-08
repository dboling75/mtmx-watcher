# Disaster Recovery Test Matrix

This document maps each disaster scenario to the **expected system behavior** for the Azure-based RTP solution.

## System Invariants
- No duplicate postings to FIS IBS
- RPO = last committed (zero data loss for ACKed requests)
- Single active processing authority
- Automated recovery within ≤4 hours

---

## A. Traffic Authority & Ingress

### A1. Traffic Manager Probe Degradation / Flapping
**Scenario:** Probe latency or intermittent failures cause false failover  
**Expected Behavior:**
- Only lease-holding region processes writes/posting
- Non-authoritative region fails closed (503)
- No split-brain processing

### A2. DNS Cache Staleness After Failover
**Scenario:** Clients pinned to old region via DNS TTL  
**Expected Behavior:**
- Both regions may receive traffic
- Only lease holder processes
- No ACK unless lease + durability checks pass

### A3. Traffic Manager Control Plane Impairment
**Scenario:** TM unable to update routing  
**Expected Behavior:**
- Application-level lease enforces single writer
- Safe degradation (availability loss acceptable, consistency not)

---

## B. Cosmos DB

### B1. Write Latency / RU Throttling
**Scenario:** Writes succeed slowly  
**Expected Behavior:**
- No ACK until durable write completes
- Retries are idempotent via ledger

### B2. Write Region Outage
**Scenario:** Primary write region unavailable  
**Expected Behavior:**
- Fail closed until failover completes
- Resume with no data loss or duplicates

### B3. Replication Lag Before Failover
**Scenario:** Writes acknowledged but not replicated  
**Expected Behavior:**
- ACK semantics aligned to defined “committed” rule
- Replay-safe after failover

---

## C. Redis Premium

### C1. Redis Failover with Lost Writes
**Scenario:** Acknowledged keys lost  
**Expected Behavior:**
- Redis treated as cache only
- Cosmos ledger prevents duplicates

### C2. Redis Full Outage
**Scenario:** Redis unavailable  
**Expected Behavior:**
- Cosmos-only dedupe for posting path
- Graceful degradation

### C3. Redis Geo Replication Lag
**Scenario:** Secondary region sees stale state  
**Expected Behavior:**
- Region relies on Cosmos ledger
- No duplicate postings

---

## D. FIS IBS Posting

### D1. Network Failure After Send
**Scenario:** Unknown post outcome  
**Expected Behavior:**
- Ledger state = UNKNOWN
- Reconciliation before retry
- Never blind re-post

### D2. FIS Latency / Function Timeout
**Scenario:** Slow responses  
**Expected Behavior:**
- Treat as UNKNOWN
- Ledger-driven retry logic

### D3. Failover with In-flight Posts
**Scenario:** Region switch mid-flight  
**Expected Behavior:**
- Ledger reconciliation
- No duplicate sends

---

## E. Event Hub

### E1. Backlog & Replay
**Scenario:** Consumer pause then resume  
**Expected Behavior:**
- Idempotent consumption
- Conditional Cosmos updates

---

## F. Key Vault

### F1. Key Vault Latency
**Scenario:** Slow secret access  
**Expected Behavior:**
- Probes reflect readiness
- Avoid false failover