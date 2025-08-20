# Azure Cache for Redis Tiers

Azure Cache for Redis offers multiple **tiers** that differ in performance, features, and price. Here’s the breakdown:

## 1. Basic Tier
- **Single-node** deployment (no replication or SLA).
- Entry-level, low-cost option.
- No data replication or automatic failover.
- Best for **dev/test** or non-critical workloads.

## 2. Standard Tier
- **Two-node replicated cache** (primary + replica).
- Provides **99.9% SLA**.
- Automatic failover if the primary node goes down.
- Best for **production workloads** that need reliability.

## 3. Premium Tier
- All features of Standard, plus:
  - **Better performance** (lower latency, higher throughput).
  - **Persistence** (RDB snapshots, AOF persistence).
  - **Virtual Network (VNet) integration** for isolation/security.
  - **Clustering** (scale beyond the max size of a single node).
  - **Up to 120 GB** cache size per shard.
- Best for **mission-critical** apps.

## 4. Enterprise Tier
- Based on **Redis Enterprise** (from Redis Labs).
- Built-in **Active-Active geo-replication** (CRDTs).
- Higher availability and resilience.
- Enterprise-grade features like **Redis Modules** (RediSearch, Bloom, JSON, TimeSeries, etc.).
- Stronger SLA and more advanced scaling.
- Best for **global-scale applications**.

## 5. Enterprise Flash Tier
- Uses **DRAM + NVMe flash storage** for cost-efficient scaling.
- Can handle **terabyte-scale caches**.
- Same benefits as Enterprise, but at lower cost per GB.
- Best for **large datasets** that don’t fit in-memory only.

---

## ✅ Summary Table

| Tier                | SLA     | Replication | Clustering | Persistence | VNet Support | Advanced Features                  |
|---------------------|---------|-------------|------------|-------------|--------------|------------------------------------|
| **Basic**           | None    | No          | No         | No          | No           | None                               |
| **Standard**        | 99.9%   | Yes         | No         | No          | No           | None                               |
| **Premium**         | 99.9%   | Yes         | Yes        | Yes         | Yes          | None                               |
| **Enterprise**      | Higher  | Yes         | Yes        | Yes         | Yes          | Redis Modules, Active-Active       |
| **Enterprise Flash**| Higher  | Yes         | Yes        | Yes         | Yes          | Huge dataset support (DRAM + Flash)|