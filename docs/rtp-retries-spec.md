# Retry Strategy Options for RTP Account Posting API

## Overview
This document summarizes safe, proven retry strategies for handling posting requests when FIS IBS Core experiences predictable or unpredictable downtime during End-of-Day (EOD) processing or weekend maintenance windows.

Your Account Posting API must remain online 24/7/365 and respond immediately to UGP with clear machine-readable status codes, even when the FIS system is slow, timing out, or unavailable.

## 1. Key Assumptions and Prerequisites

### Idempotency Requirement
- UGP **must** include a unique transaction identifier.
- Your API must store and enforce idempotency so retries never double-post transactions.

### Status Codes Returned
- `SUCCESS` — FIS posted successfully.
- `PENDING_UNKNOWN` — timeout, no reply from FIS.
- `FAILED_TRANSIENT` — 5xx or system-down condition.
- Optional: `FAILED_FINAL` — non-recoverable error.

### Timeouts
- UGP timeout to your API: **3–5 seconds**
- Your API timeout to FIS Core: **2–3 seconds**, not minutes.

### Backoff + Jitter
All retry schemes must include exponential backoff with random jitter to prevent retry storms.

## 2. Retry Strategy 1: Stateless Exponential Backoff (Recommended Baseline)

UGP treats your status codes as signals for when and how aggressively to retry.

### A. When receiving `PENDING_UNKNOWN` (Timeout)
Use fast retries initially, then slow down if the outage persists.

**Fast Phase:**
- Retry 1: 1s
- Retry 2: 2s
- Retry 3: 4s
- Retry 4: 8s

**Slow Phase:**
- Retry 5: 1 min
- Retry 6: 2 min
- Retry 7: 4 min
- Retry 8: 8 min

**Cutoff:** 30 minutes total or ~8 retries.

### B. When receiving `FAILED_TRANSIENT` (System Down)
Assume the outage may last longer.

**Initial Phase:**
- Retry 1: 10s
- Retry 2: 30s

**Outage Phase:**
- Retry 3: 2 min
- Retry 4: 5 min
- Retry 5: 10 min
- Retry 6+: every 15 min

**Cutoff:** ~2 hours, then escalate manually.

## 3. Retry Strategy 2: Window-Aware Backoff (EOD / Maintenance Sensitive)

UGP adjusts retry behavior based on known problematic windows.

### Outside EOD / Maintenance
Use Strategy 1 unchanged.

### Inside EOD / Maintenance Windows
Assume delays may be long.

**Sanity Phase:**
- Retry 1: 15s
- Retry 2: 60s

**Slow Phase:**
- Retry 3: 5 min
- Retry 4: 10 min
- Retry 5+: every 15–30 min

**Cutoff:** 90 minutes or end of window + 30 minutes.

---

## 4. Architectural Upgrade Option (Recommended Long-Term)

Instead of requiring UGP to keep retrying posts:

1. UGP sends a **single SubmitTransaction** request.
2. Your API stores the request durably and returns `ACCEPTED_PENDING_POST`.
3. Your system asynchronously attempts posts with its own retry/backoff logic.
4. UGP calls **GET /transactions/{id}** or receives webhook updates for final status.

### Benefits:
- Guarantees 24/7/365 availability even if FIS is down.
- Eliminates retry storms.
- Reduces vendor-side complexity.
- Aligns with common patterns in payments and ACH/RTP systems.


## 5. Recommendation Summary

### If keeping UGP-managed retries:
- Use **Strategy 1** as baseline (fast then slow backoff).
- Enhance with **Strategy 2** if EOD/maintenance windows are predictable.
- Ensure strict idempotency and clear status codes.

### If evolving architecture:
- Adopt **Submit + Status Query** model for maximum resilience.


