public class Pacs008Message
{
    public string MessageId { get; set; }                    // <GrpHdr><MsgId>
    public DateTime CreationDateTime { get; set; }           // <GrpHdr><CreDtTm>
    public string InstructionId { get; set; }                // <CdtTrfTxInf><PmtId><InstrId>
    public string EndToEndId { get; set; }                   // <CdtTrfTxInf><PmtId><EndToEndId>
    public string TransactionId { get; set; }                // <CdtTrfTxInf><PmtId><TxId>
    public string DebtorName { get; set; }                   // <CdtTrfTxInf><Dbtr><Nm>
    public string DebtorAccount { get; set; }                // <CdtTrfTxInf><DbtrAcct><Id><IBAN>
    public string CreditorName { get; set; }                 // <CdtTrfTxInf><Cdtr><Nm>
    public string CreditorAccount { get; set; }              // <CdtTrfTxInf><CdtrAcct><Id><IBAN>
    public string Currency { get; set; }                     // <CdtTrfTxInf><Amt><InstdAmt Ccy="">
    public decimal Amount { get; set; }                      // <CdtTrfTxInf><Amt><InstdAmt>
    public string PurposeCode { get; set; }                  // <CdtTrfTxInf><Purp><Cd>
}
