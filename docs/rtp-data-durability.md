# RTP Data Durability

This section explains how our RTP middleware ensures **zero data loss** and **no duplicate processing** under any failure condition, using only **Cosmos DB** and **Redis**. The design meets the requirement of RPO = last committed transaction and RTO ≤ 4 hours.


## Purpose

The purpose of this layer is to ensure all critical transaction data is captured and not lost, regardless of what fails:

- A region outage
- Redis cache failures or desync
- Code crashes or container restarts
- Broken connection between datacenters

It also ensures we **never process a transaction more than once**, which is critical for financial applications.


## Core Components

| Component             | Purpose                                                                 |
|-----------------------|-------------------------------------------------------------------------|
| **Message API Call**  | Receives RTP-related API calls from the vendor (e.g., AccountPost, ADV) |
| **Cosmos DB**         | Durable, indexed storage for all API messages (by type)                 |
| **Redis Cache**       | Tracks which transactions have already been processed (deduplication)   |


## Workflow Summary

1. **Message Received**
   - API receives a call from vendor GPT (e.g., AccountPost or ADV).
   
2. **Cosmos Write (Durable Storage)**
   - The message payload is written to the appropriate Cosmos DB collection (e.g., AccountPost, OFAC, ADV).
   - Cosmos DB is set to geo-replicate to the secondary region.

3. **Redis Entry (Deduplication Flag)**
   - A transaction ID is written to Redis with a flag like `false` or `pending` to indicate the message has not yet been processed.

4. **Processing Logic**
   - Processing logic checks Redis before doing any downstream work.
   - If the flag is already `true`, the transaction is skipped (prevents duplicates).
   - Once processing is successfully completed, Redis is updated to `true`.


## Key Resilience Features

| Feature                     | Description                                                                 |
|-----------------------------|-----------------------------------------------------------------------------|
| **Cosmos DB Geo-Replication** | Ensures data is available in both regions for fast recovery                  |
| **Redis Deduplication Check** | Prevents duplicate processing even if the same message arrives twice         |
| **Component Region Awareness** | Components only execute if their region is currently marked "active" (via Redis) |
| **Conflict Protection**     | If Redis replication fails, fallback deduping logic can be invoked using Cosmos |


## Failure Scenarios & Recovery

| Failure Scenario                         | Result                                     | Recovery Path                                               |
|------------------------------------------|--------------------------------------------|--------------------------------------------------------------|
| **Region failure (e.g., West 2 outage)** | Cosmos replicated to paired region         | Switch to secondary region, resume from Cosmos + Redis       |
| **Redis cache outage**                  | Deduplication flag may be unavailable      | Block processing; resume after Redis availability or verify via Cosmos |
| **API or processing crash**             | Data persisted in Cosmos                   | Reprocess transaction using ID; Redis ensures no duplicates  |
| **Network split between datacenters**   | Redis may become stale                     | Use active-region flag, prevent double execution, reconcile on recovery |
| **Code bug or function restart**        | Message may re-trigger                     | Redis deduplication ensures reprocessing is safe             |


## Summary

This design eliminates data loss and avoids duplicates by relying on:

- **Durable storage** in Cosmos DB
- **In-memory flags** in Redis
- **Cross-region checks** and failover controls
- **Region-aware processing logic** (only active region executes)

It is designed for high-resilience middleware operating in a financial transaction environment, meeting strict RPO and RTO guarantees without relying on queues.

