# Design Delta – Minimum Changes for Zero-Loss + No-Duplicate

This document defines the **minimum architectural changes** required to meet strict financial guarantees.

---

## Delta 1: Processing Authority Lease (Cosmos)
**Problem:** DNS-based failover can route traffic to multiple regions  
**Change:**
- Single Cosmos document acting as a lease
- Conditional (ETag) updates
- Short TTL renewal by active region

**Result:** Enforced single processing authority

---

## Delta 2: Durable Posting Ledger (Cosmos)
**Problem:** Redis Premium is lossy under failover  
**Change:**
- New Cosmos container: PostingLedger
- Partition key = transactionId
- Conditional state transitions

**States:**
- RECEIVED
- POST_INTENT
- POST_SENT
- POST_CONFIRMED
- UNKNOWN

**Result:** Exactly-once posting control

---

## Delta 3: ACK Semantics Tied to Durability
**Problem:** ACK before durability breaks RPO  
**Change:**
- ACK only after Cosmos commit (request + ledger)
- Fail closed if Cosmos unavailable

**Result:** Enforceable zero data loss

---

## Delta 4: Treat FIS Timeouts as UNKNOWN
**Problem:** Blind retries cause duplicates  
**Change:**
- Never mark timeout as FAILED
- Reconcile via ledger before retry

**Result:** No duplicate financial postings

---

## Delta 5: Health vs Readiness Probes
**Problem:** Cold-path dependencies trigger false failover  
**Change:**
- Separate liveness vs readiness-for-posting endpoints
- TM probes reflect readiness, not runtime liveness

**Result:** Stable failover behavior

---

## Delta 6: Reconciliation Worker
**Problem:** Ambiguous post outcomes  
**Change:**
- Timer/Event Hub worker scans UNKNOWN states
- Controlled retry or manual escalation (tail only)

**Result:** Automated recovery within ≤4h RTO