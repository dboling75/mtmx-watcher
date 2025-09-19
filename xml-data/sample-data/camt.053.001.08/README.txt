camt.053.001.08 — Wire-Focused Test Set
=========================================
Scope
-----
- 10 statements tailored for WIRE validation in SWIFT Translator.
- 100% of transaction entries are modeled as wire movements (domestic + cross‑border).

Modeling specifics
------------------
- Namespace: urn:iso:std:iso:20022:tech:xsd:camt.053.001.08
- BkTxCd for wires:
  * Credits: PMNT / RCDT / DMCT or XCRT
  * Debits:  PMNT / ICDT / DMCT or XCRT
- Refs per transaction: EndToEndId, TxId, UETR (UUID v4)
- Related Parties: Debtor/Creditor names + IBAN accounts
- Related Agents: DebtorAgent and CreditorAgent with BICFI
- Remittance: Unstructured text representative of wire narratives
- Dates: Booked and value date set to statement date for simplicity
- Balances: OPBD and CLBD included; Account owner and servicing BIC present

Fees
----
Two fee conventions are demonstrated:
1) Separate fee entries (DBIT) following the wire using BkTxCd domain CHRG (CHRG/CHRG/OTHR).
2) Netted fees for incoming credits (amount reduced by a small fee).

Notes
-----
- All data is fictitious and checksum‑agnostic; IBANs/BICs are plausible but not real.
- Use for translator/schema/conversion testing only; not for production or compliance validation.
