# Resilient Active/Passive Architecture Explanation

## High-Level Purpose

This architecture is designed to:
- Prevent data loss
- Avoid duplicate processing
- Enable fast, controlled failover
- Maintain message order and integrity

Even in the face of regional outages, message corruption, or service failure.

---

## System Overview

There are two Azure regions:
- **DC04 (US West 2)** — typically active  
- **DC06 (US West Central)** — typically passive

Each region runs the same full set of components:
- `QueueBuilder` — reads from MTS MQ (on-prem)
- `Transformation Runner` — transforms and delivers messages
- `Watchdog/Recovery Function` — monitors health, controls failover, and replays messages if needed

---

## Region Activation Control: Redis

A replicated Redis cache holds a single flag:

```
active_dc = DC04  # or DC06
```

Every component checks this flag before acting:
- If the region is not active, it doesn't process messages.
- This prevents dual-processing and enforces control over where work happens.

The flag is monitored and updated by the Watchdog component.

---

## Message Ingestion: QueueBuilder

QueueBuilder in each region:
- Pulls messages from on-prem `MTS MQ`
- Writes the message to **Blob Storage** (write-ahead log)
- Sends it to **Azure Service Bus** for processing
- Checks `active_dc` before doing anything

If it’s the active region, it proceeds. If not, it halts.

---

## Message Processing: Transformation Runner

Triggered by messages in Service Bus:
- Checks **Cosmos DB** to see if this message ID was already processed
- If not, it transforms the message (e.g., camt.053 to MT950)
- Delivers the result to the on-prem `MessageWay` system
- Writes to Blob again (optional backup)
- Records processing in Cosmos DB (idempotency)

This function is stateless but relies on **Cosmos DB for deduplication** and **Blob for backup**.

---

## Recovery: Watchdog/Recovery Function

A single function performs both:
- **Monitoring**:
  - Checks health of the local region (QueueBuilder, SB, Blob, Redis)
  - If failure detected, updates Redis to point to the other region
  - Can trigger a **Service Bus alias failover** if needed
- **Recovery**:
  - Scans Blob Storage for unprocessed messages
  - Replays them into the queue (if in the active region)

---

## Replication and Redundancy

| Component     | Replication Scope |
|---------------|-------------------|
| **Blob Storage** | Geo-replicated across regions |
| **Cosmos DB**     | Multi-region write-enabled or read-replica sync |
| **Redis**         | Replicated or managed for high availability |

Each region has a local copy of everything needed to pick up processing at any time.

---

## How It All Comes Together

1. **DC04 is active**. All traffic flows through it. DC06 is idle.
2. Messages are logged, processed, and delivered. Deduplication and backups ensure accuracy.
3. If **DC04 fails**:
   - Watchdog in DC06 detects the failure
   - It flips the Redis flag to `DC06`
   - Now DC06 begins processing from the same queue with the same logic
   - No need to reconfigure routing — just a role switch

---

## Benefits

| Objective               | How It's Achieved |
|-------------------------|--------------------|
| **No data loss**        | Blob-based write-ahead log + recovery logic |
| **No duplicates**       | Cosmos DB deduplication using message IDs |
| **Maintains order**     | Service Bus FIFO and single active consumer region |
| **Fast recovery**       | Watchdog flips active region and restarts processing |
| **Scales easily**       | Both regions fully provisioned — only one is active at a time |