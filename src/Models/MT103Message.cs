public class MT103Message
{
    public string TransactionReferenceNumber { get; set; }   // :20:
    public string BankOperationCode { get; set; }            // :23B:
    public DateTime ValueDate { get; set; }                  // :32A: (Value Date + Currency + Amount)
    public string Currency { get; set; }                     // part of :32A:
    public decimal Amount { get; set; }                      // part of :32A:
    public string OrderingCustomer { get; set; }             // :50A/K/F:
    public string BeneficiaryCustomer { get; set; }          // :59:
    public string RemittanceInformation { get; set; }        // :70:
    public string DetailsOfCharges { get; set; }             // :71A:
}
