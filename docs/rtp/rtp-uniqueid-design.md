# Breakdown

## 1. UETR
- **Scope:** Global, end-to-end.  
- **Persistence:** Must remain the same across all messages (`pacs.008`, `pacs.002`, `camt.056`, `camt.029`, `camt.054`, etc.) for the same transaction.  
- **Creator:** Originating FI.  
- **Variation:** Does not vary. This is your safest "primary key."  

## 2. EndToEndId
- **Scope:** Customer → Customer (originator reference).  
- **Persistence:** Typically stays constant across all related messages for a transaction, but...  
  - It comes from the originator’s payment instruction.  
  - If a return/reversal happens, the receiving FI may supply a new EndToEndId.  
- **Variation:** Usually stable, but not enforced to be globally unique. May repeat across unrelated payments.  

## 3. InstrId
- **Scope:** Instructing agent’s internal reference (their system’s "instruction ID").  
- **Persistence:**  
  - Can differ between the initial payment and a return.  
  - May not be preserved in all message types, depending on the FI’s systems.  
- **Variation:** Often varies. Think of it as "local system ID," not an end-to-end key.  

## 4. TxId
- **Scope:** Unique within a chain of FI-to-FI transactions (from instructing bank to instructed bank).  
- **Persistence:**  
  - The same TxId is expected to appear in acknowledgments (`pacs.002`) corresponding to a `pacs.008`.  
  - But a different TxId may be used in a return or related transaction.  
- **Variation:** Stable only within one bilateral leg of the transaction. It does not persist across the whole RTP lifecycle.  

---

## Summary Table

| Field       | Who sets it           | Scope                 | Across lifecycle |
|-------------|----------------------|-----------------------|-----------------|
| **UETR**    | Originating FI       | Global, network-wide  | Stable always |
| **EndToEndId** | Originator (customer) | Customer instruction  | Usually stable, but may change in returns |
| **InstrId** | Instructing FI       | FI’s internal process | Varies frequently |
| **TxId**    | Instructing FI       | FI-to-FI leg          | Stable per leg, but not end-to-end |

---

So, in practical terms:  
- Key off **UETR** if you want one ID that survives across all related messages.  
- Use **EndToEndId** if you need to align with the originator’s own references (e.g., for customer service).  
- Treat **InstrId** and **TxId** as contextual/local IDs, not lifecycle anchors.  
