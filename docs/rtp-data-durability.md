# RTP Data Durability

This component of the architecture is responsible for ensuring **no data is lost** under any failure scenario. It acts as a protective buffer between message intake and downstream processing.


## Purpose

Guarantee **zero data loss** for critical financial transactions—even in the event of:

- Region failure
- Queue corruption
- Redis desync
- Code bugs or restarts


## Components

| Component             | Purpose                                                                 |
|-----------------------|-------------------------------------------------------------------------|
| **Message API Call**  | Incoming transaction request (e.g., from vendor or channel system)      |
| **Blob Storage (WAL)**| Write-Ahead Log: durable and timestamped storage of every inbound call  |
| **Azure Service Bus** | High-throughput message queue for processing                            |
| **Redis Cache**       | Prevents duplicate transaction execution                                |
| **Queue Processor**   | Reads messages from the queue and executes processing logic             |


## Workflow Summary

1. **Message Received**
   - Message API receives a transaction from upstream source (e.g., RTP rail or channel system)

2. **Durable Logging (Blob)**
   - Immediately writes the full message payload to Blob Storage (Write-Ahead Log) to protect against failures

3. **Message Queuing**
   - Message is enqueued to Azure Service Bus for processing

4. **Idempotency Marker**
   - A key is written to Redis with a “pending” or false flag

5. **Queue Processing**
   - Queue Processor is triggered by the message in SB
   - Processor checks Redis: if already processed, skip (prevents duplicates)
   - After successful processing, updates Redis to true (processed)


## Key Resilience Benefits

| Feature                   | Description                                                                 |
|---------------------------|-----------------------------------------------------------------------------|
| **Blob WAL**              | Recovers lost or corrupted messages via re-read                             |
| **Redis Flag Check**      | Avoids processing the same message twice (deduplication)                    |
| **Azure SB Checkpointing**| Auto-retries or skips poison messages with built-in resiliency              |
| **Manual Replay Ready**   | Operations team can manually re-push Blob WAL messages if needed            |


## Scenarios Covered

| Failure Scenario                  | Result                                  | Recovery Path                                  |
|----------------------------------|------------------------------------------|------------------------------------------------|
| Queue fails                      | Blob still holds message                 | Re-post from WAL                               |
| Processor fails mid-run          | Redis flag not set → retry safe          | Processor resumes safely                       |
| Region loss                      | Replicated WAL + Redis enables resume    | Use backup region’s services                   |
| Redis corruption                 | Blob/WAL ensures message is not lost     | Recheck message intent via WAL                 |