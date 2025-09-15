# RTO / RPO Discussion

## Recovery Point Objective (RPO)
- **Definition:** The maximum tolerable amount of data loss measured in time.  
- **Practical Meaning:** Defines how far back in time the system must be able to recover data after a failure.  
- **Examples:**
  - **24 hours:** Nightly backups; risk of losing a full day’s worth of data.
  - **15 minutes:** Database log shipping or near-real-time replication.
  - **Zero (0):** Synchronous replication; no data loss tolerated.

**Guiding Question:** *“If the system fails right now, how much data are we allowed to lose?”*

---

## Recovery Time Objective (RTO)
- **Definition:** The maximum acceptable amount of downtime before the system must be restored.  
- **Practical Meaning:** Defines how quickly the service must be brought back online after a failure.  
- **Examples:**
  - **4 hours:** Critical internal applications must be back up same business day.
  - **24 hours:** Non-critical systems can be restored next day.
  - **Near-zero:** Mission-critical platforms require automated failover with almost no downtime.

**Guiding Question:** *“If the system fails right now, how quickly must it be back up and running?”*

---

## Tradeoffs
- **Shorter RPO/RTO = Higher Cost**
  - More frequent backups or real-time replication.
  - More resilient infrastructure (multi-region, clustering, hot standby).
- **Longer RPO/RTO = Lower Cost**
  - Simpler backup solutions.
  - Longer allowable downtime and potential data loss.

---

## Business Alignment
- **RPO** aligns with *data protection* needs (loss tolerance).  
- **RTO** aligns with *availability* needs (downtime tolerance).  
- Both must be defined by **business impact analysis** and balanced with cost and technical feasibility.
