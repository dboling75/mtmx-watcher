# Disaster & Degradation Scenario Catalog (Azure Financial System)

This document catalogs **realistic disaster and failure scenarios** for an Azure-based, financial-grade system.  
Each scenario focuses on **degradation before failure**, **split-brain risk**, and **replication impairment**, not just clean outages.

Severity and likelihood are qualitative: **Low / Medium / High / Critical**.

---

## 1. Network Degradation Between Regions (No Failover)

**Type:** Inter-region network impairment  

**Description:**  
Connectivity between US WEST 2 and US WEST CENTRAL degrades (latency, packet loss, throttling) without triggering failover. Replication slows but traffic continues.

**Primary Issues:**  
- Replication lag  
- Inconsistent secondary state  
- ACKs issued without cross-region durability  

**Risks:**  
- Data loss after later failover  
- Duplicate processing due to missing dedupe state  

**Mitigations:**  
- Define “committed” explicitly (Cosmos primary or durable ledger)  
- ACK only after durable write  
- Monitor replication lag and fail closed when thresholds exceeded  

**Risk Severity:** High  
**Risk Likelihood:** High  

---

## 2. Network Degradation Followed by Failover

**Type:** Degradation → regional failover  

**Description:**  
Replication is impaired, then Traffic Manager or Cosmos initiates failover. Secondary activates with stale or incomplete data.

**Primary Issues:**  
- Lost acknowledged writes  
- Missing dedupe state  

**Risks:**  
- RPO violation  
- Duplicate FIS IBS postings  

**Mitigations:**  
- Durable commit ledger in Cosmos  
- Redis treated as cache only  
- Fail closed during failover window  

**Risk Severity:** Critical  
**Risk Likelihood:** Medium–High  

---

## 3. Split-Brain via DNS / Traffic Manager

**Type:** Control-plane inconsistency  

**Description:**  
DNS caching or probe flapping causes some clients to reach US WEST 2 and others US WEST CENTRAL simultaneously.

**Primary Issues:**  
- Dual writers  
- Conflicting state  

**Risks:**  
- Irreversible duplicate financial postings  

**Mitigations:**  
- Cosmos-based processing authority lease  
- Non-authoritative region must reject writes  
- Traffic Manager treated as routing hint only  

**Risk Severity:** Critical  
**Risk Likelihood:** Medium  

---

## 4. Partial Network Partition (Read-Only Illusion)

**Type:** Asymmetric network failure  

**Description:**  
A region can read data but cannot write to Cosmos primary, yet still receives traffic.

**Primary Issues:**  
- ACK without durability  
- Retry storms  

**Risks:**  
- Data loss  
- Duplicate retries  

**Mitigations:**  
- Enforce write-before-ACK invariant  
- Readiness probes must include write capability  
- Reject posting when write path unavailable  

**Risk Severity:** High  
**Risk Likelihood:** Medium  

---

## 5. Redis Replication Lag Without Redis Failure

**Type:** Eventual consistency exposure  

**Description:**  
Redis accepts writes, but replication lags. No Redis failover occurs.

**Primary Issues:**  
- Secondary region lacks dedupe state  
- False confidence in cache  

**Risks:**  
- Duplicate postings after region switch  

**Mitigations:**  
- Redis used only as performance cache  
- Cosmos ledger is source of truth  
- TTLs never define correctness  

**Risk Severity:** High  
**Risk Likelihood:** High  

---

## 6. Redis Failover with Acknowledged Write Loss

**Type:** Cache state loss  

**Description:**  
Redis primary fails and replica is promoted, losing recently acknowledged keys.

**Primary Issues:**  
- Lost dedupe entries  

**Risks:**  
- Duplicate FIS postings  

**Mitigations:**  
- Ledger-backed dedupe in Cosmos  
- Redis rebuilt from durable state  

**Risk Severity:** Critical  
**Risk Likelihood:** Medium  

---

## 7. Cosmos Write Latency / Throttling

**Type:** Performance degradation  

**Description:**  
Cosmos remains available but slow due to RU exhaustion or backend pressure.

**Primary Issues:**  
- Partial writes  
- Timeouts  

**Risks:**  
- Duplicate retries  
- Incorrect ACK semantics  

**Mitigations:**  
- Conditional writes with ETags  
- Ledger state written before side effects  
- Timeouts treated as UNKNOWN  

**Risk Severity:** High  
**Risk Likelihood:** High  

---

## 8. Cosmos Failover with In-Flight Transactions

**Type:** Mid-transaction failover  

**Description:**  
Cosmos fails over while requests are in progress, leaving ambiguous commit state.

**Primary Issues:**  
- Unknown outcomes  
- Inconsistent state views  

**Risks:**  
- Duplicate replay  
- Orphaned transactions  

**Mitigations:**  
- Monotonic ledger states  
- UNKNOWN + reconciliation worker  
- No blind retries  

**Risk Severity:** Critical  
**Risk Likelihood:** Medium  

---

## 9. FIS IBS Timeout / Disconnect

**Type:** External dependency ambiguity  

**Description:**  
Posting request sent to FIS IBS; network failure prevents knowing the outcome.

**Primary Issues:**  
- Exactly-once illusion collapse  

**Risks:**  
- Duplicate financial posting  

**Mitigations:**  
- Record POST_INTENT before send  
- Treat timeout as UNKNOWN  
- Reconcile before retry  

**Risk Severity:** Critical  
**Risk Likelihood:** Medium–High  

---

## 10. Event Hub Backlog and Replay

**Type:** Asynchronous replay amplification  

**Description:**  
Event Hub builds backlog during outage, then replays rapidly on recovery.

**Primary Issues:**  
- Duplicate consumption  
- Reordering  

**Risks:**  
- Secondary duplicate side effects  

**Mitigations:**  
- Idempotent consumers  
- Conditional Cosmos updates  

**Risk Severity:** Medium  
**Risk Likelihood:** High  

---

## 11. Key Vault Latency Causing False Failover

**Type:** Dependency-induced failover  

**Description:**  
Key Vault slowness causes Function cold-start delays and failed probes.

**Primary Issues:**  
- False Traffic Manager failover  
- Cascading split-brain risk  

**Risks:**  
- Dual-region processing  

**Mitigations:**  
- Separate liveness vs readiness probes  
- Cache secrets in memory  
- Probe only “safe-to-process” state  

**Risk Severity:** Medium–High  
**Risk Likelihood:** Medium  

---

## 12. Compound Silent Degradation

**Type:** Latent multi-factor failure  

**Description:**  
Multiple degraded components coexist without triggering alerts. Failover later exposes accumulated risk.

**Primary Issues:**  
- Undetected inconsistency  
- False confidence  

**Risks:**  
- Catastrophic duplicate or data loss event  

**Mitigations:**  
- Explicit replication-lag SLOs  
- Fail-closed thresholds  
- DR tests that inject degradation before outage  

**Risk Severity:** Critical  
**Risk Likelihood:** Medium  

---

## Key Takeaway

Most outages do not start as outages — they start as **degradation**.  
DR testing must validate behavior under **“yellow but wrong”** conditions, not just clean failures.